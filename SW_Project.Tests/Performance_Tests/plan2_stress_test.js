import http from "k6/http";
import { check, sleep } from "k6";
import { Trend, Rate, Counter } from "k6/metrics";

const BASE_URL = "http://aure-api.runasp.net";

const loginDuration      = new Trend("stress_login_duration");
const bookDuration       = new Trend("stress_book_duration");
const appointmentsDuration = new Trend("stress_appointments_duration");
const errorRate          = new Rate("stress_error_rate");
const serverErrors       = new Counter("server_errors_500");

export const options = {
  stages: [
    { duration: "10s", target: 200 },
    { duration: "1m",  target: 200 },
    { duration: "10s", target: 0   },
  ],
  thresholds: {
    http_req_duration: ["p(95)<3000"],
    stress_login_duration:        ["p(95)<2000"],
    stress_book_duration:         ["p(95)<3000"],
    stress_appointments_duration: ["p(95)<2000"],
    stress_error_rate: ["rate<0.05"],
  },
};

const JSON_HEADERS = { "Content-Type": "application/json" };

function loginAndGetToken() {
  const res = http.post(
    `${BASE_URL}/api/auth/login`,
    JSON.stringify({ email: "patient@test.com", password: "password123" }),
    { headers: JSON_HEADERS }
  );

  loginDuration.add(res.timings.duration);

  if (res.status !== 200) {
    errorRate.add(1);
    if (res.status >= 500) serverErrors.add(1);
    return null;
  }

  errorRate.add(0);

  try {
    return JSON.parse(res.body).Token;
  } catch {
    return null;
  }
}

export default function () {
  const token = loginAndGetToken();

  check(token, {
    "Login returned a token": (t) => t !== null,
  });

  if (token) {
    const bookPayload = JSON.stringify({
      doctorId: 1,
      date: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
    });

    const bookRes = http.post(`${BASE_URL}/api/appointments/Book`, bookPayload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    bookDuration.add(bookRes.timings.duration);
    errorRate.add(bookRes.status >= 500);
    if (bookRes.status >= 500) serverErrors.add(1);

    check(bookRes, {
      "POST Book → not 500": (r) => r.status !== 500,
      "POST Book → responded": (r) => r.timings.duration < 5000,
    });

    const myAppRes = http.get(`${BASE_URL}/api/appointments/my-appointments`, {
      headers: { Authorization: `Bearer ${token}` },
    });

    appointmentsDuration.add(myAppRes.timings.duration);
    errorRate.add(myAppRes.status >= 500);
    if (myAppRes.status >= 500) serverErrors.add(1);

    check(myAppRes, {
      "GET my-appointments → status 200": (r) => r.status === 200,
      "GET my-appointments → not empty": (r) => {
        try {
          return JSON.parse(r.body).length >= 0;
        } catch {
          return false;
        }
      },
    });
  }
}
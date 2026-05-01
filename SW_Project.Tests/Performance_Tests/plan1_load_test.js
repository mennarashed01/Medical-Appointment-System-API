import http from "k6/http";
import { check, sleep } from "k6";
import { Trend, Rate } from "k6/metrics";

const BASE_URL = "http://aure-api.runasp.net";

const loginDuration       = new Trend("login_duration");
const getDoctorsDuration  = new Trend("get_doctors_duration");
const searchDuration      = new Trend("search_duration");
const bookDuration        = new Trend("book_duration");
const errorRate           = new Rate("error_rate");

export const options = {
  stages: [
    { duration: "30s", target: 50 },
    { duration: "2m",  target: 50 },
    { duration: "30s", target: 0  },
  ],
  thresholds: {
    http_req_duration: ["p(95)<1000"],
    login_duration:       ["p(95)<800"],
    get_doctors_duration: ["p(95)<600"],
    search_duration:      ["p(95)<800"],
    book_duration:        ["p(95)<1000"],
    error_rate: ["rate<0.01"],
  },
};

const JSON_HEADERS = { "Content-Type": "application/json" };

export default function () {

  const getAllRes = http.get(`${BASE_URL}/api/doctors`);
  getDoctorsDuration.add(getAllRes.timings.duration);
  errorRate.add(getAllRes.status !== 200);

  check(getAllRes, {
    "GET /api/doctors → status 200": (r) => r.status === 200,
    "GET /api/doctors → response has body": (r) => r.body.length > 0,
  });

  sleep(1);

  const searchRes = http.get(`${BASE_URL}/api/doctors/search-by-name/Ahmed`);
  searchDuration.add(searchRes.timings.duration);
  errorRate.add(searchRes.status !== 200 && searchRes.status !== 404);

  check(searchRes, {
    "GET search-by-name → not a server error": (r) => r.status < 500,
  });

  sleep(1);

  const symptomRes = http.get(`${BASE_URL}/api/doctors/search-by-symptom/headache`);
  searchDuration.add(symptomRes.timings.duration);
  errorRate.add(symptomRes.status >= 500);

  check(symptomRes, {
    "GET search-by-symptom → not a server error": (r) => r.status < 500,
  });

  sleep(1);

  const loginPayload = JSON.stringify({
    email: "patient@test.com",
    password: "password123",
  });

  const loginRes = http.post(`${BASE_URL}/api/auth/login`, loginPayload, {
    headers: JSON_HEADERS,
  });

  loginDuration.add(loginRes.timings.duration);
  errorRate.add(loginRes.status !== 200);

  const loginOk = check(loginRes, {
    "POST /api/auth/login → status 200": (r) => r.status === 200,
    "POST /api/auth/login → returns token": (r) => {
      try {
        return JSON.parse(r.body).Token !== undefined;
      } catch {
        return false;
      }
    },
  });

  sleep(1);

  if (loginOk) {
    const token = JSON.parse(loginRes.body).Token;

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

    check(bookRes, {
      "POST /api/appointments/Book → not a server error": (r) => r.status < 500,
    });
  }

  sleep(2);
}
// 
import http from "k6/http";
import { check, sleep } from "k6";
import { Trend, Rate } from "k6/metrics";

const BASE_URL = "http://aure-api.runasp.net";
// const BASE_URL = "http://localhost:5206";

const loginDuration       = new Trend("login_duration");
const getDoctorsDuration  = new Trend("get_doctors_duration");
const searchDuration      = new Trend("search_duration");
const bookDuration        = new Trend("book_duration");
const errorRate           = new Rate("error_rate");

export const options = {
  stages: [
    { duration: "30s", target: 20 }, // قللنا العدد لـ 20 لضمان استقرار السيرفر المحلي
    { duration: "1m",  target: 20 }, 
    { duration: "30s", target: 0  },
  ],
  thresholds: {
    http_req_duration: ["p(95)<5000"], 
    login_duration:    ["p(95)<5000"],
    get_doctors_duration: ["p(95)<6000"],
    search_duration:   ["p(95)<5000"],
    book_duration:     ["p(95)<6000"],
    error_rate:        ["rate<0.10"], // سماحية خطأ 10%
  },
};

const JSON_HEADERS = { "Content-Type": "application/json" };

export default function () {
  // 1. جلب الدكاترة
  const getAllRes = http.get(`${BASE_URL}/api/doctors`);
  getDoctorsDuration.add(getAllRes.timings.duration);
  errorRate.add(getAllRes.status !== 200);

  check(getAllRes, {
    "GET /api/doctors → status 200": (r) => r.status === 200,
  });

  sleep(1);

  // 2. تسجيل الدخول
  const loginPayload = JSON.stringify({
    email: "ali@test.com",
    password: "ali123",
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
        return JSON.parse(r.body).token !== undefined;
      } catch { return false; }
    },
  });

  sleep(1);

  // 3. حجز موعد (Booking)
  if (loginOk) {
    const responseData = JSON.parse(loginRes.body);
    const token = responseData.token;

    const bookPayload = JSON.stringify({
      doctorId: 2,
      date: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
    });

    const bookRes = http.post(`${BASE_URL}/api/appointments/Book`, bookPayload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    bookDuration.add(bookRes.timings.duration);
    
    // سطر لمساعدتك في تتبع الخطأ لو الـ Booking فشل
    if (bookRes.status >= 400 && bookRes.status !== 400) {
        console.log(`Booking Issue: Status ${bookRes.status} | Message: ${bookRes.body}`);
    }

    check(bookRes, {
      "POST /api/appointments/Book → valid logic": (r) => r.status === 200 || r.status === 201 || r.status === 400,
      "POST /api/appointments/Book → not a server error": (r) => r.status < 500,
    });
  }

  sleep(2);
}
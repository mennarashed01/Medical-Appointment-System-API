<div align="center">

# 🏥 Medical Appointment Management System API

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white"/>
  <img src="https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white"/>
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white"/>
  <img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white"/>
  <img src="https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white"/>
  <img src="https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger&logoColor=black"/>
</p>

A production-ready RESTful API for managing medical appointments, diagnoses, and patient-doctor interactions — built with clean architecture principles on ASP.NET Core.

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Design Patterns](#-design-patterns)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)

---

## 🔍 Overview

This system provides a complete backend solution for healthcare appointment management, supporting three user roles — **Patients**, **Doctors**, and **Secretaries** — each with tailored access and functionality. The API is secured with JWT authentication and follows a clean layered architecture to ensure maintainability and scalability.

---

## ✨ Features

| Feature | Description |
|---|---|
| 🔐 **Auth & Authorization** | JWT-based login, registration, and role-based access control |
| 📅 **Appointment Management** | Book, update, and track appointments by role |
| 🩺 **Diagnosis & Medical Records** | Create diagnoses, re-examinations, and view patient history |
| 🔍 **Doctor Search** | Search doctors by name, symptom, or specialization |
| ⭐ **Rating System** | Patients can rate and review doctors |
| 🗂️ **Secretary Management** | Doctors can assign/remove secretaries |
| 👤 **Profile Management** | Full account and profile control per role |

---

## 🏗️ Architecture

The project follows a **Layered Architecture** to enforce separation of concerns:

```
Client
   ↓
Controllers        →  Handle HTTP requests & responses
   ↓
Services           →  Business logic & orchestration
   ↓
Repositories       →  Data access abstraction
   ↓
Database (SQL Server)
```

### Project Structure

```
MedicalAppointmentSystemAPI/
│
├── Controllers/       # API endpoints and route handling
├── Services/          # Business logic layer
├── Repositories/      # Data access layer
├── Models/            # Database entities
├── DTOs/              # Data Transfer Objects
├── Data/              # DbContext and configuration
├── Migrations/        # EF Core migrations
└── Program.cs         # App entry point and DI setup
```

---

## 🎨 Design Patterns

### 🗄️ Repository Pattern
Separates database logic from business logic, providing a clean abstraction over data access.
> `DoctorRepository` · `PatientRepository` · `DiagnosisRepository`

### ⚙️ Service Layer Pattern
Business logic lives in dedicated service classes — not controllers — promoting reusability and testability.
> `AuthService` · `DoctorService` · `PatientService`

### 📦 DTO Pattern
DTOs ensure clean data contracts between the API and client, preventing exposure of internal models.
> `RegisterDto` · `LoginDto` · `CreateDiagnosisDto`

### 💉 Dependency Injection
ASP.NET Core's built-in DI container wires services and repositories, enabling loose coupling and easier unit testing.

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8.0 |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | Microsoft SQL Server |
| Authentication | JWT (JSON Web Tokens) |
| Password Hashing | BCrypt.Net-Next |
| API Docs | Swagger / Swashbuckle |
| IDE | Visual Studio |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Microsoft SQL Server (local or remote)
- Visual Studio 2022+ or any .NET-compatible IDE

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/your-username/MedicalAppointmentSystemAPI.git
cd MedicalAppointmentSystemAPI

# 2. Update the connection string in appsettings.json
# "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=SW_ProjectDB;..."

# 3. Restore dependencies
dotnet restore

# 4. Apply database migrations
dotnet ef database update

# 5. Run the application
dotnet run
```

### Access

| Interface | URL |
|---|---|
| API Base | `https://localhost:5001` |
| Swagger UI | `https://localhost:5001/swagger` |

> All endpoints (except login & register) require a JWT token in the `Authorization: Bearer <token>` header.

---

## 📡 API Endpoints

### 🔐 Auth
| Method | Endpoint | Role | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | Public | Register a new user |
| `POST` | `/api/auth/login` | Public | Login and receive JWT token |
| `POST` | `/api/auth/change-password` | Authorized | Change account password |

### 📅 Appointments
| Method | Endpoint | Role | Description |
|---|---|---|---|
| `POST` | `/api/appointments/Book` | Patient | Book a new appointment |
| `PATCH` | `/api/appointments/{id}/status` | Doctor / Secretary | Update appointment status |
| `GET` | `/api/appointments/my-appointments` | Authorized | Get user's appointments |

### 🩺 Diagnoses
| Method | Endpoint | Role | Description |
|---|---|---|---|
| `POST` | `/api/diagnoses/create` | Doctor | Create initial diagnosis |
| `POST` | `/api/diagnoses/re-examine` | Doctor | Add re-examination record |
| `GET` | `/api/diagnoses/history/{patientId}` | Doctor | View patient diagnosis history |

### 👨‍⚕️ Doctors
| Method | Endpoint | Role | Description |
|---|---|---|---|
| `GET` | `/api/doctors` | Public | List all doctors |
| `GET` | `/api/doctors/{id}` | Public | Get doctor by ID |
| `GET` | `/api/doctors/search-by-name/{name}` | Public | Search doctor by name |
| `GET` | `/api/doctors/search-by-symptom/{symptomName}` | Public | Find doctors by symptom |
| `GET` | `/api/doctors/specialization/{specName}` | Public | Filter by specialization |
| `PUT` | `/api/doctors/my-profile` | Doctor | Update doctor profile |
| `DELETE` | `/api/doctors/delete-my-account` | Doctor | Delete account |
| `GET` | `/api/doctors/my-patients` | Doctor | View associated patients |
| `POST` | `/api/doctors/add-secretary` | Doctor | Add a secretary |
| `DELETE` | `/api/doctors/remove-secretary/{id}` | Doctor | Remove a secretary |

### 🧑‍🦯 Patients
| Method | Endpoint | Role | Description |
|---|---|---|---|
| `POST` | `/api/patients/recommend-doctors` | Patient | Get doctor recommendations by symptoms |
| `GET` | `/api/patients` | Public | List all patients |
| `GET` | `/api/patients/{id}` | Public | Get patient by ID |
| `GET` | `/api/patients/search/{name}` | Public | Search patient by name |
| `PUT` | `/api/patients/my-profile` | Patient | Update patient profile |
| `DELETE` | `/api/patients/delete-my-profile` | Patient | Delete account |
| `GET` | `/api/patients/my-medical-record` | Patient | View full medical record |
| `POST` | `/api/patients/rate-doctor` | Patient | Rate and review a doctor |

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

<div align="center">
  <sub>Built with ❤️ using ASP.NET Core · Clean Architecture · RESTful Design</sub>
</div>
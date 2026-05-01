# 🏥 Medical Appointment Management System API

## Description

A comprehensive medical appointment management system built with ASP.NET Core Web API. This application facilitates the management of medical appointments, diagnoses, and user interactions between patients, doctors, and secretaries.

## Features

- **User Authentication and Authorization**: Secure login, registration, and role-based access control for Patients, Doctors, and Secretaries.
- **Appointment Management**: Patients can book appointments, while doctors and secretaries can update appointment statuses.
- **Diagnosis and Medical Records**: Doctors can create initial diagnoses, perform re-examinations, and maintain patient medical histories.
- **Doctor Recommendations**: Patients can search for doctors based on symptoms and view recommendations.
- **Doctor Rating System**: Patients can rate and comment on doctors to help others make informed decisions.
- **Secretary Management**: Doctors can add and remove secretaries to assist with administrative tasks.
- **Profile Management**: Users can update their profiles and manage their accounts.

## Tools and Technologies

- **Framework**: .NET 8.0
- **Web Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt.Net-Next
- **API Documentation**: Swagger/Swashbuckle
- **Development Tools**: Visual Studio, Entity Framework Tools

## Architecture

The project follows a layered architecture to ensure separation of concerns and maintainability.

```
Client
  ↓
Controllers
  ↓
Services
  ↓
Repositories
  ↓
Database
```

Each layer has a specific responsibility:

- **Controllers**: Handle HTTP requests and responses, acting as the entry point for API calls.
- **Services**: Contain business logic and orchestrate operations between repositories and other services.
- **Repositories**: Manage data access and database operations, abstracting the data layer.
- **Models**: Represent database entities and define the data structure.
- **DTOs**: Transfer data between the API and client, ensuring clean data contracts.

## Project Structure

```
SW_Project
│
├── Controllers
├── Services
├── Repositories
├── Models
├── DTOs
├── Data
├── Migrations
└── Program.cs
```

## Design Patterns Used

### Repository Pattern
Used to separate database logic from business logic, providing a clean abstraction over data access.

Examples:
- `DoctorRepository`: Handles doctor-related database operations.
- `PatientRepository`: Manages patient data access.
- `DiagnosisRepository`: Deals with diagnosis records.

### Service Layer Pattern
Business logic is implemented in service classes instead of controllers, promoting reusability and testability.

Examples:
- `AuthService`: Manages authentication and authorization logic.
- `DoctorService`: Handles doctor-specific business operations.
- `PatientService`: Contains patient-related business rules.

### DTO Pattern
DTOs (Data Transfer Objects) are used to transfer data between the API and the client, avoiding exposing internal models.

Examples:
- `RegisterDto`: For user registration data.
- `LoginDto`: For login credentials.
- `CreateDiagnosisDto`: For diagnosis creation.

### Dependency Injection
ASP.NET Core's built-in dependency injection container is used to inject services and repositories, enabling loose coupling and easier testing.

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK installed on your machine.
- SQL Server installed and running.
- Visual Studio or any preferred IDE for .NET development.

### Installation Steps

1. **Clone the Repository**:
   ```
   git clone <repository-url>
   cd SW_Project
   ```

2. **Update Connection String**:
   - Open `SW_Project/appsettings.json`.
   - Update the `DefaultConnection` in `ConnectionStrings` to point to your SQL Server instance.
     ```
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=SW_ProjectDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"
     }
     ```

3. **Restore Packages**:
   ```
   dotnet restore
   ```

4. **Run Database Migrations**:
   ```
   dotnet ef database update
   ```

5. **Run the Application**:
   ```
   dotnet run
   ```

6. **Access the API**:
   - The application will start on `https://localhost:5001` (or as configured in `launchSettings.json`).
   - Swagger UI will be available at `https://localhost:5001/swagger` for API testing.

## API Endpoints Documentation

All endpoints require authentication except for login and registration. Include the JWT token in the `Authorization` header as `Bearer <token>`.

### Authentication Endpoints

- **POST** `/api/auth/login`
  - Body: `{ "email": "string", "password": "string" }`
  - Returns: JWT token and success message.

  **Example Request**

  ```
  POST /api/auth/login
  {
    "email": "test@example.com",
    "password": "123456"
  }
  ```

- **POST** `/api/auth/register`
  - Body: RegisterDto (includes user details and role)
  - Returns: Success message.

- **POST** `/api/auth/change-password` (Authorized)
  - Body: `{ "oldPassword": "string", "newPassword": "string" }`
  - Returns: Success message.

### Appointment Endpoints

- **POST** `/api/appointments/Book` (Patient role)
  - Body: BookAppointmentDto
  - Books an appointment for the patient.

- **PATCH** `/api/appointments/{id}/status` (Secretary, Doctor roles)
  - Body: `{ "status": "enum" }` (e.g., Pending, Confirmed, Completed)
  - Updates the status of an appointment.

- **GET** `/api/appointments/my-appointments` (Authorized)
  - Returns: List of user's appointments based on role.

### Diagnosis Endpoints (Doctor role)

- **POST** `/api/diagnoses/create`
  - Body: CreateDiagnosisDto
  - Creates an initial diagnosis for a patient.

- **POST** `/api/diagnoses/re-examine`
  - Body: ReExaminationDto
  - Adds a re-examination as a new version of diagnosis.

- **GET** `/api/diagnoses/history/{patientId}`
  - Returns: Diagnosis history for the specified patient.

### Doctor Endpoints

- **GET** `/api/doctors`
  - Returns: List of all doctors.

- **GET** `/api/doctors/{id}`
  - Returns: Doctor details by ID.

- **GET** `/api/doctors/search-by-name/{name}`
  - Returns: Doctors matching the name.

- **GET** `/api/doctors/search-by-symptom/{symptomName}`
  - Returns: Doctors who treat the specified symptom.

- **GET** `/api/doctors/specialization/{specName}`
  - Returns: Doctors with the specified specialization.

- **PUT** `/api/doctors/my-profile` (Doctor role)
  - Body: UpdateDoctorDto
  - Updates the doctor's profile.

- **DELETE** `/api/doctors/delete-my-account` (Doctor role)
  - Deletes the doctor's account.

- **GET** `/api/doctors/my-patients` (Doctor role)
  - Returns: List of patients associated with the doctor.

- **POST** `/api/doctors/add-secretary` (Doctor role)
  - Body: RegisterSecretaryDto
  - Adds a secretary to the doctor's team.

- **DELETE** `/api/doctors/remove-secretary/{id}` (Doctor role)
  - Removes the specified secretary.

### Patient Endpoints

- **POST** `/api/patients/recommend-doctors` (Patient role)
  - Body: `{ "symptoms": ["string"] }`
  - Returns: Recommended doctors based on symptoms.

- **GET** `/api/patients`
  - Returns: List of all patients.

- **GET** `/api/patients/{id}`
  - Returns: Patient details by ID.

- **GET** `/api/patients/search/{name}`
  - Returns: Patients matching the name.

- **PUT** `/api/patients/my-profile` (Patient role)
  - Body: UpdatePatientDto
  - Updates the patient's profile.

- **DELETE** `/api/patients/delete-my-profile` (Patient role)
  - Deletes the patient's profile.

- **GET** `/api/patients/my-medical-record` (Patient role)
  - Returns: Full medical record of the patient.

- **POST** `/api/patients/rate-doctor` (Patient role)
  - Body: RateDoctorDto
  - Rates and comments on a doctor.

## Contributing

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Push to the branch.
5. Open a Pull Request.

## License

This project is licensed under the MIT License.
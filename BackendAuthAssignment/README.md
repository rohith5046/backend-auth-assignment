# Backend Auth Assignment 🚀

This project is a **Backend Authentication System** built using **ASP.NET Core (.NET 8)** with **OTP-based login**, **JWT authentication**, **PostgreSQL**, and **Swagger UI**.

The implementation follows the trial assignment specifications and focuses on clean backend design, authentication flow, security basics, and documentation.

---

## 🛠 Tech Stack

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- PostgreSQL
- JWT (Access & Refresh Tokens)
- Swagger / OpenAPI

---

## 📁 Project Structure



BackendAuthAssignment/
│
├── Authorization/ # Custom authorization handlers & policies
├── Controllers/ # AuthController, UserController
├── Data/ # AppDbContext
├── Dtos/ # Request & Response DTOs
├── Models/ # User, UserProfile, Session, OtpRequest
├── Services/ # AuthService, OtpService, JwtTokenService
├── Migrations/ # EF Core migrations
├── Program.cs # Application startup & configuration
├── appsettings.json # Configuration
└── README.md # Documentation


---

## 🔐 Authentication Flow

### 1️⃣ Request OTP
**POST** `/auth/request-otp`

```json
{
  "phoneNumber": "9999999999"
}


✔️ OTP is generated and stored securely (hashed).

2️⃣ Verify OTP

POST /auth/verify-otp

{
  "phoneNumber": "9999999999",
  "otp": "123456"
}


✔️ On success, returns:

accessToken

refreshToken

isBasicRegistrationComplete

3️⃣ Refresh Access Token

POST /auth/refresh

{
  "refreshToken": "<refresh_token>"
}

4️⃣ Logout

POST /auth/logout

{
  "refreshToken": "<refresh_token>"
}

👤 User APIs
🔹 Basic Registration (Protected)

POST /user/register/basic

Authorization Header

Bearer <access_token>


Request Body

{
  "fullName": "Rohith Marupaka",
  "dateOfBirth": "2004-09-11",
  "email": "de@gmail.com",
  "location": {
    "city": "Hyderabad",
    "state": "Telangana",
    "country": "India"
  }
}


✔️ Marks basic registration as complete.

🔹 Get Current User (Protected + Policy)

GET /user/me

✔️ Requires:

Valid JWT token

Basic registration completed

🔒 Authorization & Policy

JWT authentication using Bearer tokens

Custom authorization policy: RegistrationComplete

/user/me endpoint is accessible only after basic registration is completed

⏱ OTP Rate Limiting

To prevent OTP abuse, rate limiting is implemented at the database level.

Limits

Hourly Limit: Max 3 OTP requests per phone number per hour

Daily Limit: Max 10 OTP requests per phone number in a rolling 24-hour window

Behavior

OTP requests beyond limits are blocked

Clear error messages are returned

Rate limit counters reset automatically based on time window

Each OTP request stores:

Phone number

Timestamp

Status (OK, BLOCKED_HOURLY, BLOCKED_DAILY)

📌 OTP Delivery (Mocked)

For this assignment, OTP delivery is mocked.

OTPs are not sent via SMS

Generated OTPs are logged to the application console/terminal

This avoids external SMS integrations while preserving OTP flow logic

➡️ During testing, check the server console output to retrieve the OTP.

🔑 Swagger Usage

Open Swagger UI

http://localhost:5176/swagger


Call /auth/verify-otp to get accessToken

Click Authorize (🔒)

Enter:

Bearer <access_token>


Test protected APIs

Note: Swagger auto-generates request templates.
For fields like location, the request body must be manually edited to provide a valid JSON object.

⚙️ Configuration

appsettings.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=backend_auth;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "backend-auth",
    "Audience": "backend-auth",
    "Key": "CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_KEY_32PLUS_CHARS",
    "AccessTokenMinutes": 15
  },
  "Auth": {
    "OtpValidityMinutes": 5,
    "OtpLength": 6,
    "OtpMaxAttempts": 3,
    "RefreshTokenDays": 7,
    "OtpHourlyLimit": 3,
    "OtpDailyLimit": 10
  }
}

🧪 Run Project
dotnet restore
dotnet ef database update
dotnet run


Swagger will be available at:

http://localhost:5176/swagger

✅ Features Covered

OTP-based authentication

JWT access & refresh tokens

OTP rate limiting (hourly & daily)

Secure token storage (hashed refresh tokens)

Custom authorization policy

Protected APIs

Swagger integration

PostgreSQL with EF Core

Clean & normalized database design

👨‍💻 Author

Rohith Marupaka
Backend Auth Assignment
ASP.NET Core | JWT | PostgreSQL
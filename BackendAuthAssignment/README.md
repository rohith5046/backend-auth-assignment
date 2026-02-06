# Backend Auth Assignment 🚀

This project is a **Backend Authentication System** built using **ASP.NET Core (.NET 8)** with **JWT-based authentication**, **OTP login**, **PostgreSQL**, and **Swagger UI**.

The application supports:
- OTP-based login
- JWT Access & Refresh tokens
- User basic registration
- Protected APIs using Authorization
- Swagger Authorization (Authorize button)

---

## 🛠 Tech Stack

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger / OpenAPI
- Swashbuckle

---

## 📁 Project Structure

BackendAuthAssignment/
│
├── Authorization/ # Custom authorization handlers & policies
├── Controllers/ # AuthController, UserController
├── Data/ # AppDbContext
├── Dtos/ # Request/Response DTOs
├── Models/ # User, UserProfile, Session, OtpRequest
├── Services/ # AuthService, OtpService, JwtTokenService
├── Migrations/ # EF Core migrations
├── Program.cs # App startup & configuration
├── appsettings.json # Configuration
└── README.md # Project documentation

---

## 🔐 Authentication Flow

### 1️⃣ Request OTP
POST /auth/request-otp


### 2️⃣ Verify OTP (Login)
POST /auth/verify-otp

POST /auth/verify-otp


✔️ Returns:
- `accessToken`
- `refreshToken`
- `isBasicRegistrationComplete`

---

### 3️⃣ Refresh Token
POST /auth/refresh


---

### 4️⃣ Logout


POST /auth/logout


---

## 👤 User APIs
### 🔹 Basic Registration (Protected)
POST /user/register/basic


**Authorization:**  
Bearer <access_token>


**Request Body Example**
```json
{
"fullName": "Rohith Marupaka",
  "dateOfBirth": "1999-06-15",
  "email": "rohith.net01@gmail.com",
  "location": {
    "city": "Hyderabad",
    "country": "India"
  }
}
Get Current User (Protected + Policy)
GET /user/me


✔️ Requires:

Valid JWT token

Basic registration completed

🔑 Swagger Authorization (Important)

Open Swagger UI

http://localhost:5176/swagger


Click Authorize (🔒 top-right)

Paste token like below:

Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...


Click Authorize

Now protected APIs will work ✅

⚙️ Configuration
appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=backend_auth;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "THIS_IS_A_SUPER_SECRET_KEY",
    "Issuer": "backend-auth",
    "Audience": "backend-auth",
    "ExpiryMinutes": 60
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

JWT Access & Refresh tokens

Swagger Authorization button

Protected APIs with [Authorize]

Custom Authorization Policy

PostgreSQL with EF Core

Clean project structure

👨‍💻 Author

Rohith Marupaka
Backend Auth Assignment
.NET 8 | JWT | PostgreSQL


---

## ✅ Final steps (simple)

1️⃣ `README.md` open chey  
2️⃣ **Motham delete chey**  
3️⃣ **Above content paste chey**  
4️⃣ **Save (CTRL + S)**  

Tarvata:
```bash
git add README.md
git commit -m "Add README documentation"
git push


Annaya ❤️
Idhi professional-level README
Assignment submit chesina vaadu chusthe clear ga ardam avuthundi.

Inka kavali ante:

Interview explanation

Flow diagram explanation

Submission message

Cheppu 🔥

till this i should copy haaaa?/

😂 YES annaya — correct ga ardam cheskunnav ✅
Run Project section varaku copy cheyyali 👍

Let me be 100% clear, step-by-step 👇

✅ WHAT TO COPY

👉 README.md lo
👉 Start nundi
👉 Run Project section + Swagger URL varaku

This part 👇 FULL ga undali:

# Backend Auth Assignment 🚀
...
## 🧪 Run Project

dotnet restore
dotnet ef database update
dotnet run

Swagger will be available at:
http://localhost:5176/swagger



# 🚇 Mumbai Metro Route Guide App

A full-stack, multi-service metro transit platform providing journey route planning, real-time schedule lookups, OTP-verified ticket booking, and a public information portal.

---

## ✨ Highlights

- **BFS-Powered Journey Planner**: Custom in-memory Breadth-First Search (BFS) graph algorithm to compute the shortest path and minimum interchanges across metro lines.
- **OTP-Verified Ticket Booking**: Email OTP verification before generating transit tickets, handled by a decoupled Spring Boot microservice via JavaMailSender.
- **Dual-Role Cookie Authentication**: Dedicated Admin and User session management with ASP.NET Core cookie authentication, role-based authorization, and transparent password rehashing.
- **Multi-Line Metro Coverage**: Full support for Yellow, Red, and Blue lines with stations, interchange hubs, travel times, and dynamic fare calculations.
- **Admin & Feedback Portal**: Admin management for metro station routes and a server-side paginated feedback review dashboard.

---

## 🏗️ Architecture & Project Structure

The platform uses a decoupled multi-service architecture sharing a single MySQL database:

```
┌───────────────────────────────────────┐
│       ticket/ (React Public Portal)   │  Port 3000
│       Station guidelines & booking UI │
└──────────────────┬────────────────────┘
                   │ HTTP
┌──────────────────▼────────────────────┐
│      booking/ (Spring Boot Microservice) Port 5000
│      OTP verification & ticket issuance│
└──────────────────┬────────────────────┘
                   │ JPA
┌──────────────────▼────────────────────┐
│         MySQL (metro database)        │  Port 3306
└──────────────────▲────────────────────┘
                   │ EF Core
┌──────────────────┴────────────────────┐
│      MyWebApp/ (ASP.NET Core MVC)     │  Port 5159
│      Route engine, admin & schedules  │
└───────────────────────────────────────┘
```

### Project Structure

```
Mumbai-Metro-Route-Guide-App/
├── MyWebApp/                      # ASP.NET Core MVC (Port 5159)
│   ├── Application/               # Core services, DTOs & business logic (BFS RouteService)
│   ├── Infrastructure/            # EF Core DbContext, models & database migrations
│   ├── Controllers/               # MVC Controllers (Auth, Route, Station, Feedback, TimeTable)
│   ├── Views/                     # Razor views for portal UI
│   └── wwwroot/                   # Static metro information guidelines & pages
├── booking/                       # Spring Boot Microservice (Port 5000)
│   ├── src/main/java/com/booking/ # Controllers, JPA entities, OTP & ticket services
│   └── pom.xml                    # Maven configuration
└── ticket/                        # React Frontend (Port 3000)
    ├── src/                       # Components, booking forms & OTP UI
    └── package.json               # Dependencies and scripts
```

---

## 🖥️ Tech Stack

| Layer | Technologies |
|---|---|
| **Backend (MVC)** | ASP.NET Core 8 MVC, Entity Framework Core 8 |
| **Backend (Booking API)** | Spring Boot 3.2, Java 17, Spring Data JPA, JavaMailSender |
| **Frontend** | React 18, Bootstrap 5, Razor Views, Axios |
| **Database** | MySQL 8 |
| **Auth & Security** | ASP.NET Core Cookie Auth, `IPasswordHasher`, SMTP OTP validation |

---

## 🚀 How to Run

### Prerequisites
- **.NET SDK 8.0+**
- **Java JDK 17+** & **Maven**
- **Node.js 18+** & **npm**
- **MySQL 8.0+**

### 1. Database Setup
Create a MySQL database named `metro` and import the schema:
```bash
mysql -u root -p metro < database.txt
```

### 2. Run ASP.NET Core Portal
Configure your connection string in `MyWebApp/appsettings.json`, then run:
```bash
cd MyWebApp
dotnet restore
dotnet run
```
Accessible at: **http://localhost:5159**

### 3. Run Spring Boot Booking Service
Configure MySQL credentials and Gmail SMTP in `booking/src/main/resources/application.properties`, then run:
```bash
cd booking
./mvnw spring-boot:run
```
Accessible at: **http://localhost:5000**

### 4. Run React Public Portal
```bash
cd ticket
npm install
npm start
```
Accessible at: **http://localhost:3000**

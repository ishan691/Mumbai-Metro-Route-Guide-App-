# 🚇 Mumbai Metro Route Guide App

> A full-stack, multi-service metro guide platform covering **route planning**, **live train schedules**, **online ticket booking**, **feedback management**, and a **public information portal** — built across three independent services with ASP.NET Core MVC, Spring Boot, and React.

---

## ✨ Highlights

- 🗺️ **BFS-powered Journey Planner** — Custom Breadth-First Search graph algorithm computes the shortest metro path across interchanges in real time
- 🎫 **OTP-Verified Ticket Booking** — Email OTP authentication before ticket generation, powered by a dedicated Spring Boot microservice
- 🔐 **Dual-role Cookie Auth** — Separate Admin and User login flows with ASP.NET Core cookie authentication and role-based `[Authorize]` guards
- 🔑 **Live Password Upgrade** — Legacy plain-text passwords are transparently rehashed with ASP.NET Core `IPasswordHasher` on first successful login
- 🚉 **3-Line Metro Coverage** — Yellow, Red, and Blue metro lines with full station data, interchange tracking, and fare/time lookup
- 📊 **Paginated Feedback Dashboard** — Admin can browse all user feedback in a server-side paginated view
- 🌐 **Public Info Portal** — Standalone React app serving metro guidelines, blog posts, station facilities, and carriage rules
- 🗃️ **Rich MySQL Schema** — Pre-populated `route_stations` table with stop count, travel time, fare, and interchange stops for every O-D pair

---

## 🖥️ Tech Stack

### Backend — Route Guide & Admin Portal
| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 (MySQL) |
| Database | MySQL 8 |
| Authentication | Cookie-based (ASP.NET Core Identity primitives) |
| Password Hashing | `Microsoft.AspNetCore.Identity.IPasswordHasher<T>` |
| Architecture | 3-layer: Application / Infrastructure / Presentation |

### Booking Microservice
| Layer | Technology |
|---|---|
| Framework | Spring Boot 3.2 (Java 17) |
| ORM | Spring Data JPA / Hibernate |
| Database | MySQL 8 (shared `metro` schema) |
| Email / OTP | Spring Boot Mail + Gmail SMTP |
| Build | Maven |

### Public Info Portal
| Layer | Technology |
|---|---|
| Framework | React 18 |
| UI | Bootstrap 5 + Font Awesome 6 |
| HTTP Client | Axios |
| Routing | React Router DOM v6 |

---

## 🏗️ Architecture

This project is split into **three independent services** that run concurrently and share a single MySQL database.

```
Mumbai-Metro-Route-Guide-App/
│
├── MyWebApp/              ← ASP.NET Core MVC  (port 5159)
│   Route planning, station management,
│   train schedules, feedback, admin panel
│
├── booking/               ← Spring Boot REST API  (port 5000)
│   OTP email verification + ticket booking
│
└── ticket/                ← React 18 App  (port 3000)
    Public metro info portal (blog, rules,
    facilities) + ticket purchase UI
```

```
┌──────────────────────────────────────────────────────────┐
│               ticket/ (React — Public Portal)            │
│   calls booking/ API for OTP + ticket generation         │
└──────────────────┬───────────────────────────────────────┘
                   │ HTTP  (localhost:5000)
┌──────────────────▼───────────────────────────────────────┐
│          booking/ (Spring Boot — Booking Service)         │
│   OTP generation → Email send → verify → /tickets/book   │
└──────────────────┬───────────────────────────────────────┘
                   │ JPA
┌──────────────────▼───────────────────────────────────────┐
│                  MySQL  (metro database)                  │
│  stations · route_lines · route_stations · time_table    │
│  users · admin · feedbacks · tickets                     │
└──────────────────▲───────────────────────────────────────┘
                   │ EF Core
┌──────────────────┴───────────────────────────────────────┐
│         MyWebApp/ (ASP.NET Core MVC — Main Portal)        │
│   Journey Planner · Admin Panel · Feedback · Schedules   │
└──────────────────────────────────────────────────────────┘
```

---

## 🗺️ Core Feature: BFS Journey Planner

The standout feature is the custom **graph-based shortest-path finder** in [`RouteService.cs`](MyWebApp/Application/Services/RouteService.cs).

**How it works:**
1. All stations are loaded from MySQL with their `PreviousStationId` and `NextStationId` links
2. An **adjacency list graph** is built in-memory from those links (bidirectional)
3. **Breadth-First Search (BFS)** finds the shortest hop-count path between any two stations — including cross-line interchange paths
4. Fare, travel time, stop count, and interchange stop count are fetched from the pre-computed `route_stations` lookup table
5. The ordered station list is returned to the view for step-by-step route rendering

```csharp
// RouteService.cs — BFS core
var queue = new Queue<uint>();
var visited = new HashSet<uint>();
var parent = new Dictionary<uint, uint?>();
// ... BFS traversal across all metro lines ...
```

---

## 🔐 Security & Authentication

### ASP.NET Core Portal
- **Cookie-based sessions** — `ClaimsPrincipal` with `ClaimTypes.Role` ("Admin" / "User") stored in an encrypted cookie
- **Role-based authorization** — `[Authorize(Roles = "Admin")]` on admin-only actions (Add Station, View Feedback)
- **Live password rehashing** — If a stored password doesn't look like a hash (no `AQAAAA` prefix), it's verified as plaintext and immediately upgraded to `IPasswordHasher` format on success
- **Duplicate checks** — Both username and email uniqueness are validated before registration

### Booking Service
- **6-digit OTP via Gmail SMTP** — Generated server-side, emailed, and verified before any ticket is issued
- OTP is held in controller instance memory per request (stateless session)

---

## 📁 Project Structure

```
Mumbai-Metro-Route-Guide-App/
│
├── MyWebApp/                              ← ASP.NET Core MVC
│   ├── MyWebApp.sln
│   ├── Program.cs                         ← DI wiring, EF, cookie auth, middleware
│   ├── appsettings.json                   ← MySQL connection string
│   │
│   ├── Application/                       ← Business Logic Layer
│   │   ├── DTOs/
│   │   │   ├── Auth/                      ← Login/Register request + result DTOs
│   │   │   ├── Feedback/                  ← Feedback create + list DTOs
│   │   │   ├── Route/                     ← JourneyPlannerResultDto, RouteStationDto
│   │   │   ├── Station/                   ← StationDto, StationCreateRequestDto
│   │   │   └── TimeTable/                 ← TimeTableDto, TrainScheduleResultDto
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IFeedbackService.cs
│   │   │   ├── IRouteService.cs
│   │   │   ├── IStationService.cs
│   │   │   └── ITimeTableService.cs
│   │   └── Services/
│   │       ├── AuthService.cs             ← Login, Register, password upgrade
│   │       ├── FeedbackService.cs         ← Submit + paginated list
│   │       ├── RouteService.cs            ← BFS graph path finder ★
│   │       ├── StationService.cs          ← All stations + add station
│   │       └── TimeTableService.cs        ← First/last train schedules
│   │
│   ├── Infrastructure/
│   │   └── Data/
│   │       ├── SiteDbContext.cs           ← EF Core DbContext (7 DbSets)
│   │       └── Entities/
│   │           ├── Admin.cs
│   │           ├── User.cs
│   │           ├── Station.cs
│   │           ├── RouteLine.cs
│   │           ├── RouteStation.cs        ← Fare, time, stops, interchange data
│   │           ├── TimeTable.cs           ← First/last train per O-D pair
│   │           └── Feedback.cs
│   │
│   ├── Controllers/
│   │   ├── AuthController.cs              ← Admin login, User login, Registration, Logout
│   │   ├── StationController.cs           ← View stations, Add station (Admin only)
│   │   ├── RouteController.cs             ← Journey Planner + Metro Between Stations
│   │   ├── FeedbackController.cs          ← Submit feedback, View feedback, Thank You
│   │   └── TimeTableController.cs         ← Train schedule display
│   │
│   ├── ViewModels/                        ← Strongly-typed view models per page
│   └── Views/Home/                        ← 14 Razor views (styled pages)
│
├── booking/                               ← Spring Boot 3.2 REST API
│   ├── pom.xml
│   └── src/main/java/com/booking/
│       ├── BookingApplication.java
│       ├── controller/
│       │   ├── EmailController.java        ← /send-test-email, /verify-otp
│       │   └── TicketController.java       ← POST /tickets/book, GET /tickets/{id}
│       ├── entity/
│       │   ├── Station.java / RouteLine.java / RouteStation.java
│       │   ├── Ticket.java
│       │   └── User.java
│       ├── repo/                           ← Spring Data JPA repositories
│       └── service/
│           ├── EmailService.java           ← JavaMailSender wrapper
│           ├── OTPService.java             ← 6-digit random OTP generate + validate
│           ├── TicketService.java          ← Fare lookup from route_stations → save Ticket
│           └── UserService.java
│
└── ticket/                                ← React 18 Public Portal
    └── src/
        ├── App.js                         ← Header + Data + Footer shell
        └── Componenets/
            ├── Header.js / Header.css     ← Navigation bar
            ├── Footer.js                  ← Footer links
            └── Data.js                    ← Booking form: OTP flow + ticket display
    wwwroot (in MyWebApp):
        ├── index.html                     ← Landing page
        ├── userInterface.html             ← Logged-in user dashboard
        ├── adminDashboard.html            ← Admin panel
        ├── blog.html                      ← Metro news & articles
        ├── about.html                     ← About Mumbai Metro
        ├── station-facilities.html        ← Accessibility & facilities guide
        ├── do-and-dont.htm                ← Metro etiquette rules
        ├── offences-penalties.htm         ← Fines & penalties reference
        └── ticket-carriage-rules.html     ← What you can/can't carry
```

---

## 🗃️ Database Schema

> All three services connect to the **same `metro` MySQL database**.

```sql
-- Users
users
  username        VARCHAR(50)  PK
  name            VARCHAR(200)
  email           VARCHAR(255) UNIQUE
  password        VARCHAR(255)          -- IPasswordHasher hash

-- Admin
admin
  username        VARCHAR(50)  PK
  password        VARCHAR(255)          -- IPasswordHasher hash

-- Metro Lines
route_lines
  route_id        INT  PK
  name            VARCHAR(100)          -- Yellow / Red / Blue

-- Stations
stations
  station_id      INT  PK
  station_name    VARCHAR(200)
  route_id        INT  FK → route_lines
  previous_station_id  INT  NULL
  next_station_id      INT  NULL

-- Pre-computed O-D Lookup (fare + time + stops)
route_stations
  id              INT  PK  AUTO_INCREMENT
  station_id_from INT  [INDEXED]
  station_id_to   INT  [INDEXED]
  stop            INT                   -- number of intermediate stops
  time            TIME                  -- travel time
  fare            DECIMAL               -- fare in ₹
  interchange_stops INT                 -- number of line changes

-- Train Schedule
time_table
  id              INT  PK  AUTO_INCREMENT
  station_id_from INT  [INDEXED]
  station_id_to   INT  [INDEXED]
  first_train     TIME
  last_train      TIME

-- Feedback
feedbacks
  Id              INT  PK  AUTO_INCREMENT
  EmailId         VARCHAR(255) [INDEXED]
  OverallRating   INT
  CleanlinessRating INT
  FacilitiesRating  INT
  AccessibilityRating INT
  Suggestions     VARCHAR(500)
  CreatedAt       DATETIME  [INDEXED]

-- Tickets (managed by Spring Boot)
tickets
  ticket_id       INT  PK  AUTO_INCREMENT
  station_id_from INT
  station_id_to   INT
  fare            DECIMAL
  time            TIME
```

---

## 🌐 Routes & Endpoints

### ASP.NET Core MVC — `MyWebApp/` (port 5159)
| Method | Route | Access | Description |
|---|---|---|---|
| GET/POST | `/Auth/AdminLogin` | Public | Admin login with cookie |
| GET/POST | `/Auth/UserLogin` | Public | User login with cookie |
| GET/POST | `/Auth/UserRegistration` | Public | New user registration |
| POST | `/Auth/Logout` | Auth | Sign out |
| GET | `/Route/JourneyPlanner` | Public | BFS route finder with fare & time |
| GET | `/Route/MetroBetweenStations` | Public | Inter-station metro info |
| GET | `/Station/ViewStation` | Public | Full station list |
| GET | `/Station/DisplayAllLineStations` | Public | Stations grouped by line |
| GET/POST | `/Station/AddStation` | **Admin** | Add new station |
| GET | `/TimeTable/TrainSchedule` | Public | First/last train schedule |
| GET/POST | `/Feedback/FeedbackForm` | Public | Submit feedback |
| GET | `/Feedback/ViewFeedback` | Public | Paginated feedback list |
| GET | `/Feedback/ThankYou` | Public | Post-submission thank you page |

### Spring Boot REST API — `booking/` (port 5000)
| Method | Endpoint | Description |
|---|---|---|
| GET | `/send-test-email?to={email}` | Generate OTP, send via Gmail SMTP |
| POST | `/verify-otp` | Validate entered OTP |
| POST | `/tickets/book` | Look up fare, save ticket, return ticket object |
| GET | `/tickets/{ticketId}` | Retrieve ticket by ID |

---

## 🎫 Ticket Booking Flow

```
User fills form (Route + Source + Destination + Email)
          │
          ▼
  React calls GET /send-test-email?to={email}
          │
          ▼
  Spring Boot generates 6-digit OTP
  JavaMailSender sends OTP to user's email via Gmail SMTP
          │
          ▼
  Browser prompt("Enter OTP:")  ← user types OTP
          │
          ▼
  React calls POST /verify-otp  { enteredOTP: "123456" }
          │
          ├─ Invalid → alert("Invalid OTP")
          │
          └─ Valid ──▶  POST /tickets/book
                              │
                              ▼
                    Lookup route_stations (fare, time)
                    Save Ticket to DB
                    Return Ticket JSON
                              │
                              ▼
                    Display Ticket ID + Fare + Time
```

---

## 🚀 Getting Started

### Prerequisites
| Tool | Version |
|---|---|
| .NET SDK | 8.0+ |
| Java JDK | 17+ |
| Maven | 3.8+ |
| Node.js | 18+ |
| MySQL | 8.0+ |

---

### 1. Set Up the Database

Import the schema and data:

```bash
mysql -u root -p < database.txt
```

> `database.txt` is excluded from the repository (see `.gitignore`).  
> Ask the project author for the data dump or recreate it from the schema above.

---

### 2. Run the ASP.NET Core Portal

Edit [`MyWebApp/appsettings.json`](MyWebApp/appsettings.json):

```json
{
  "ConnectionStrings": {
    "metro": "Server=localhost;Database=metro;User=root;Password=yourpassword;"
  }
}
```

> ⚠️ Note: The key in `appsettings.json` must match the name used in `Program.cs`:  
> `builder.Configuration.GetConnectionString("metro")`

```bash
cd MyWebApp
dotnet restore
dotnet run
```

- **App:** http://localhost:5159
- **Default Admin:** Set up via direct DB insert or use the admin credentials in `database.txt`

---

### 3. Run the Booking Microservice (Spring Boot)

Edit [`booking/src/main/resources/application.properties`](booking/src/main/resources/application.properties):

```properties
server.port=5000

spring.datasource.url=jdbc:mysql://localhost:3306/metro
spring.datasource.username=root
spring.datasource.password=yourpassword

spring.mail.username=your-gmail@gmail.com
spring.mail.password=your-app-password   # Gmail App Password (not your account password)
```

```bash
cd booking
./mvnw spring-boot:run
# or on Windows:
mvnw.cmd spring-boot:run
```

- **API:** http://localhost:5000

---

### 4. Run the React Public Portal

```bash
cd ticket
npm install
npm start
```

- **App:** http://localhost:3000

---

## 🔑 Default Credentials

| Role | Username | Password |
|---|---|---|
| Admin | Set in DB | Set in DB |
| User | Register via `/Auth/UserRegistration` | Set during registration |

---

## 📦 Key Packages

### NuGet (ASP.NET Core)
| Package | Purpose |
|---|---|
| `MySql.EntityFrameworkCore` 8.0.0 | MySQL ORM provider for EF Core |
| `Microsoft.AspNetCore.Authentication.Cookies` | Cookie-based session auth |
| `Microsoft.AspNetCore.Identity` | `IPasswordHasher<T>` password hashing |

### Maven (Spring Boot)
| Artifact | Purpose |
|---|---|
| `spring-boot-starter-data-jpa` | JPA / Hibernate ORM |
| `spring-boot-starter-web` | REST API + embedded Tomcat |
| `spring-boot-starter-mail` | JavaMailSender for Gmail SMTP |
| `mysql-connector-j` | MySQL JDBC driver |
| `spring-boot-devtools` | Hot reload in development |

### NPM (React)
| Package | Purpose |
|---|---|
| `react` / `react-dom` 18 | Core React library |
| `react-router-dom` v6 | Client-side routing |
| `axios` | HTTP client for booking API calls |
| `bootstrap` 5 | Responsive UI grid & components |
| `@fortawesome/react-fontawesome` | Icon set |

---

## 🧑‍💻 Key Engineering Decisions

| Decision | Rationale |
|---|---|
| BFS for route path | Metro network is an unweighted graph where each hop = 1 stop; BFS guarantees the minimum-stop path without the overhead of Dijkstra |
| Pre-computed `route_stations` table | Stores fare/time/interchange data for every O-D pair, avoiding expensive real-time graph weight computation |
| `IPasswordHasher<T>` over BCrypt | Native ASP.NET Core primitive; auto-detects legacy plain-text via prefix check and upgrades on login |
| Separate Spring Boot booking service | Isolates email/OTP logic and Java ecosystem libraries from the C# portal; each service can be scaled independently |
| Cookie auth over JWT for MVC | Server-rendered Razor views work naturally with cookie sessions; no need for token management in JavaScript |
| Soft `[AllowAnonymous]` on public controllers | Public pages (Journey Planner, Train Schedule) remain accessible without login while admin routes remain protected |
| DTOs separate from entities | Entities never leave the infrastructure layer; controllers and views always receive purpose-built DTOs / ViewModels |

---

## ⚠️ Known Issues & Improvement Notes

| # | Issue | Location | Suggestion |
|---|---|---|---|
| 1 | **OTP stored as instance field** — in a multi-user or load-balanced environment, `lastGeneratedOTP` in `EmailController` is shared state, causing race conditions | `booking/.../EmailController.java:29` | Replace with a `ConcurrentHashMap<email, otp>` or Redis with TTL |
| 2 | **Destination dropdown is incomplete** — only 4 options are active in the React booking form; options 5–43 are commented out | `ticket/src/Componenets/Data.js:184-222` | Either restore all options or fetch station list dynamically from the API |
| 3 | **SMTP credentials in source code** — Gmail username and App Password are committed in `application.properties` | `booking/.../application.properties:17-18` | Move to environment variables or Spring Cloud Config |
| 4 | **`appsettings.json` key mismatch** — file uses key `"MetroDb"` but `Program.cs` reads `"metro"` | `appsettings.json:10` vs `Program.cs:13` | Align to one consistent key name |
| 5 | **`Nullable` annotations disabled** — `<!-- <Nullable>enable</Nullable> -->` is commented out | `MyWebApp.csproj:7` | Enable to catch null-reference bugs at compile time |
| 6 | **No OTP expiry** — OTP has no time-to-live; a generated OTP remains valid indefinitely until the server restarts | `OTPService.java` | Add a timestamp and reject OTPs older than 5 minutes |

---

## 📂 Additional Pages (wwwroot)

The static HTML pages served by the ASP.NET Core app cover a full public metro guide:

| Page | Description |
|---|---|
| `index.html` | Landing page with metro overview |
| `userInterface.html` | Post-login user dashboard |
| `adminDashboard.html` | Admin control panel |
| `blog.html` | Metro news, updates, and articles |
| `about.html` | About Mumbai Metro project |
| `station-facilities.html` | Lifts, ramps, ticket counters, and amenities |
| `do-and-dont.htm` | Metro etiquette and conduct rules |
| `offences-penalties.htm` | Fine schedule for violations |
| `ticket-carriage-rules.html` | Allowed/prohibited items guide |

---

## 👤 Author

**Ishan Mansuri**

- GitHub: [@ishan691](https://github.com/ishan691)
- Email: ishan24mansuri@gmail.com

---

> *Built to demonstrate full-stack, multi-service architecture spanning ASP.NET Core, Spring Boot, and React — all integrated around a real-world Mumbai Metro dataset.*
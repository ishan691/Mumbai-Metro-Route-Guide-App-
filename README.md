# Mumbai Metro Project

Mumbai Metro is a multi-module metro management and ticket booking project built using ASP.NET Core MVC, Spring Boot, React, and MySQL.

## Project Modules

- **MyWebApp** - ASP.NET Core MVC application for metro management features
  - User and admin authentication
  - Station management
  - Journey planner
  - Metro route calculation
  - Train schedule
  - Feedback module

- **booking** - Spring Boot backend for ticket booking and OTP-related booking flow

- **ticket** - React frontend for ticket booking UI

## Tech Stack

- ASP.NET Core MVC
- C#
- Spring Boot / Java
- React.js
- MySQL
- Entity Framework Core

## Key Features

- Role-based login for admin and users
- Station and route management
- BFS-based shortest route calculation
- Journey planning between stations
- Train timetable display
- Feedback submission and viewing
- Ticket booking frontend and backend integration

## Folder Structure

```text
MumbaiMetro/
├── MyWebApp/      # ASP.NET Core MVC metro management app
├── booking/       # Spring Boot ticket booking backend
├── ticket/        # React ticket booking frontend
├── .config/
└── database.txt   # local database-related notes/config
```

## How to Run

### 1. ASP.NET Core app
Open the `MyWebApp` folder and run:

```bash
dotnet run
```

### 2. Spring Boot backend
Open the `booking` folder and run the Spring Boot application.

### 3. React frontend
Open the `ticket` folder and run:

```bash
npm install
npm start
```


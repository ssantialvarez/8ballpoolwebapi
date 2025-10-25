# Project Overview - Backend API

This project is the **Backend API** that supports the frontend application for scheduling and organizing 8 Ball Pool matches.
It is built using **.NET 8** and adheres to **Clean Architecture** principles, prioritizing separation of concerns, testability, and maintainability.
The API provides **RESTful endpoints** for all CRUD operations related to Users, Matches, and Venues.

---

## Technology Stack and Configuration

- **Framework**: **.NET 8 (ASP.NET Core Web API)**.
- **Language**: C# (version 12 features are allowed).
- **Database**: **PostgreSQL**.
- **ORM**: **Entity Framework Core (EF Core)** for database access and migrations.
- **Dependency Injection**: Use the **built-in .NET Core DI container**.
- **API Documentation**: Use **Swagger/OpenAPI** for endpoint documentation.
- **Authentication**: Bearer Token authentication (JWT) for secure access.

---

## Folder Structure and Architecture

The backend follows a layered architecture with the following main projects:

- **`[ProjectName].WebApi`**: **Presentation Layer**. Contains the **Controllers** (endpoints), DTOs (Request/Response Models), and configuration (e.g., authentication setup, CORS). **Controllers should be thin** and primarily delegate tasks to the Business layer.
- **`[ProjectName].Business`**: **Business/Application Layer**. Contains the core **business logic, services, interfaces**, and validation rules. It defines the use cases of the application.
- **`[ProjectName].Data`**: **Persistence/Infrastructure Layer**. Contains the **Repository implementations**, EF Core `DbContext`, database configuration, and data access logic. It implements interfaces defined in the Business layer.

---

## Coding Standards and Best Practices

- **Language Features**: Prefer **record types** for DTOs and immutable data models. Use **C# features** like expression-bodied members and pattern matching where it improves readability.
- **Naming Conventions**: Use **PascalCase** for classes, methods, properties, and namespaces. Use **camelCase** for local variables and method parameters.
- **Async/Await**: All I/O-bound operations (e.g., database calls) **must be asynchronous** using the `async/await` pattern. Methods should end with the `Async` suffix (e.g., `GetMatchesAsync`).
- **Dependency Injection**: Services and repositories must be registered and injected via their **interface** (e.g., inject `IMatchService`, not `MatchService`).
- **Error Handling**: Use **custom exception types** in the Business layer to clearly communicate business rule violations. Controllers should catch these and map them to appropriate HTTP status codes (e.g., `400 Bad Request`, `404 Not Found`).
- **Repositories**: Repositories in the `Data` layer should expose simple CRUD operations on entities and **should not contain business logic**.

---

## API Guidelines

- **RESTful Design**: Endpoints must adhere to REST principles (e.g., `GET /api/matches`, `POST /api/matches`, `PUT /api/matches/{id}`).
- **Input/Output**: Use dedicated **DTOs (Data Transfer Objects)** for all incoming request bodies and outgoing responses. **Never expose EF Core Entities directly** from the API.
- **Response Codes**: Return standard HTTP status codes: `200 OK`, `201 Created` (for POST), `204 No Content` (for successful DELETE or PUT without content), `400 Bad Request`, `401 Unauthorized`, `404 Not Found`, `500 Internal Server Error`.
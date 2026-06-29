# ContactListApp - Contact Management System

A web application for managing a contact list with categorization and full user authentication. The project is built using a client-server architecture.

## Tech Stack & Libraries Used
* **Backend:** .NET 8 / ASP.NET Core Web API
* **Database:** PostgreSQL (setup on a Docker container)
* **ORM & Database Provider:** Entity Framework Core (`Microsoft.EntityFrameworkCore.Design`, `Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Security & Auth:** JWT Bearer Authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`)
* **Frontend:** Vanilla JavaScript (ES6+), HTML5, Bootstrap 5

---

## Technical Documentation & Architecture

### 1. Classes and Methods Description (Architecture)
Backend follows a layered architecture separation using specialized classes:

* **Controllers (`ContactsListController`, `AuthController`):** Entry points for HTTP requests (REST API). They handle user registration/login and CRUD operations to the database (e.g., `GetContacts`, `PostContactItem`, `PutContactItem`). Methods modifying data are protected with `[Authorize]` filter.
* **Data Transfer Objects (`ContactDetailsDTO`, `CreateContactDTO`, etc.):** DTOs used for data serialization, validating incoming payloads using Data Annotations (e.g., `[Required]`, `[EmailAddress]`).
* **Models / Entities (`ContactItem`, `User`):** Database schema and relational mapping, translated into PostgreSQL tables by EF Core.
* **Frontend Client (`app.js`):** Asynchronous `fetch` wrappers for API communication, lightweight client-side JWT payload decoding to track the logged-in session, and dynamic DOM manipulation for view handling.

### 2. Authentication and Security
* Stateless **JWT (JSON Web Tokens)** authentication.
* User passwords securely hashed on the backend before being persisted to the database.
* Endpoints for creating, updating, and deleting contacts are protected using the `[Authorize]` attribute.
* Secured HTTPS communication.

### 3. Data Flow and Optimization
To optimize network transfer and reduce database load, the application uses an **On-Demand Fetching (Lazy Loading)** strategy:
* `GET /api/contacts` – Returns a lightweight list of `ContactsListItemDTO` objects (only essential data for the sidebar list).
* `GET /api/contacts/{id}` – Fetches the full detailed DTO only when a user clicks on a specific contact, dynamically decoding the owner's token to securely toggle edit permissions.

---

## Project Status & Technical Debt

While the core backend API is fully operational and secured, the frontend serves as a functional prototype focusing on the primary user flows. 

**Features prioritized for the nearest future:**
1. **Full Frontend Integration for PUT/DELETE:** The `PUT /api/contacts/{id}` and `DELETE /api/contacts/{id}` endpoints are fully implemented. The frontend needs some more work.
2. **Database-driven Dictionaries:** Moving static categories and subcategories from the frontend file into relational database tables and feeding them via dedicated dictionary endpoints.
3. **Automated Testing:** Unit and integration test for the backend services.

---

## Compilation & Running Instructions

### Prerequisites
* .NET 8 SDK
* Running PostgreSQL instance (e.g., via the configured Docker container)

### 1. Backend Compilation and Launch
You can compile and run the API using an IDE (Visual Studio / Rider), or via the .NET Core CLI.

Instruction for CLI:

**1. Navigate to the backend directory:**
```bash
cd ContactListApp/backend
```

**2. Restore dependencies and compile the application:**
```bash
dotnet build
```

**3. Run the Web API:**
```bash
dotnet run
```

The API will start and listen at **https://localhost:7035**

### 2. Frontend Launch
Open frontend/index.html file in a browser.



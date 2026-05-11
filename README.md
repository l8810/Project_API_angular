
# Chinese Auction System — Full-Stack Web Application

A full-stack web application for managing a **Chinese Auction (lottery-style gifting event)**.  
Users browse luxury gifts, purchase lottery tickets, and an admin panel handles donors, gifts, orders, and winner draws.

---

## Tech Stack

**Frontend**
- Angular 21 (with SSR — Server-Side Rendering)
- PrimeNG 21 · PrimeFlex · PrimeIcons
- TypeScript · RxJS
- JWT authentication (client-side)

**Backend**
- ASP.NET Core 8 Web API (C#)
- Entity Framework Core 9 + SQL Server
- JWT Bearer authentication
- Repository & Service pattern
- Serilog structured logging
- Swagger / OpenAPI

---

## Features

### User-facing
- Browse a catalog of luxury gifts (grid / list view, pagination)
- Filter by category, search by name, sort by donor
- Add gifts to cart with quantity selector
- View personal order history

### Admin Panel
- Dashboard with live statistics
- Manage donors (CRUD)
- Manage gifts with image upload
- View and manage all orders
- **Run lottery draws** — randomly select a winner per gift

### System
- Role-based access control (user / admin)
- JWT tokens with automatic expiry
- File upload service for gift images
- CORS configured for Angular dev server

---

## Project Structure

```
├── client/
│   └── client/                  # Angular 21 application
│       ├── src/app/
│       │   ├── components/
│       │   │   ├── admin/       # Dashboard, donors, gifts, orders
│       │   │   ├── auth/        # Login & register
│       │   │   ├── gifts/       # Gift catalog
│       │   │   ├── cart/        # Shopping cart
│       │   │   ├── home/        # Landing page
│       │   │   └── my-orders/   # Order history
│       │   ├── models/          # TypeScript interfaces
│       │   └── services/        # HTTP services (Angular)
│       └── package.json
│
└── server/
    └── StoreApi/
        └── StoreApi/            # ASP.NET Core 8 Web API
            ├── Controllers/     # REST API endpoints
            ├── Services/        # Business logic
            ├── Repositories/    # Data access layer
            ├── Models/          # EF Core entities
            ├── DTOs/            # Data transfer objects
            ├── Data/            # DbContext
            └── Migrations/      # EF Core migrations
```

---

## Getting Started

### Prerequisites
- Node.js 18+ and Angular CLI (`npm install -g @angular/cli`)
- .NET 8 SDK
- SQL Server or SQL Server LocalDB

### Backend Setup

```bash
cd server/StoreApi/StoreApi
```

Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LoteryDB;Trusted_Connection=True;"
}
```

Run migrations and start the API:
```bash
dotnet ef database update
dotnet run
```

API runs on `https://localhost:7xxx` · Swagger UI available at `/swagger`

### Frontend Setup

```bash
cd client/client
npm install
ng serve
```

App runs on `http://localhost:4200`

---

## API Endpoints

| Controller | Method | Route | Description |
|---|---|---|---|
| Auth | POST | `/api/auth/register` | Register new user |
| Auth | POST | `/api/auth/login` | Login & receive JWT |
| Gift | GET | `/api/gift` | Get all gifts |
| Gift | GET | `/api/gift/category/{id}` | Filter by category |
| Gift | POST | `/api/gift/{id}/lottery` | Draw winner (admin) |
| Cart | GET | `/api/cart` | Get user's cart |
| Cart | POST | `/api/cart` | Add item to cart |
| Order | GET | `/api/order` | Get all orders (admin) |
| Donor | GET | `/api/donor` | Get all donors |
| FileUpload | POST | `/api/fileupload` | Upload gift image |

---

## Architecture

```
Angular Client  ──(HTTP + JWT)──►  ASP.NET Core API
                                        │
                              ┌─────────┴──────────┐
                           Services          Repositories
                              │                    │
                         Business Logic      EF Core + SQL Server
```

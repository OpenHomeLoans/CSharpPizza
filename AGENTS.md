# AI Agent Instructions - CSharp Pizza Project

## Project Overview

This is a full-stack pizza e-commerce platform built with:
- **Backend**: ASP.NET Core 9.0 Web API with Entity Framework Core
- **Frontend**: React 19 with TypeScript, Vite, React Router, Zustand, TanStack Query
- **Database**: SQLite with Entity Framework Core migrations
- **Authentication**: JWT Bearer tokens

## Architecture

### Clean Architecture Layers

```
CSharpPizza.Api/          # Web API layer (Controllers, Middleware)
CSharpPizza.Domain/       # Business logic layer (Services)
CSharpPizza.Data/         # Data access layer (Entities, Repositories, DbContext)
CSharpPizza.DTO/          # Data Transfer Objects (DTOs, AutoMapper profiles)
CSharpPizza.WebUI/        # React frontend application
```

### Key Design Patterns

1. **Repository Pattern**: [`IRepository<T>`](CSharpPizza.Data/Repositories/IRepository.cs:1), [`Repository<T>`](CSharpPizza.Data/Repositories/Repository.cs:1)
2. **Service Layer Pattern**: All business logic in [`CSharpPizza.Domain/Services/`](CSharpPizza.Domain/Services/)
3. **DTO Pattern**: AutoMapper profiles in [`CSharpPizza.DTO/Mappings/`](CSharpPizza.DTO/Mappings/)
4. **Dependency Injection**: Configured in [`Program.cs`](CSharpPizza.Api/Program.cs:1)

## Core Entities

- [`User`](CSharpPizza.Data/Entities/User.cs:1) - User accounts with roles (Customer/Admin)
- [`Pizza`](CSharpPizza.Data/Entities/Pizza.cs:1) - Pizza products with base prices
- [`Topping`](CSharpPizza.Data/Entities/Topping.cs:1) - Available toppings with costs
- [`Cart`](CSharpPizza.Data/Entities/Cart.cs:1) / [`CartItem`](CSharpPizza.Data/Entities/CartItem.cs:1) - Shopping cart functionality
- [`Order`](CSharpPizza.Data/Entities/Order.cs:1) / [`OrderItem`](CSharpPizza.Data/Entities/OrderItem.cs:1) - Order management
- [`BaseEntity`](CSharpPizza.Data/Entities/BaseEntity.cs:1) - Base class with Id, timestamps, soft delete

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login (returns JWT token)

### Pizzas
- `GET /api/pizzas` - List all pizzas with toppings
- `GET /api/pizzas/{id}` - Get pizza details
- `POST /api/pizzas` - Create pizza (Admin only)
- `PUT /api/pizzas/{id}` - Update pizza (Admin only)
- `DELETE /api/pizzas/{id}` - Soft delete pizza (Admin only)

### Toppings
- `GET /api/toppings` - List all toppings
- `POST /api/toppings` - Create topping (Admin only)
- `PUT /api/toppings/{id}` - Update topping (Admin only)
- `DELETE /api/toppings/{id}` - Soft delete topping (Admin only)

### Cart
- `GET /api/cart` - Get user's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{id}` - Update cart item
- `DELETE /api/cart/items/{id}` - Remove cart item
- `DELETE /api/cart` - Clear cart

### Orders
- `GET /api/orders` - Get user's orders
- `GET /api/orders/{id}` - Get order details
- `POST /api/orders` - Create order from cart
- `PUT /api/orders/{id}/status` - Update order status (Admin only)

## Development Guidelines

### When Adding New Features

1. **Entity Changes**:
   - Add/modify entities in [`CSharpPizza.Data/Entities/`](CSharpPizza.Data/Entities/)
   - Inherit from [`BaseEntity`](CSharpPizza.Data/Entities/BaseEntity.cs:1) for standard fields
   - Update [`PizzaDbContext`](CSharpPizza.Data/PizzaDbContext.cs:1) with new DbSet
   - Create migration: `dotnet ef migrations add MigrationName --project CSharpPizza.Data --startup-project CSharpPizza.Api`

2. **Repository Layer**:
   - Create interface in [`CSharpPizza.Data/Repositories/`](CSharpPizza.Data/Repositories/)
   - Implement repository extending [`Repository<T>`](CSharpPizza.Data/Repositories/Repository.cs:1)
   - Register in [`Program.cs`](CSharpPizza.Api/Program.cs:28)

3. **Service Layer**:
   - Create interface and implementation in [`CSharpPizza.Domain/Services/`](CSharpPizza.Domain/Services/)
   - Inject required repositories
   - Register in [`Program.cs`](CSharpPizza.Api/Program.cs:33)

4. **DTOs**:
   - Create DTOs in appropriate folder under [`CSharpPizza.DTO/`](CSharpPizza.DTO/)
   - Create AutoMapper profile in [`CSharpPizza.DTO/Mappings/`](CSharpPizza.DTO/Mappings/)

5. **Controllers**:
   - Create controller in [`CSharpPizza.Api/Controllers/`](CSharpPizza.Api/Controllers/)
   - Use `[Authorize]` attribute for protected endpoints
   - Use `[Authorize(Roles = "Admin")]` for admin-only endpoints

### Frontend Development

1. **API Integration**:
   - API client configured in [`src/api/client.ts`](CSharpPizza.WebUI/src/api/client.ts:1)
   - API functions in [`src/api/`](CSharpPizza.WebUI/src/api/) directory
   - Use TanStack Query for data fetching

2. **State Management**:
   - Auth state: [`src/stores/authStore.ts`](CSharpPizza.WebUI/src/stores/authStore.ts:1)
   - Cart state: [`src/stores/cartStore.ts`](CSharpPizza.WebUI/src/stores/cartStore.ts:1)
   - Use Zustand for global state

3. **Routing**:
   - Routes defined in [`src/App.tsx`](CSharpPizza.WebUI/src/App.tsx:1)
   - Protected routes use [`ProtectedRoute`](CSharpPizza.WebUI/src/components/ProtectedRoute.tsx:1) component

4. **Components**:
   - Reusable components in [`src/components/`](CSharpPizza.WebUI/src/components/)
   - Page components in [`src/pages/`](CSharpPizza.WebUI/src/pages/)

## Running the Application

### Backend
```bash
cd CSharpPizza.Api
dotnet run
```
- API runs on: `https://localhost:7000` or `http://localhost:5000`
- Swagger UI: `https://localhost:7000/swagger`

### Frontend
```bash
cd CSharpPizza.WebUI
npm install
npm run dev
```
- Frontend runs on: `http://localhost:5173`

### Database
- SQLite database: [`CSharpPizza.Api/csharp-pizza.db`](CSharpPizza.Api/csharp-pizza.db:1)
- Migrations auto-run on startup
- Data seeding auto-runs on startup via [`DataSeeder`](CSharpPizza.Domain/Services/DataSeeder.cs:1)

## Common Tasks

### Creating a Migration
```bash
dotnet ef migrations add MigrationName --project CSharpPizza.Data --startup-project CSharpPizza.Api
```

### Applying Migrations
```bash
dotnet ef database update --project CSharpPizza.Data --startup-project CSharpPizza.Api
```

### Removing Last Migration
```bash
dotnet ef migrations remove --project CSharpPizza.Data --startup-project CSharpPizza.Api
```

## Code Conventions

### C# Backend
- Use async/await for all I/O operations
- Follow repository pattern for data access
- Use DTOs for all API responses
- Implement proper error handling with [`GlobalExceptionMiddleware`](CSharpPizza.Api/Middleware/GlobalExceptionMiddleware.cs:1)
- Use soft deletes (IsDeleted flag) instead of hard deletes
- All entities inherit from [`BaseEntity`](CSharpPizza.Data/Entities/BaseEntity.cs:1)

### TypeScript Frontend
- Use TypeScript strict mode
- Define types in [`src/types/index.ts`](CSharpPizza.WebUI/src/types/index.ts:1)
- Use React hooks and functional components
- Use TanStack Query for server state
- Use Zustand for client state
- Use React Router for navigation

## Authentication Flow

1. User registers/logs in via [`/api/auth/register`](CSharpPizza.Api/Controllers/AuthController.cs:1) or [`/api/auth/login`](CSharpPizza.Api/Controllers/AuthController.cs:1)
2. Backend returns JWT token in [`AuthResponseDto`](CSharpPizza.DTO/Auth/AuthResponseDto.cs:1)
3. Frontend stores token in [`authStore`](CSharpPizza.WebUI/src/stores/authStore.ts:1)
4. Token included in Authorization header for protected endpoints
5. Backend validates token via JWT Bearer authentication

## Important Notes

- **Soft Deletes**: All entities use `IsDeleted` flag - never hard delete
- **CORS**: Configured for `http://localhost:5173` in [`Program.cs`](CSharpPizza.Api/Program.cs:74)
- **JWT Secret**: Configured in `appsettings.json` (not committed to git)
- **Database**: SQLite file is gitignored - will be created on first run
- **Seeding**: Sample data automatically seeded on startup

## Error Handling

- Global exception middleware catches all unhandled exceptions
- Returns consistent error response format
- Logs errors for debugging
- Returns appropriate HTTP status codes

## Testing Endpoints

Use Swagger UI at `https://localhost:7000/swagger` or tools like:
- Postman
- Thunder Client (VS Code extension)
- curl
- [`CSharpPizza.Api.http`](CSharpPizza.Api/CSharpPizza.Api.http:1) file (VS Code REST Client)

## Security Considerations

- JWT tokens expire after configured time
- Passwords hashed with BCrypt
- Role-based authorization for admin endpoints
- CORS restricted to frontend origin
- HTTPS enforced in production
- SQL injection prevented by EF Core parameterization

## File Modification Guidelines

When modifying files:
1. Read related files first to understand context
2. Maintain existing patterns and conventions
3. Update DTOs and mappings when changing entities
4. Create migrations after entity changes
5. Update both interface and implementation
6. Test changes with both frontend and backend running
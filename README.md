# 🍕 CSharp Pizza - Full-Stack E-Commerce Platform

A modern pizza ordering platform built with ASP.NET Core 9.0 and React 19, featuring a clean architecture, JWT authentication, and real-time cart management.

## 🚀 Features

- **User Authentication**: Secure JWT-based authentication with role-based access control
- **Pizza Catalog**: Browse pizzas with customizable toppings
- **Shopping Cart**: Real-time cart management with topping customization
- **Order Management**: Place and track orders with status updates
- **Admin Panel**: Manage pizzas, toppings, and orders (Admin role required)
- **Responsive Design**: Mobile-friendly interface built with React

## 🏗️ Architecture

### Backend Stack
- **Framework**: ASP.NET Core 9.0 Web API
- **ORM**: Entity Framework Core
- **Database**: SQLite
- **Authentication**: JWT Bearer Tokens
- **Mapping**: AutoMapper
- **API Documentation**: Swagger/OpenAPI

### Frontend Stack
- **Framework**: React 19 with TypeScript
- **Build Tool**: Vite
- **Routing**: React Router v7
- **State Management**: Zustand
- **Data Fetching**: TanStack Query (React Query)
- **HTTP Client**: Axios
- **Notifications**: React Hot Toast

### Project Structure

```
CSharpPizza/
├── CSharpPizza.Api/          # Web API Controllers & Middleware
├── CSharpPizza.Domain/       # Business Logic & Services
├── CSharpPizza.Data/         # Data Access Layer & Entities
├── CSharpPizza.DTO/          # Data Transfer Objects
└── CSharpPizza.WebUI/        # React Frontend Application
```

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/) (with npm)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## 🛠️ Installation & Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd csharp-project
```

### 2. Backend Setup

```bash
# Navigate to API project
cd CSharpPizza.Api

# Restore dependencies
dotnet restore

# Run the application (migrations and seeding run automatically)
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7000`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:7000/swagger`

### 3. Frontend Setup

```bash
# Navigate to WebUI project
cd CSharpPizza.WebUI

# Install dependencies
npm install

# Start development server
npm run dev
```

The frontend will be available at: `http://localhost:5173`

## 🗄️ Database

The application uses SQLite with automatic migrations and data seeding:

- **Database File**: `CSharpPizza.Api/csharp-pizza.db`
- **Migrations**: Auto-applied on startup
- **Seeding**: Sample data (pizzas, toppings, admin user) auto-seeded on first run

### Default Admin Account

```
Email: admin@csharppizza.com
Password: Admin123!
```

### Sample Customer Account

```
Email: customer@example.com
Password: Customer123!
```

## 📚 API Documentation

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login user | No |

### Pizza Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/pizzas` | Get all pizzas | No |
| GET | `/api/pizzas/{id}` | Get pizza by ID | No |
| POST | `/api/pizzas` | Create pizza | Admin |
| PUT | `/api/pizzas/{id}` | Update pizza | Admin |
| DELETE | `/api/pizzas/{id}` | Delete pizza | Admin |

### Topping Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/toppings` | Get all toppings | No |
| POST | `/api/toppings` | Create topping | Admin |
| PUT | `/api/toppings/{id}` | Update topping | Admin |
| DELETE | `/api/toppings/{id}` | Delete topping | Admin |

### Cart Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/cart` | Get user's cart | Yes |
| POST | `/api/cart/items` | Add item to cart | Yes |
| PUT | `/api/cart/items/{id}` | Update cart item | Yes |
| DELETE | `/api/cart/items/{id}` | Remove cart item | Yes |
| DELETE | `/api/cart` | Clear cart | Yes |

### Order Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/orders` | Get user's orders | Yes |
| GET | `/api/orders/{id}` | Get order details | Yes |
| POST | `/api/orders` | Create order | Yes |
| PUT | `/api/orders/{id}/status` | Update order status | Admin |

## 🔧 Development

### Running Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName --project CSharpPizza.Data --startup-project CSharpPizza.Api

# Apply migrations
dotnet ef database update --project CSharpPizza.Data --startup-project CSharpPizza.Api

# Remove last migration
dotnet ef migrations remove --project CSharpPizza.Data --startup-project CSharpPizza.Api
```

### Building for Production

#### Backend
```bash
cd CSharpPizza.Api
dotnet publish -c Release -o ./publish
```

#### Frontend
```bash
cd CSharpPizza.WebUI
npm run build
```

## 🧪 Testing

### Using Swagger UI
1. Navigate to `https://localhost:7000/swagger`
2. Click "Authorize" and enter JWT token (get from login endpoint)
3. Test endpoints directly from the UI

### Using VS Code REST Client
Open `CSharpPizza.Api/CSharpPizza.Api.http` and use the REST Client extension to test endpoints.

## 🔐 Security Features

- **JWT Authentication**: Secure token-based authentication
- **Password Hashing**: BCrypt password hashing
- **Role-Based Authorization**: Admin and Customer roles
- **CORS Protection**: Restricted to frontend origin
- **HTTPS**: Enforced in production
- **SQL Injection Prevention**: EF Core parameterized queries
- **Soft Deletes**: Data preservation with IsDeleted flag

## 📁 Key Files

### Backend
- `CSharpPizza.Api/Program.cs` - Application configuration and DI setup
- `CSharpPizza.Data/PizzaDbContext.cs` - Database context
- `CSharpPizza.Domain/Services/` - Business logic services
- `CSharpPizza.Api/Controllers/` - API endpoints

### Frontend
- `CSharpPizza.WebUI/src/App.tsx` - Main app component and routing
- `CSharpPizza.WebUI/src/stores/` - Zustand state stores
- `CSharpPizza.WebUI/src/api/` - API client functions
- `CSharpPizza.WebUI/src/pages/` - Page components

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 Code Conventions

### C# Backend
- Use async/await for all I/O operations
- Follow repository pattern for data access
- Use DTOs for all API responses
- Implement proper error handling
- Use soft deletes (IsDeleted flag)
- All entities inherit from BaseEntity

### TypeScript Frontend
- Use TypeScript strict mode
- Use functional components with hooks
- Use TanStack Query for server state
- Use Zustand for client state
- Follow React best practices

## 🐛 Troubleshooting

### Database Issues
```bash
# Delete database and recreate
rm CSharpPizza.Api/csharp-pizza.db
dotnet run --project CSharpPizza.Api
```

### Port Conflicts
- Backend: Change ports in `CSharpPizza.Api/Properties/launchSettings.json`
- Frontend: Change port in `CSharpPizza.WebUI/vite.config.ts`

### CORS Issues
- Ensure frontend URL matches CORS configuration in `Program.cs`
- Default: `http://localhost:5173`

## 📄 License

This project is licensed under the MIT License.

## 👥 Authors

- Your Name - Initial work

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- React team for the amazing frontend library
- All open-source contributors

---

**Happy Coding! 🍕**
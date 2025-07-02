# Restaurant Booking System

A modern restaurant booking system built with .NET Aspire, ASP.NET Core Minimal API, and React TypeScript frontend.

## Project Structure

```
├── src/
│   ├── RestaurantBooking.Api/          # Minimal API project
│   ├── RestaurantBooking.Core/         # Domain models and business logic
│   ├── RestaurantBooking.Infrastructure/ # Data access and external services
│   ├── RestaurantBooking.AppHost/      # Aspire orchestration
│   ├── RestaurantBooking.ServiceDefaults/ # Shared service configuration
│   └── restaurant-booking-frontend/    # React TypeScript frontend
├── tests/
│   ├── RestaurantBooking.Api.Tests/
│   ├── RestaurantBooking.Core.Tests/
│   └── RestaurantBooking.Infrastructure.Tests/
└── architecture/
    └── Initial-Specification.md
```

## Technology Stack

### Backend
- **Framework**: ASP.NET Core (.NET 9) with Minimal APIs
- **Local Development**: .NET Aspire
- **Database**: PostgreSQL (configured for future use)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Logging**: Serilog
- **Testing**: xUnit, Moq

### Frontend
- **Framework**: React with TypeScript
- **State Management**: Redux Toolkit
- **Styling**: Tailwind CSS
- **Routing**: React Router
- **HTTP Client**: Axios
- **Testing**: Jest, React Testing Library

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- Docker (for Aspire)

### Running the Application

1. **Start the backend with Aspire:**
   ```bash
   cd src/RestaurantBooking.AppHost
   dotnet run
   ```
   This will start the API and Aspire dashboard.

2. **Start the React frontend (in a separate terminal):**
   ```bash
   cd src/restaurant-booking-frontend
   npm install
   npm start
   ```

### Building the Solution

```bash
# Build all projects
dotnet build

# Run all tests
dotnet test
```

### Frontend Development

```bash
cd src/restaurant-booking-frontend

# Install dependencies
npm install

# Start development server
npm start

# Build for production
npm run build

# Run tests
npm test
```

## Project Features

### Current Implementation
- ✅ Basic project structure with clean architecture
- ✅ .NET Aspire orchestration
- ✅ Minimal API with sample endpoint
- ✅ React frontend with Redux store
- ✅ Tailwind CSS styling
- ✅ API integration with Axios
- ✅ Test projects with xUnit and Moq
- ✅ CORS configuration
- ✅ JWT authentication setup (ready for implementation)

### Ready for Development
- 📋 Domain models (Core project)
- 📋 Repository pattern (Infrastructure project)
- 📋 Business logic services
- 📋 Restaurant management endpoints
- 📋 Booking management endpoints
- 📋 User authentication and authorization
- 📋 Database integration with PostgreSQL
- 📋 Frontend components and pages

## Development Guidelines

### Backend (TDD Approach)
1. Write tests first in the appropriate test project
2. Implement the minimal code to make tests pass
3. Refactor while keeping tests green
4. Follow clean architecture principles

### API Endpoints
The API includes:
- Health checks at `/health`
- OpenAPI documentation at `/openapi` (development)
- Sample weather endpoint at `/weatherforecast`

### Frontend Architecture
- Components in `src/components/`
- Services in `src/services/`
- Redux store in `src/store/`
- API integration via Axios client

## Docker Support

The frontend includes a multi-stage Dockerfile for production deployment with Nginx.

## Next Steps

1. Define domain models in the Core project
2. Implement repository pattern in Infrastructure
3. Add PostgreSQL database integration
4. Create restaurant and booking management endpoints
5. Implement user authentication
6. Build React components for booking workflow
7. Add comprehensive test coverage
8. Set up CI/CD pipeline

## Contributing

This project follows Test-Driven Development (TDD) principles. Please write tests before implementing features and ensure all tests pass before submitting changes.

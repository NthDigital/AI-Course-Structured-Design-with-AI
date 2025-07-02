# Epic 1: Project Setup and Infrastructure

## Overview
**Phase**: Foundation & Authentication (Sprint 1)  
**Duration**: 3 weeks  
**GitHub Issues**: 6 issues  
**Epic Goal**: Establish solid architectural foundation with Clean Architecture principles, Entity Framework Core infrastructure, dependency injection, JWT authentication, and database seeding strategies.

## GitHub Issues

### 1. Create Core Domain Entities with TDD

**User Story**: As a developer, I need to create all domain entities in the Core project following Clean Architecture principles and TDD practices so that the system has a solid foundation for business logic.

**Technical Requirements**:
- Create domain entities in `RestaurantBooking.Core/Entities/` folder
- Implement all entities with proper encapsulation and validation
- Add domain-specific validation rules and business logic
- Follow Clean Architecture principles (entities should not depend on external concerns)
- Use record types where appropriate for value objects

**TDD Approach**:
- Start by writing failing tests for each entity's behavior
- Test entity creation, validation, and business rules
- Test value object equality and immutability
- Test domain events if applicable

**Entities to Create**:
```csharp
// Core entities with their key properties and behaviors
- RestaurantOwner (validation: email format, required fields)
- Restaurant (validation: name length, cuisine type, status transitions)
- Table (validation: capacity > 0, unique table number per restaurant)
- Customer (validation: email format, phone format)
- Reservation (validation: future dates, party size, 3-hour duration)
- OperatingHours (validation: time ranges, day of week)
- AvailabilityBlock (validation: time ranges, date logic)
```

**Test Coverage Requirements**:
- Entity creation with valid data
- Entity validation with invalid data
- Business rule enforcement
- Edge cases and boundary conditions
- Domain events (if implemented)

**Acceptance Criteria**:
- [ ] All domain entities created in Core project
- [ ] Comprehensive unit tests with 100% coverage for entities
- [ ] All business validation rules implemented and tested
- [ ] Entities follow Clean Architecture principles (no external dependencies)
- [ ] Value objects are immutable and have proper equality
- [ ] Domain events are properly implemented (if applicable)
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Core/Entities/RestaurantOwner.cs
src/RestaurantBooking.Core/Entities/Restaurant.cs  
src/RestaurantBooking.Core/Entities/Table.cs
src/RestaurantBooking.Core/Entities/Customer.cs
src/RestaurantBooking.Core/Entities/Reservation.cs
src/RestaurantBooking.Core/Entities/OperatingHours.cs
src/RestaurantBooking.Core/Entities/AvailabilityBlock.cs
src/RestaurantBooking.Core/ValueObjects/
tests/RestaurantBooking.Core.Tests/Entities/
```

### 2. Implement Repository Interfaces and Domain Services with TDD

**User Story**: As a developer, I need to define repository interfaces and domain services in the Core project so that the business logic is decoupled from data access concerns.

**Technical Requirements**:
- Create repository interfaces in `RestaurantBooking.Core/Interfaces/`
- Implement domain services for complex business logic
- Define specification patterns for complex queries
- Create domain service interfaces for business operations
- Follow Clean Architecture dependency inversion principle

**TDD Approach**:
- Write tests using mock repositories to verify domain service behavior
- Test domain service business logic in isolation
- Test specification patterns for query building
- Test repository interface contracts with mock implementations

**Interfaces to Create**:
```csharp
- IRestaurantOwnerRepository
- IRestaurantRepository  
- ITableRepository
- ICustomerRepository
- IReservationRepository
- IOperatingHoursRepository
- IAvailabilityBlockRepository
- IUnitOfWork
- IPasswordHashingService
- IReservationValidationService
- ITableAssignmentService
- IAvailabilityService
```

**Domain Services to Create**:
```csharp
- ReservationValidationService (validate business rules)
- TableAssignmentService (find optimal table for reservation)
- AvailabilityService (check table availability)
- OperatingHoursService (validate operating hours)
```

**Acceptance Criteria**:
- [ ] All repository interfaces defined with proper methods
- [ ] Domain services implement complex business logic
- [ ] Specification patterns implemented for queries
- [ ] Unit tests cover all domain service behavior
- [ ] Mock repository tests verify business logic
- [ ] All interfaces follow Clean Architecture principles
- [ ] Domain services are properly tested in isolation
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Core/Interfaces/Repositories/
src/RestaurantBooking.Core/Interfaces/Services/
src/RestaurantBooking.Core/Services/
src/RestaurantBooking.Core/Specifications/
tests/RestaurantBooking.Core.Tests/Services/
```

### 3. Implement Entity Framework Core Infrastructure with TDD

**User Story**: As a developer, I need to implement Entity Framework Core infrastructure with proper configuration and migrations so that the application can persist data to PostgreSQL.

**Technical Requirements**:
- Create DbContext with proper entity configurations
- Implement repository pattern with EF Core
- Configure entity relationships and constraints
- Create initial database migration
- Implement proper connection string management
- Add database seeding for development data

**TDD Approach**:
- Use in-memory database for repository tests
- Test repository CRUD operations
- Test entity relationships and constraints
- Test transaction handling and unit of work
- Test database seeding functionality

**Infrastructure to Implement**:
```csharp
- RestaurantBookingDbContext
- Entity configurations (fluent API)
- Repository implementations
- UnitOfWork implementation
- Database seeding service
- Connection string configuration
```

**Database Configuration**:
- PostgreSQL connection setup
- Entity relationships (foreign keys, navigation properties)
- Indexes for performance
- Database constraints matching business rules
- Audit fields (CreatedAt, UpdatedAt)

**Acceptance Criteria**:
- [ ] DbContext properly configured with all entities
- [ ] Repository implementations with full CRUD operations
- [ ] Entity relationships properly configured
- [ ] Initial migration creates all tables with correct schema
- [ ] Database seeding works for development data
- [ ] Integration tests verify database operations
- [ ] Repository tests use in-memory database
- [ ] Transaction handling properly implemented
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Infrastructure/Data/RestaurantBookingDbContext.cs
src/RestaurantBooking.Infrastructure/Data/Configurations/
src/RestaurantBooking.Infrastructure/Repositories/
src/RestaurantBooking.Infrastructure/Services/
src/RestaurantBooking.Infrastructure/Migrations/
tests/RestaurantBooking.Infrastructure.Tests/
```

### 4. Configure Dependency Injection and Service Registration with TDD

**User Story**: As a developer, I need to configure dependency injection for all services and repositories so that the application follows Clean Architecture dependency inversion principles.

**Technical Requirements**:
- Configure DI container in API project
- Register all repositories and services
- Configure Entity Framework with PostgreSQL
- Set up service lifetimes (Singleton, Scoped, Transient)
- Configure options pattern for settings
- Add health checks for dependencies

**TDD Approach**:
- Test service registration and resolution
- Test service lifetimes are correct
- Test configuration binding works
- Integration tests to verify DI container setup
- Test health checks functionality

**Services to Register**:
```csharp
- DbContext (Scoped)
- Repositories (Scoped)
- Domain Services (Scoped)
- Infrastructure Services (Scoped)
- Configuration options
- Health checks
```

**Configuration Areas**:
- Database connection strings
- JWT authentication settings
- Application settings
- Logging configuration
- CORS policies

**Acceptance Criteria**:
- [ ] All services properly registered in DI container
- [ ] Service lifetimes correctly configured
- [ ] Configuration options properly bound
- [ ] Health checks implemented and working
- [ ] Integration tests verify DI container
- [ ] Application starts without DI errors
- [ ] All dependencies resolve correctly
- [ ] Tests verify service registration
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Api/Extensions/ServiceCollectionExtensions.cs
src/RestaurantBooking.Infrastructure/Extensions/ServiceCollectionExtensions.cs
src/RestaurantBooking.Api/Configuration/
tests/RestaurantBooking.Api.Tests/Extensions/
```

### 5. Implement JWT Authentication Infrastructure with TDD

**User Story**: As a developer, I need to implement JWT authentication infrastructure so that the application can securely authenticate restaurant owners and customers.

**Technical Requirements**:
- Configure JWT authentication in API project
- Implement password hashing service
- Create JWT token generation service
- Implement refresh token functionality
- Add authentication middleware
- Configure authorization policies for different user types

**TDD Approach**:
- Test JWT token generation and validation
- Test password hashing and verification
- Test authentication middleware behavior
- Test authorization policies
- Test token refresh functionality

**Authentication Components**:
```csharp
- IJwtTokenService
- IPasswordHashingService
- IRefreshTokenService
- AuthenticationMiddleware
- AuthorizationPolicies
- JWT configuration
```

**Security Features**:
- Password hashing with salt
- JWT token with proper expiration
- Refresh token rotation
- Role-based authorization
- Secure token validation

**Acceptance Criteria**:
- [ ] JWT authentication properly configured
- [ ] Password hashing service implemented and tested
- [ ] JWT token service generates valid tokens
- [ ] Refresh token mechanism works correctly
- [ ] Authorization policies enforce user roles
- [ ] Authentication middleware handles requests properly
- [ ] Security tests cover all authentication scenarios
- [ ] Integration tests verify end-to-end authentication
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Infrastructure/Services/JwtTokenService.cs
src/RestaurantBooking.Infrastructure/Services/PasswordHashingService.cs
src/RestaurantBooking.Infrastructure/Services/RefreshTokenService.cs
src/RestaurantBooking.Api/Middleware/AuthenticationMiddleware.cs
src/RestaurantBooking.Api/Authorization/
tests/RestaurantBooking.Infrastructure.Tests/Services/
```

### 6. Create Database Seeding and Migration Strategy with TDD

**User Story**: As a developer, I need to implement database seeding and migration strategies so that the application can be deployed with consistent data and schema updates.

**Technical Requirements**:
- Create database seeding service for development data
- Implement migration strategy for database updates
- Add sample data for testing
- Configure different seeding for different environments
- Create database initialization service

**TDD Approach**:
- Test seeding service creates expected data
- Test migration application
- Test data consistency after seeding
- Integration tests verify seeded data
- Test environment-specific seeding

**Seeding Components**:
```csharp
- IDatabaseSeeder
- DatabaseSeederService
- Sample data generators
- Environment-specific configurations
- Migration runner
```

**Sample Data to Create**:
- Sample restaurant owners
- Sample restaurants with tables
- Sample customers
- Sample operating hours
- Sample reservations (historical and future)

**Acceptance Criteria**:
- [ ] Database seeding service implemented
- [ ] Sample data created for all entities
- [ ] Environment-specific seeding works
- [ ] Migration strategy properly implemented
- [ ] Seeding tests verify data creation
- [ ] Integration tests use seeded data
- [ ] Database initialization works on startup
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Infrastructure/Data/Seeding/
src/RestaurantBooking.Infrastructure/Data/Migrations/
src/RestaurantBooking.Api/Extensions/DatabaseExtensions.cs
tests/RestaurantBooking.Infrastructure.Tests/Data/Seeding/
```

## Success Criteria

### Architecture Quality
- Clean Architecture principles properly implemented with clear layer separation
- Domain entities encapsulate business logic without external dependencies
- Dependency injection configured with proper service lifetimes
- Repository pattern abstracts data access concerns

### Testing Excellence
- All code developed using Red-Green-Refactor TDD cycle
- 100% test coverage for domain entities and business logic
- Integration tests verify infrastructure components
- Performance tests validate database operations

### Infrastructure Robustness
- Entity Framework Core properly configured with PostgreSQL
- Database migrations handle schema changes reliably
- JWT authentication infrastructure secure and performant
- Database seeding provides consistent development environments

### Quality Assurance
- All acceptance criteria met and verified
- Code review processes followed for all changes
- Documentation updated to reflect architectural decisions
- Performance benchmarks established for future comparison

This epic establishes the foundational architecture that all subsequent development will build upon, ensuring scalability, maintainability, and testability throughout the project lifecycle.

# Restaurant Booking System - Backend Specification

## Table of Contents
1. [Project Overview](#project-overview)
2. [System Requirements](#system-requirements)
3. [Domain Model](#domain-model)
4. [Database Design](#database-design)
5. [API Endpoints](#api-endpoints)
6. [Business Rules](#business-rules)
7. [Authentication & Authorization](#authentication--authorization)
8. [Error Handling](#error-handling)
9. [Development Task Breakdown](#development-task-breakdown)

## Project Overview

The Restaurant Booking System is a comprehensive platform that enables customers to discover and book tables at restaurants while providing restaurant owners with tools to manage their establishments, tables, and reservations.

### Key Features
- Multi-restaurant platform supporting multiple restaurant owners
- Real-time table availability management
- Customer registration and reservation management
- Restaurant owner dashboard for managing restaurants, tables, and operating hours
- Flexible availability blocking system for special events or maintenance
- Automated reservation conflict prevention
- 3-hour meal duration enforcement

### Technology Stack
- **Backend**: ASP.NET Core 9.0 (Minimal API)
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Architecture**: Clean Architecture (Core, Infrastructure, API layers)
- **Orchestration**: .NET Aspire for local development

## System Requirements

### Functional Requirements

#### Customer Management
- **FR-001**: Customers must register with email and basic information to make reservations
- **FR-002**: Customers can view their reservation history
- **FR-003**: Customers can cancel their own reservations
- **FR-004**: System must prevent duplicate customer registrations with the same email

#### Restaurant Management
- **FR-005**: Restaurant owners can register and manage their restaurant profiles
- **FR-006**: Restaurant owners can define operating hours for each day of the week
- **FR-007**: Restaurant owners can create availability blocks to prevent bookings during specific periods
- **FR-008**: Restaurant owners can view and manage all reservations for their restaurants

#### Table Management
- **FR-009**: Restaurant owners can define tables with specific capacities
- **FR-010**: Tables can be marked as available, occupied, or under maintenance
- **FR-011**: System must track table availability in real-time

#### Reservation Management
- **FR-012**: Reservations must be for future dates and times only
- **FR-013**: System must find available tables matching party size requirements
- **FR-014**: Each table booking blocks the table for 3 hours (meal duration)
- **FR-015**: Cancelled reservations immediately free up table availability
- **FR-016**: System must prevent double-booking of tables
- **FR-017**: Reservations must respect restaurant operating hours and availability blocks

### Non-Functional Requirements
- **NFR-001**: API response time < 500ms for 95% of requests
- **NFR-002**: Support for 1000+ concurrent users
- **NFR-003**: 99.9% uptime availability
- **NFR-004**: GDPR compliance for customer data
- **NFR-005**: Secure authentication and authorization

## Domain Model

### Core Entities

#### Restaurant Owner
- Represents individuals who own and manage restaurants
- Can own multiple restaurants
- Has authentication credentials and contact information

#### Restaurant
- Represents a dining establishment
- Belongs to a restaurant owner
- Has operational details, contact information, and status
- Contains multiple tables and operating schedules

#### Table
- Represents a physical table within a restaurant
- Has a specific capacity and operational status
- Can be reserved by customers

#### Customer
- Represents individuals who make reservations
- Has contact information and reservation history
- Must be registered to make bookings

#### Reservation
- Represents a booking made by a customer for a specific table
- Links customer, restaurant, and table with date/time information
- Has status tracking and special requirements

#### Operating Hours
- Defines when a restaurant is open for business
- Specified per day of the week
- Used to validate reservation times

#### Availability Block
- Represents periods when a restaurant is unavailable for bookings
- Used for special events, maintenance, or owner preferences
- Overrides normal operating hours

## Database Design

### Entity Relationship Diagram
```
Restaurant_Owners (1) -----> (N) Restaurants
Restaurants (1) -----> (N) Tables
Restaurants (1) -----> (N) Operating_Hours
Restaurants (1) -----> (N) Availability_Blocks
Restaurants (1) -----> (N) Reservations
Customers (1) -----> (N) Reservations
Tables (1) -----> (N) Reservations
```

### Table Specifications
### Table Specifications

```sql
-- 1. RESTAURANT_OWNERS
CREATE TABLE restaurant_owners (
    owner_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. RESTAURANTS
CREATE TABLE restaurants (
    restaurant_id SERIAL PRIMARY KEY,
    owner_id INTEGER NOT NULL,
    name VARCHAR(100) NOT NULL,
    cuisine_type VARCHAR(50),
    address TEXT,
    phone VARCHAR(20),
    email VARCHAR(100),
    description TEXT,
    status VARCHAR(20) CHECK (status IN ('active', 'inactive', 'pending')) DEFAULT 'pending',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (owner_id) REFERENCES restaurant_owners(owner_id) ON DELETE CASCADE
);

-- 3. TABLES
CREATE TABLE tables (
    table_id SERIAL PRIMARY KEY,
    restaurant_id INTEGER NOT NULL,
    table_number VARCHAR(10) NOT NULL,
    capacity INTEGER NOT NULL CHECK (capacity > 0),
    status VARCHAR(20) CHECK (status IN ('available', 'occupied', 'maintenance')) DEFAULT 'available',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (restaurant_id) REFERENCES restaurants(restaurant_id) ON DELETE CASCADE,
    UNIQUE(restaurant_id, table_number)
);

-- 4. OPERATING_HOURS
CREATE TABLE operating_hours (
    hours_id SERIAL PRIMARY KEY,
    restaurant_id INTEGER NOT NULL,
    day_of_week INTEGER CHECK (day_of_week BETWEEN 0 AND 6), -- 0=Sunday, 6=Saturday
    open_time TIME,
    close_time TIME,
    is_closed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (restaurant_id) REFERENCES restaurants(restaurant_id) ON DELETE CASCADE,
    UNIQUE(restaurant_id, day_of_week)
);

-- 5. CUSTOMERS
CREATE TABLE customers (
    customer_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20),
    password_hash VARCHAR(255) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 6. RESERVATIONS
CREATE TABLE reservations (
    reservation_id SERIAL PRIMARY KEY,
    restaurant_id INTEGER NOT NULL,
    customer_id INTEGER NOT NULL,
    table_id INTEGER,
    reservation_date DATE NOT NULL,
    reservation_time TIME NOT NULL,
    end_time TIME NOT NULL, -- Calculated as reservation_time + 3 hours
    party_size INTEGER NOT NULL CHECK (party_size > 0),
    status VARCHAR(20) CHECK (status IN ('pending', 'confirmed', 'cancelled', 'completed', 'no_show')) DEFAULT 'confirmed',
    special_requests TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (restaurant_id) REFERENCES restaurants(restaurant_id) ON DELETE CASCADE,
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id) ON DELETE CASCADE,
    FOREIGN KEY (table_id) REFERENCES tables(table_id) ON DELETE SET NULL
);

-- 7. AVAILABILITY_BLOCKS
CREATE TABLE availability_blocks (
    block_id SERIAL PRIMARY KEY,
    restaurant_id INTEGER NOT NULL,
    block_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    reason VARCHAR(100),
    is_recurring BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (restaurant_id) REFERENCES restaurants(restaurant_id) ON DELETE CASCADE,
    CHECK (end_time > start_time)
);

-- Indexes for performance
CREATE INDEX idx_restaurants_owner_id ON restaurants(owner_id);
CREATE INDEX idx_tables_restaurant_id ON tables(restaurant_id);
CREATE INDEX idx_operating_hours_restaurant_id ON operating_hours(restaurant_id);
CREATE INDEX idx_reservations_restaurant_id ON reservations(restaurant_id);
CREATE INDEX idx_reservations_customer_id ON reservations(customer_id);
CREATE INDEX idx_reservations_date_time ON reservations(reservation_date, reservation_time);
CREATE INDEX idx_reservations_table_date ON reservations(table_id, reservation_date);
CREATE INDEX idx_availability_blocks_restaurant_date ON availability_blocks(restaurant_id, block_date);
```

## API Endpoints

### Authentication Endpoints

#### POST /api/auth/restaurant-owner/register
Register a new restaurant owner account.

**Request Body:**
```json
{
  "firstName": "string",
  "lastName": "string", 
  "email": "string",
  "password": "string",
  "phone": "string"
}
```

**Response:** `201 Created`
```json
{
  "ownerId": "integer",
  "email": "string",
  "token": "string",
  "refreshToken": "string"
}
```

#### POST /api/auth/restaurant-owner/login
Authenticate a restaurant owner.

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Response:** `200 OK`
```json
{
  "ownerId": "integer",
  "email": "string",
  "token": "string",
  "refreshToken": "string"
}
```

#### POST /api/auth/customer/register
Register a new customer account.

**Request Body:**
```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "password": "string",
  "phone": "string"
}
```

**Response:** `201 Created`
```json
{
  "customerId": "integer",
  "email": "string",
  "token": "string",
  "refreshToken": "string"
}
```

#### POST /api/auth/customer/login
Authenticate a customer.

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Response:** `200 OK`
```json
{
  "customerId": "integer",
  "email": "string",
  "token": "string",
  "refreshToken": "string"
}
```

#### POST /api/auth/refresh
Refresh authentication token.

**Request Body:**
```json
{
  "refreshToken": "string"
}
```

**Response:** `200 OK`
```json
{
  "token": "string",
  "refreshToken": "string"
}
```

### Restaurant Management Endpoints

#### GET /api/restaurants
Get all active restaurants (public endpoint).

**Query Parameters:**
- `cuisine`: Filter by cuisine type
- `search`: Search by name or description
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10)

**Response:** `200 OK`
```json
{
  "restaurants": [
    {
      "restaurantId": "integer",
      "name": "string",
      "cuisineType": "string",
      "address": "string",
      "phone": "string",
      "email": "string",
      "description": "string",
      "status": "string"
    }
  ],
  "totalCount": "integer",
  "page": "integer",
  "pageSize": "integer"
}
```

#### GET /api/restaurants/{restaurantId}
Get restaurant details by ID.

**Response:** `200 OK`
```json
{
  "restaurantId": "integer",
  "name": "string",
  "cuisineType": "string",
  "address": "string",
  "phone": "string",
  "email": "string",
  "description": "string",
  "status": "string",
  "operatingHours": [
    {
      "dayOfWeek": "integer",
      "openTime": "time",
      "closeTime": "time",
      "isClosed": "boolean"
    }
  ]
}
```

#### POST /api/restaurants
Create a new restaurant (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "name": "string",
  "cuisineType": "string",
  "address": "string",
  "phone": "string",
  "email": "string",
  "description": "string"
}
```

**Response:** `201 Created`
```json
{
  "restaurantId": "integer",
  "name": "string",
  "status": "pending"
}
```

#### PUT /api/restaurants/{restaurantId}
Update restaurant details (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "name": "string",
  "cuisineType": "string",
  "address": "string",
  "phone": "string",
  "email": "string",
  "description": "string"
}
```

**Response:** `200 OK`

#### GET /api/restaurants/owner/me
Get restaurants owned by authenticated owner.

**Headers:** `Authorization: Bearer {token}`

**Response:** `200 OK`
```json
{
  "restaurants": [
    {
      "restaurantId": "integer",
      "name": "string",
      "status": "string",
      "totalTables": "integer",
      "totalReservations": "integer"
    }
  ]
}
```

### Table Management Endpoints

#### GET /api/restaurants/{restaurantId}/tables
Get all tables for a restaurant.

**Response:** `200 OK`
```json
{
  "tables": [
    {
      "tableId": "integer",
      "tableNumber": "string",
      "capacity": "integer",
      "status": "string"
    }
  ]
}
```

#### POST /api/restaurants/{restaurantId}/tables
Create a new table (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "tableNumber": "string",
  "capacity": "integer"
}
```

**Response:** `201 Created`
```json
{
  "tableId": "integer",
  "tableNumber": "string",
  "capacity": "integer",
  "status": "available"
}
```

#### PUT /api/restaurants/{restaurantId}/tables/{tableId}
Update table details (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "tableNumber": "string",
  "capacity": "integer",
  "status": "string"
}
```

**Response:** `200 OK`

#### DELETE /api/restaurants/{restaurantId}/tables/{tableId}
Delete a table (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Response:** `204 No Content`

### Operating Hours Endpoints

#### GET /api/restaurants/{restaurantId}/operating-hours
Get operating hours for a restaurant.

**Response:** `200 OK`
```json
{
  "operatingHours": [
    {
      "dayOfWeek": "integer",
      "openTime": "time",
      "closeTime": "time",
      "isClosed": "boolean"
    }
  ]
}
```

#### PUT /api/restaurants/{restaurantId}/operating-hours
Update operating hours (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "operatingHours": [
    {
      "dayOfWeek": "integer",
      "openTime": "time",
      "closeTime": "time",
      "isClosed": "boolean"
    }
  ]
}
```

**Response:** `200 OK`

### Availability Management Endpoints

#### GET /api/restaurants/{restaurantId}/availability
Check table availability for specific date and time.

**Query Parameters:**
- `date`: Date (YYYY-MM-DD)
- `time`: Time (HH:MM)
- `partySize`: Number of people

**Response:** `200 OK`
```json
{
  "isAvailable": "boolean",
  "availableTables": [
    {
      "tableId": "integer",
      "tableNumber": "string",
      "capacity": "integer"
    }
  ],
  "suggestedTimes": [
    {
      "time": "time",
      "availableTableCount": "integer"
    }
  ]
}
```

#### POST /api/restaurants/{restaurantId}/availability-blocks
Create an availability block (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "blockDate": "date",
  "startTime": "time",
  "endTime": "time",
  "reason": "string",
  "isRecurring": "boolean"
}
```

**Response:** `201 Created`

#### GET /api/restaurants/{restaurantId}/availability-blocks
Get availability blocks for a restaurant (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**
- `startDate`: Start date filter
- `endDate`: End date filter

**Response:** `200 OK`
```json
{
  "blocks": [
    {
      "blockId": "integer",
      "blockDate": "date",
      "startTime": "time",
      "endTime": "time",
      "reason": "string",
      "isRecurring": "boolean"
    }
  ]
}
```

#### DELETE /api/restaurants/{restaurantId}/availability-blocks/{blockId}
Delete an availability block (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Response:** `204 No Content`

### Reservation Endpoints

#### POST /api/reservations
Create a new reservation (customer only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "restaurantId": "integer",
  "reservationDate": "date",
  "reservationTime": "time",
  "partySize": "integer",
  "specialRequests": "string"
}
```

**Response:** `201 Created`
```json
{
  "reservationId": "integer",
  "restaurantName": "string",
  "tableNumber": "string",
  "reservationDate": "date",
  "reservationTime": "time",
  "endTime": "time",
  "partySize": "integer",
  "status": "confirmed"
}
```

#### GET /api/reservations/customer/me
Get customer's reservations.

**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**
- `status`: Filter by status
- `upcoming`: boolean (default: false)

**Response:** `200 OK`
```json
{
  "reservations": [
    {
      "reservationId": "integer",
      "restaurantName": "string",
      "restaurantAddress": "string",
      "tableNumber": "string",
      "reservationDate": "date",
      "reservationTime": "time",
      "endTime": "time",
      "partySize": "integer",
      "status": "string",
      "specialRequests": "string"
    }
  ]
}
```

#### GET /api/reservations/restaurant/{restaurantId}
Get restaurant's reservations (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**
- `date`: Filter by specific date
- `status`: Filter by status
- `page`: Page number
- `pageSize`: Items per page

**Response:** `200 OK`
```json
{
  "reservations": [
    {
      "reservationId": "integer",
      "customerName": "string",
      "customerPhone": "string",
      "tableNumber": "string",
      "reservationDate": "date",
      "reservationTime": "time",
      "endTime": "time",
      "partySize": "integer",
      "status": "string",
      "specialRequests": "string"
    }
  ],
  "totalCount": "integer"
}
```

#### PUT /api/reservations/{reservationId}/cancel
Cancel a reservation.

**Headers:** `Authorization: Bearer {token}`

**Response:** `200 OK`
```json
{
  "reservationId": "integer",
  "status": "cancelled",
  "cancelledAt": "timestamp"
}
```

#### PUT /api/reservations/{reservationId}/status
Update reservation status (restaurant owner only).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "status": "string"
}
```

**Response:** `200 OK`

## Business Rules

### Reservation Business Rules

1. **Future Reservations Only**
   - Reservations can only be made for future dates and times
   - Minimum advance booking: 30 minutes from current time

2. **Operating Hours Validation**
   - Reservations must be within restaurant operating hours
   - System checks day-of-week operating hours before confirming

3. **Table Capacity Matching**
   - System finds tables with capacity >= party size
   - Prefers smallest suitable table to optimize space utilization

4. **3-Hour Meal Duration**
   - Each reservation blocks the table for exactly 3 hours
   - `end_time` = `reservation_time` + 3 hours
   - No overlapping reservations allowed for the same table

5. **Availability Block Enforcement**
   - Reservations cannot be made during availability blocks
   - Blocks override normal operating hours

6. **Cancellation Rules**
   - Customers can cancel their own reservations
   - Restaurant owners can cancel any reservation at their restaurant
   - Cancelled reservations immediately free up table availability

### Authentication & Authorization Rules

1. **Customer Authorization**
   - Can view and cancel own reservations
   - Can view restaurant information and availability
   - Cannot access restaurant management functions

2. **Restaurant Owner Authorization**
   - Can manage only their own restaurants
   - Can view all reservations for their restaurants
   - Can modify table configurations and operating hours
   - Cannot access other owners' restaurant data

3. **Public Access**
   - Restaurant listing and details
   - Availability checking (without booking)
   - Operating hours information

## Error Handling

### Standard HTTP Status Codes

- `200 OK`: Successful retrieval
- `201 Created`: Successful creation
- `204 No Content`: Successful deletion
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `409 Conflict`: Business rule violation (e.g., double booking)
- `422 Unprocessable Entity`: Validation errors
- `500 Internal Server Error`: Server error

### Error Response Format

```json
{
  "error": {
    "code": "string",
    "message": "string",
    "details": "string",
    "timestamp": "ISO8601 datetime",
    "path": "string"
  }
}
```

### Common Error Codes

- `VALIDATION_ERROR`: Input validation failed
- `AUTHENTICATION_FAILED`: Invalid credentials
- `AUTHORIZATION_FAILED`: Insufficient permissions
- `RESOURCE_NOT_FOUND`: Requested resource doesn't exist
- `BUSINESS_RULE_VIOLATION`: Operation violates business rules
- `TABLE_NOT_AVAILABLE`: No suitable tables available
- `OUTSIDE_OPERATING_HOURS`: Reservation outside operating hours
- `DUPLICATE_EMAIL`: Email already registered
- `RESERVATION_CONFLICT`: Time slot already booked

## Development Task Breakdown

### Phase 1: Foundation & Authentication (Sprint 1)

#### Epic 1: Project Setup and Infrastructure
**GitHub Issues:**

1. **Setup Clean Architecture Project Structure** 
   - Create Core, Infrastructure, and API projects
   - Configure dependency injection
   - Setup Entity Framework Core with PostgreSQL
   - Configure Aspire orchestration
   - **Acceptance Criteria**: Project builds and runs with basic health checks

2. **Implement Database Schema and Migrations**
   - Create all entity models in Core project
   - Configure Entity Framework relationships
   - Create initial database migration
   - Seed development data
   - **Acceptance Criteria**: Database creates successfully with all tables and relationships

3. **Setup Authentication Infrastructure**
   - Configure JWT authentication
   - Implement password hashing
   - Create authentication middleware
   - Setup authorization policies
   - **Acceptance Criteria**: JWT tokens can be generated and validated

#### Epic 2: User Authentication System
**GitHub Issues:**

4. **Implement Restaurant Owner Registration**
   - Create registration endpoint
   - Implement email uniqueness validation
   - Add password strength requirements
   - Return JWT token on successful registration
   - **Acceptance Criteria**: Restaurant owners can register and receive authentication token

5. **Implement Restaurant Owner Login**
   - Create login endpoint
   - Validate credentials
   - Return JWT token and refresh token
   - **Acceptance Criteria**: Restaurant owners can authenticate with email/password

6. **Implement Customer Registration**
   - Create customer registration endpoint
   - Implement email uniqueness validation
   - Add password strength requirements
   - Return JWT token on successful registration
   - **Acceptance Criteria**: Customers can register and receive authentication token

7. **Implement Customer Login**
   - Create customer login endpoint
   - Validate credentials
   - Return JWT token and refresh token
   - **Acceptance Criteria**: Customers can authenticate with email/password

8. **Implement Token Refresh Mechanism**
   - Create refresh token endpoint
   - Implement token rotation security
   - Handle expired token scenarios
   - **Acceptance Criteria**: Users can refresh expired tokens without re-login

### Phase 2: Restaurant Management (Sprint 2)

#### Epic 3: Restaurant CRUD Operations
**GitHub Issues:**

9. **Implement Restaurant Creation**
   - Create restaurant creation endpoint
   - Validate restaurant owner authorization
   - Implement business validation rules
   - Set default status to 'pending'
   - **Acceptance Criteria**: Restaurant owners can create restaurants

10. **Implement Restaurant Listing and Details**
    - Create public restaurant listing endpoint
    - Add filtering by cuisine type
    - Implement search functionality
    - Add pagination support
    - **Acceptance Criteria**: Public users can browse restaurants

11. **Implement Restaurant Update**
    - Create restaurant update endpoint
    - Validate owner authorization
    - Implement partial update support
    - **Acceptance Criteria**: Restaurant owners can update their restaurant details

12. **Implement Owner Restaurant Dashboard**
    - Create endpoint to list owner's restaurants
    - Include summary statistics
    - Add restaurant status management
    - **Acceptance Criteria**: Restaurant owners can view their restaurant portfolio

#### Epic 4: Table Management System
**GitHub Issues:**

13. **Implement Table CRUD Operations**
    - Create table creation endpoint
    - Implement table listing for restaurant
    - Add table update and deletion
    - Validate table number uniqueness within restaurant
    - **Acceptance Criteria**: Restaurant owners can manage their tables

14. **Implement Table Status Management**
    - Add table status update endpoints
    - Implement status validation rules
    - Track table availability in real-time
    - **Acceptance Criteria**: Table status can be managed and tracked

### Phase 3: Operating Hours & Availability (Sprint 3)

#### Epic 5: Operating Hours Management
**GitHub Issues:**

15. **Implement Operating Hours Configuration**
    - Create operating hours CRUD endpoints
    - Support different hours per day of week
    - Handle closed days
    - Validate time format and logic
    - **Acceptance Criteria**: Restaurant owners can set operating hours

16. **Implement Operating Hours Validation Service**
    - Create service to check if time is within operating hours
    - Handle day-of-week calculations
    - Support time zone considerations
    - **Acceptance Criteria**: System validates reservations against operating hours

#### Epic 6: Availability Block System
**GitHub Issues:**

17. **Implement Availability Block Management**
    - Create availability block CRUD endpoints
    - Support one-time and recurring blocks
    - Validate time ranges
    - **Acceptance Criteria**: Restaurant owners can block specific time periods

18. **Implement Availability Checking Service**
    - Create service to check table availability
    - Consider operating hours and blocks
    - Account for existing reservations
    - Suggest alternative times
    - **Acceptance Criteria**: System accurately determines table availability

### Phase 4: Reservation System (Sprint 4)

#### Epic 7: Core Reservation Functionality
**GitHub Issues:**

19. **Implement Reservation Creation**
    - Create reservation booking endpoint
    - Implement table assignment algorithm
    - Validate availability and business rules
    - Send confirmation details
    - **Acceptance Criteria**: Customers can successfully book tables

20. **Implement Reservation Validation Service**
    - Validate future date/time requirements
    - Check table capacity vs party size
    - Prevent double booking
    - Enforce 3-hour meal duration
    - **Acceptance Criteria**: All business rules are enforced during booking

21. **Implement Customer Reservation Management**
    - Create customer reservation listing
    - Add reservation cancellation
    - Support filtering by status and date
    - **Acceptance Criteria**: Customers can view and cancel their reservations

22. **Implement Restaurant Reservation Dashboard**
    - Create restaurant reservation listing
    - Add date-based filtering
    - Include customer contact information
    - Support reservation status updates
    - **Acceptance Criteria**: Restaurant owners can manage all reservations

### Phase 5: Advanced Features & Optimization (Sprint 5)

#### Epic 8: Advanced Reservation Features
**GitHub Issues:**

23. **Implement Smart Table Assignment**
    - Optimize table selection algorithm
    - Prefer smallest suitable table
    - Consider table proximity and restaurant layout
    - **Acceptance Criteria**: System efficiently assigns optimal tables

24. **Implement Reservation Conflict Resolution**
    - Add comprehensive conflict detection
    - Handle edge cases in time overlaps
    - Provide clear error messages
    - **Acceptance Criteria**: Zero double-booking incidents

25. **Implement Advanced Availability Search**
    - Add "find next available" functionality
    - Suggest alternative times within date range
    - Support flexible party size matching
    - **Acceptance Criteria**: Users get helpful alternative suggestions

#### Epic 9: Performance & Polish
**GitHub Issues:**

26. **Implement Caching Strategy**
    - Cache restaurant listings
    - Cache operating hours
    - Implement cache invalidation
    - **Acceptance Criteria**: API response times under 500ms

27. **Add Comprehensive Error Handling**
    - Implement global exception handling
    - Add structured error responses
    - Include request correlation IDs
    - **Acceptance Criteria**: All errors return consistent, helpful responses

28. **Implement API Rate Limiting**
    - Add rate limiting middleware
    - Configure limits per endpoint type
    - Implement graceful degradation
    - **Acceptance Criteria**: API protected against abuse

29. **Add API Documentation and Testing**
    - Complete OpenAPI/Swagger documentation
    - Add comprehensive unit tests
    - Implement integration tests
    - Add performance tests
    - **Acceptance Criteria**: 90%+ test coverage and complete API docs

### Estimated Timeline
- **Sprint 1** (Foundation & Auth): 2 weeks
- **Sprint 2** (Restaurant Management): 2 weeks  
- **Sprint 3** (Operating Hours & Availability): 2 weeks
- **Sprint 4** (Reservation System): 2 weeks
- **Sprint 5** (Advanced Features): 2 weeks

**Total Estimated Duration**: 10 weeks

### Definition of Done
- [ ] Code implemented following Clean Architecture principles
- [ ] Unit tests written with 80%+ coverage
- [ ] Integration tests for API endpoints
- [ ] Code reviewed and approved
- [ ] API documentation updated
- [ ] Database migrations tested
- [ ] Security review completed
- [ ] Performance requirements met
- [ ] Deployed to development environment
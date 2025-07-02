# Epic 3: Restaurant CRUD Operations

## Overview
**Phase**: Restaurant Management (Sprint 4)  
**Duration**: 3 weeks  
**GitHub Issues**: 5 issues  
**Epic Goal**: Enable restaurant owners to create, manage, and update their restaurant information with comprehensive search capabilities for customers and a management dashboard for owners.

## GitHub Issues

### 13. Implement Restaurant Creation Endpoint with TDD

**User Story**: As a restaurant owner, I want to create and register my restaurant on the platform so that customers can find and make reservations at my establishment.

**Technical Requirements**:
- Create minimal API endpoint `POST /api/restaurants`
- Implement comprehensive restaurant data validation
- Ensure only authenticated restaurant owners can create restaurants
- Set default restaurant status to 'pending' for approval workflow
- Validate business information completeness and format
- Implement duplicate restaurant name detection within owner's portfolio
- Generate restaurant-specific configuration defaults
- Create audit trail for restaurant creation events

**TDD Approach**:
- Write failing integration tests for restaurant creation endpoint
- Test successful restaurant creation with valid data
- Test authorization requirements (only restaurant owners)
- Test validation failures with invalid/incomplete data
- Test duplicate name detection within owner's restaurants
- Test default status assignment and audit logging

**API Specification**:
```csharp
// Request DTO
public record CreateRestaurantRequest(
    string Name,
    string CuisineType,
    string Address,
    string? Phone,
    string? Email,
    string? Description
);

// Response DTO
public record CreateRestaurantResponse(
    int RestaurantId,
    string Name,
    string CuisineType,
    string Address,
    string Status,
    DateTime CreatedAt,
    string Message
);
```

**Acceptance Criteria**:
- [ ] POST /api/restaurants endpoint implemented with proper authorization
- [ ] Comprehensive input validation with detailed error messages
- [ ] Restaurant status automatically set to 'pending'
- [ ] Owner authorization enforced (cannot create for others)
- [ ] Duplicate restaurant name detection within owner portfolio
- [ ] Audit logging captures creation events
- [ ] All tests are green

### 14. Implement Public Restaurant Listing and Search with TDD

**User Story**: As a customer or visitor, I want to browse and search restaurants so that I can find establishments that match my preferences and location.

**Technical Requirements**:
- Create public API endpoint `GET /api/restaurants` (no authentication required)
- Implement comprehensive filtering (cuisine, location, status)
- Add full-text search functionality across name and description
- Implement pagination with configurable page sizes
- Add sorting options (name, cuisine, rating, distance)
- Include restaurant summary information and availability indicators
- Implement caching strategy for frequently accessed data
- Add location-based search capabilities

**Acceptance Criteria**:
- [ ] GET /api/restaurants endpoint publicly accessible
- [ ] Full-text search works across name and description
- [ ] Filtering by cuisine type, location, and status works
- [ ] Pagination with configurable page sizes implemented
- [ ] Sorting by multiple criteria supported
- [ ] Availability indicators show current restaurant status
- [ ] Response caching improves performance
- [ ] All tests are green

### 15. Implement Restaurant Details Endpoint with TDD

**User Story**: As a customer, I want to view detailed information about a specific restaurant so that I can make an informed decision about making a reservation.

**Technical Requirements**:
- Create public API endpoint `GET /api/restaurants/{restaurantId}`
- Return comprehensive restaurant information including operating hours
- Include table capacity information and availability summary
- Show restaurant owner contact information (if public)
- Implement view counting for analytics
- Add related restaurants suggestions
- Include restaurant images and media (placeholder for future)
- Handle non-existent or inactive restaurants appropriately

**Acceptance Criteria**:
- [ ] GET /api/restaurants/{id} endpoint publicly accessible
- [ ] Returns comprehensive restaurant information
- [ ] Includes operating hours for all days of week
- [ ] Shows table capacity summary information
- [ ] Handles non-existent restaurants with 404 response
- [ ] Only shows active restaurants to public users
- [ ] All tests are green

### 16. Implement Restaurant Update Endpoint with TDD

**User Story**: As a restaurant owner, I want to update my restaurant information so that customers see current and accurate details about my establishment.

**Technical Requirements**:
- Create authenticated API endpoint `PUT /api/restaurants/{restaurantId}`
- Implement partial update support (PATCH-like behavior)
- Ensure only restaurant owners can update their own restaurants
- Validate all updated information according to business rules
- Implement optimistic concurrency control to prevent conflicts
- Track change history for audit purposes
- Trigger cache invalidation when restaurant data changes
- Send notifications for significant changes (status, contact info)

**Acceptance Criteria**:
- [ ] PUT /api/restaurants/{id} endpoint requires authentication
- [ ] Only restaurant owners can update their own restaurants
- [ ] Partial updates work (only specified fields updated)
- [ ] All validation rules enforced on updated fields
- [ ] Optimistic concurrency control prevents data conflicts
- [ ] Audit trail tracks all changes with timestamps
- [ ] Cache invalidation occurs after successful updates
- [ ] All tests are green

### 17. Implement Restaurant Owner Dashboard Endpoint with TDD

**User Story**: As a restaurant owner, I want to view a dashboard of all my restaurants with key metrics so that I can monitor and manage my business effectively.

**Technical Requirements**:
- Create authenticated API endpoint `GET /api/restaurants/owner/dashboard`
- Return summary information for all owner's restaurants
- Include key performance metrics (reservations, capacity utilization)
- Show restaurant status and any pending actions required
- Provide quick access to management functions
- Include recent activity summary and alerts
- Implement efficient data aggregation for large portfolios
- Support filtering and sorting of owner's restaurants

**Acceptance Criteria**:
- [ ] GET /api/restaurants/owner/dashboard requires authentication
- [ ] Only shows data for authenticated owner's restaurants
- [ ] Returns accurate summary metrics across all restaurants
- [ ] Includes performance data (reservations, utilization)
- [ ] Shows restaurant status and pending actions
- [ ] Recent activity feed shows relevant events
- [ ] Alert system identifies actionable issues
- [ ] All tests are green

## Success Criteria

### Business Functionality
- Restaurant owners can successfully create and manage their restaurants
- Public can search and discover restaurants effectively
- Comprehensive restaurant information available for decision making
- Owner dashboard provides actionable business insights

### Technical Excellence
- All endpoints follow RESTful API design principles
- Comprehensive input validation prevents data corruption
- Caching strategies improve performance for public endpoints
- Authorization ensures proper access control throughout

### User Experience
- Search functionality helps customers find suitable restaurants
- Restaurant details provide comprehensive information for booking decisions
- Owner dashboard streamlines restaurant management tasks
- Performance optimizations ensure responsive user interactions

This epic enables the core restaurant management functionality that forms the foundation for the reservation system.

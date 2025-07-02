# Epic 2: User Authentication System

## Overview
**Phase**: Foundation & Authentication (Sprints 2-3)  
**Duration**: 5 weeks total (3 weeks Sprint 2, 2 weeks Sprint 3)  
**GitHub Issues**: 6 issues  
**Epic Goal**: Implement comprehensive user authentication system supporting both restaurant owners and customers with JWT tokens, refresh token rotation, secure password handling, and role-based authorization.

## GitHub Issues

### 7. Implement Restaurant Owner Registration Endpoint with TDD

**User Story**: As a restaurant owner, I want to register for an account so that I can manage my restaurants and reservations on the platform.

**Technical Requirements**:
- Create minimal API endpoint `POST /api/auth/restaurant-owner/register`
- Implement comprehensive input validation and sanitization
- Enforce password strength requirements (minimum 8 chars, uppercase, lowercase, number, special char)
- Check email uniqueness across both restaurant owners and customers
- Hash passwords using secure algorithm (BCrypt with salt)
- Generate JWT token with appropriate claims and expiration
- Return structured response with user data and authentication tokens
- Implement proper error handling and validation messages

**TDD Approach**:
- Start with failing integration tests for the endpoint
- Test successful registration with valid data
- Test validation failures with invalid data
- Test email uniqueness constraints
- Test password hashing functionality
- Test JWT token generation and structure
- Test error responses and status codes

**API Specification**:
```csharp
// Request DTO
public record RegisterRestaurantOwnerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone
);

// Response DTO
public record RegisterRestaurantOwnerResponse(
    int OwnerId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime TokenExpiry
);
```

**Validation Rules**:
```csharp
- FirstName: Required, 1-50 characters, letters only
- LastName: Required, 1-50 characters, letters only
- Email: Required, valid email format, unique across system
- Password: Required, min 8 chars, 1 uppercase, 1 lowercase, 1 number, 1 special
- Phone: Optional, valid phone format if provided
```

**Business Logic Tests**:
- Password complexity validation
- Email format validation and uniqueness
- Successful user creation in database
- Password hashing verification
- JWT token contains correct claims (user ID, email, role)
- Refresh token generation and storage

**Acceptance Criteria**:
- [ ] POST /api/auth/restaurant-owner/register endpoint implemented
- [ ] Comprehensive input validation with detailed error messages
- [ ] Password strength requirements enforced
- [ ] Email uniqueness validated across all user types
- [ ] Passwords securely hashed with BCrypt
- [ ] JWT tokens generated with proper claims and expiration
- [ ] Refresh tokens generated and stored securely
- [ ] Integration tests cover all success and failure scenarios
- [ ] Unit tests cover all validation and business logic
- [ ] Error handling returns appropriate HTTP status codes
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Api/Endpoints/AuthenticationEndpoints.cs
src/RestaurantBooking.Api/DTOs/Authentication/RegisterRestaurantOwnerRequest.cs
src/RestaurantBooking.Api/DTOs/Authentication/RegisterRestaurantOwnerResponse.cs
src/RestaurantBooking.Api/Validators/RegisterRestaurantOwnerValidator.cs
src/RestaurantBooking.Core/Services/AuthenticationService.cs
tests/RestaurantBooking.Api.Tests/Endpoints/AuthenticationEndpointsTests.cs
tests/RestaurantBooking.Core.Tests/Services/AuthenticationServiceTests.cs
```

### 8. Implement Restaurant Owner Login Endpoint with TDD

**User Story**: As a registered restaurant owner, I want to login to my account so that I can access my restaurant management dashboard.

**Technical Requirements**:
- Create minimal API endpoint `POST /api/auth/restaurant-owner/login`
- Validate email and password credentials
- Implement secure password verification against hashed passwords
- Generate new JWT access token and refresh token
- Implement account lockout after failed attempts (5 attempts, 15-minute lockout)
- Log authentication attempts for security monitoring
- Return user information and tokens on successful authentication
- Handle inactive/disabled accounts

**TDD Approach**:
- Write failing tests for successful login scenario
- Test invalid email/password combinations
- Test account lockout functionality
- Test inactive account handling
- Test JWT token generation and refresh token creation
- Test security logging functionality

**API Specification**:
```csharp
// Request DTO
public record LoginRestaurantOwnerRequest(
    string Email,
    string Password
);

// Response DTO
public record LoginRestaurantOwnerResponse(
    int OwnerId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime TokenExpiry
);
```

**Security Features**:
```csharp
- Rate limiting: Max 5 attempts per 15 minutes per IP
- Account lockout: 5 failed attempts locks account for 15 minutes
- Password verification using BCrypt
- Security event logging
- Timing attack prevention (constant-time comparison)
- Input sanitization and validation
```

**Business Logic Tests**:
- Successful login with valid credentials
- Failed login with invalid email
- Failed login with invalid password
- Account lockout after 5 failed attempts
- Account lockout expiration handling
- Inactive account rejection
- JWT token generation with correct claims
- Refresh token generation and storage

**Acceptance Criteria**:
- [ ] POST /api/auth/restaurant-owner/login endpoint implemented
- [ ] Secure password verification using BCrypt
- [ ] Account lockout implemented after 5 failed attempts
- [ ] JWT and refresh tokens generated on successful login
- [ ] Inactive accounts properly rejected
- [ ] Security events logged for monitoring
- [ ] Rate limiting implemented per IP address
- [ ] Integration tests cover all authentication scenarios
- [ ] Unit tests cover all security and business logic
- [ ] Timing attack prevention implemented
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Api/DTOs/Authentication/LoginRestaurantOwnerRequest.cs
src/RestaurantBooking.Api/DTOs/Authentication/LoginRestaurantOwnerResponse.cs
src/RestaurantBooking.Api/Validators/LoginRestaurantOwnerValidator.cs
src/RestaurantBooking.Core/Services/AccountLockoutService.cs
src/RestaurantBooking.Infrastructure/Services/SecurityEventLogger.cs
tests/RestaurantBooking.Api.Tests/Endpoints/RestaurantOwnerLoginTests.cs
tests/RestaurantBooking.Core.Tests/Services/AccountLockoutServiceTests.cs
```

### 9. Implement Customer Registration Endpoint with TDD

**User Story**: As a potential diner, I want to create an account so that I can make restaurant reservations and manage my bookings.

**Technical Requirements**:
- Create minimal API endpoint `POST /api/auth/customer/register`
- Implement identical validation rules as restaurant owner registration
- Ensure email uniqueness across both customer and restaurant owner tables
- Generate customer-specific JWT tokens with appropriate role claims
- Implement customer-specific business rules and validations
- Store customer data with proper encryption for sensitive fields
- Generate welcome email data for future email service integration

**TDD Approach**:
- Write failing tests for customer registration endpoint
- Test email uniqueness across all user types
- Test customer-specific validation rules
- Test JWT token generation with customer role
- Test database storage of customer data
- Test conflict resolution between customer and owner emails

**API Specification**:
```csharp
// Request DTO
public record RegisterCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone
);

// Response DTO
public record RegisterCustomerResponse(
    int CustomerId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime TokenExpiry
);
```

**Customer-Specific Features**:
```csharp
- Role claim: "Customer" in JWT token
- Customer preference tracking capability
- Reservation history initialization
- Different authorization policies than restaurant owners
- Customer-specific validation rules if needed
```

**Business Logic Tests**:
- Successful customer registration
- Email uniqueness validation against both tables
- Customer role assignment in JWT token
- Customer data storage verification
- Password hashing and security
- Customer-specific authorization claims

**Acceptance Criteria**:
- [ ] POST /api/auth/customer/register endpoint implemented
- [ ] Email uniqueness enforced across all user types
- [ ] Customer role properly assigned in JWT tokens
- [ ] Customer data securely stored in database
- [ ] Validation rules identical to restaurant owner registration
- [ ] Integration tests verify customer-specific functionality
- [ ] Unit tests cover customer business logic
- [ ] Authorization policies support customer role
- [ ] Cross-user-type email validation working
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Api/DTOs/Authentication/RegisterCustomerRequest.cs
src/RestaurantBooking.Api/DTOs/Authentication/RegisterCustomerResponse.cs
src/RestaurantBooking.Api/Validators/RegisterCustomerValidator.cs
src/RestaurantBooking.Core/Services/CustomerService.cs
tests/RestaurantBooking.Api.Tests/Endpoints/CustomerRegistrationTests.cs
tests/RestaurantBooking.Core.Tests/Services/CustomerServiceTests.cs
```

### 10. Implement Customer Login Endpoint with TDD

**User Story**: As a registered customer, I want to login to my account so that I can make reservations and manage my bookings.

**Technical Requirements**:
- Create minimal API endpoint `POST /api/auth/customer/login`
- Implement identical security features as restaurant owner login
- Generate customer-specific JWT tokens with customer role claims
- Apply same account lockout and security policies
- Implement customer-specific authorization policies
- Log customer authentication events separately for analytics

**TDD Approach**:
- Write failing tests for customer login scenarios
- Test customer-specific JWT token generation
- Test customer role authorization
- Test security features (lockout, rate limiting)
- Test customer authentication event logging

**API Specification**:
```csharp
// Request DTO (same as restaurant owner)
public record LoginCustomerRequest(
    string Email,
    string Password
);

// Response DTO
public record LoginCustomerResponse(
    int CustomerId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime TokenExpiry
);
```

**Customer-Specific Security**:
```csharp
- Same security policies as restaurant owners
- Customer role in JWT claims
- Customer-specific rate limiting if needed
- Customer authentication event logging
- Customer authorization policy validation
```

**Business Logic Tests**:
- Successful customer login
- Customer JWT token contains correct role
- Customer account lockout functionality
- Customer-specific authorization claims
- Security event logging for customers

**Acceptance Criteria**:
- [ ] POST /api/auth/customer/login endpoint implemented
- [ ] Customer role properly assigned in JWT tokens
- [ ] Same security features as restaurant owner login
- [ ] Customer-specific authorization policies enforced
- [ ] Customer authentication events logged separately
- [ ] Integration tests verify customer login functionality
- [ ] Unit tests cover customer authentication logic
- [ ] Account lockout works for customer accounts
- [ ] Rate limiting applied to customer logins
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Api/DTOs/Authentication/LoginCustomerRequest.cs
src/RestaurantBooking.Api/DTOs/Authentication/LoginCustomerResponse.cs
src/RestaurantBooking.Api/Validators/LoginCustomerValidator.cs
tests/RestaurantBooking.Api.Tests/Endpoints/CustomerLoginTests.cs
```

### 11. Implement Refresh Token Mechanism with TDD

**User Story**: As a user (restaurant owner or customer), I want my session to be automatically renewed so that I don't have to constantly re-login while using the application.

**Technical Requirements**:
- Create minimal API endpoint `POST /api/auth/refresh`
- Implement secure refresh token validation and rotation
- Generate new access token and refresh token pair
- Invalidate old refresh token immediately after use
- Implement refresh token expiration (30 days)
- Handle concurrent refresh token usage detection
- Support both customer and restaurant owner token refresh
- Implement refresh token revocation functionality

**TDD Approach**:
- Write failing tests for successful token refresh
- Test refresh token validation and rotation
- Test expired refresh token handling
- Test concurrent usage detection
- Test token revocation functionality
- Test user type preservation in new tokens

**API Specification**:
```csharp
// Request DTO
public record RefreshTokenRequest(
    string RefreshToken
);

// Response DTO
public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime TokenExpiry,
    string UserType // "Customer" or "RestaurantOwner"
);
```

**Security Features**:
```csharp
- Refresh token rotation (new token issued, old token invalidated)
- Refresh token expiration (30-day sliding window)
- Concurrent usage detection (if refresh token used twice, revoke all)
- Secure token storage and validation
- User context preservation (customer vs restaurant owner)
- Token family tracking for security
```

**Business Logic Tests**:
- Successful token refresh with valid refresh token
- Refresh token rotation (old token becomes invalid)
- Expired refresh token rejection
- Invalid refresh token rejection
- Concurrent usage detection and token family revocation
- User type preservation in new access token
- Token expiration time calculation

**Acceptance Criteria**:
- [ ] POST /api/auth/refresh endpoint implemented
- [ ] Refresh token rotation implemented securely
- [ ] Expired refresh tokens properly rejected
- [ ] Concurrent usage detection prevents token theft
- [ ] User type (customer/owner) preserved in new tokens
- [ ] Token family revocation on security violations
- [ ] Integration tests cover all refresh scenarios
- [ ] Unit tests cover token validation and generation logic
- [ ] Security tests verify token rotation and revocation
- [ ] Performance tests ensure refresh endpoint speed
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Api/DTOs/Authentication/RefreshTokenRequest.cs
src/RestaurantBooking.Api/DTOs/Authentication/RefreshTokenResponse.cs
src/RestaurantBooking.Core/Services/RefreshTokenService.cs
src/RestaurantBooking.Core/Entities/RefreshToken.cs
src/RestaurantBooking.Infrastructure/Repositories/RefreshTokenRepository.cs
tests/RestaurantBooking.Api.Tests/Endpoints/RefreshTokenTests.cs
tests/RestaurantBooking.Core.Tests/Services/RefreshTokenServiceTests.cs
```

### 12. Implement Authentication Middleware and Authorization Policies with TDD

**User Story**: As a developer, I need authentication middleware and authorization policies so that API endpoints are properly secured and users can only access resources they're authorized to use.

**Technical Requirements**:
- Create JWT authentication middleware for request processing
- Implement role-based authorization policies (Customer, RestaurantOwner)
- Create resource-based authorization for restaurant owner resources
- Implement custom authorization handlers for complex scenarios
- Add authentication state management in HTTP context
- Create authorization attributes for easy endpoint protection
- Implement proper error responses for authentication/authorization failures

**TDD Approach**:
- Write tests for middleware JWT token validation
- Test authorization policy enforcement
- Test resource-based authorization logic
- Test authentication failure scenarios
- Test authorization error responses

**Authorization Policies**:
```csharp
// Policy definitions
- "CustomerOnly": Requires Customer role
- "RestaurantOwnerOnly": Requires RestaurantOwner role
- "RestaurantOwner": Resource-based auth for restaurant ownership
- "AuthenticatedUser": Any authenticated user
```

**Middleware Features**:
```csharp
- JWT token validation from Authorization header
- User context establishment in HttpContext
- Automatic token expiration handling
- Authentication challenge responses
- Request/response logging for security events
```

**Custom Authorization Handlers**:
```csharp
- RestaurantOwnershipHandler: Verify restaurant ownership
- ReservationOwnershipHandler: Verify reservation ownership
- TableManagementHandler: Verify table management rights
```

**Business Logic Tests**:
- Valid JWT token authentication
- Invalid/expired JWT token rejection
- Role-based policy enforcement
- Resource ownership validation
- Authentication challenge responses
- Authorization error handling

**Acceptance Criteria**:
- [ ] JWT authentication middleware properly validates tokens
- [ ] Role-based authorization policies enforce access control
- [ ] Resource-based authorization validates ownership
- [ ] Authentication failures return appropriate HTTP responses
- [ ] Authorization failures return detailed error information
- [ ] Custom authorization handlers work for complex scenarios
- [ ] Integration tests verify end-to-end authorization
- [ ] Unit tests cover all middleware and policy logic
- [ ] Security tests verify proper access control
- [ ] Performance tests ensure minimal overhead
- [ ] All tests are green

**Files to Create**:
```
src/RestaurantBooking.Api/Middleware/JwtAuthenticationMiddleware.cs
src/RestaurantBooking.Api/Authorization/Policies/AuthorizationPolicies.cs
src/RestaurantBooking.Api/Authorization/Handlers/RestaurantOwnershipHandler.cs
src/RestaurantBooking.Api/Authorization/Handlers/ReservationOwnershipHandler.cs
src/RestaurantBooking.Api/Authorization/Requirements/ResourceOwnershipRequirement.cs
src/RestaurantBooking.Api/Attributes/AuthorizeResourceAttribute.cs
tests/RestaurantBooking.Api.Tests/Middleware/JwtAuthenticationMiddlewareTests.cs
tests/RestaurantBooking.Api.Tests/Authorization/AuthorizationPolicyTests.cs
```

## Success Criteria

### Security Excellence
- JWT authentication with secure token generation and validation
- BCrypt password hashing with proper salt implementation
- Refresh token rotation with concurrent usage detection
- Account lockout mechanisms prevent brute force attacks
- Comprehensive security event logging for monitoring

### Authorization Robustness
- Role-based authorization policies properly segregate user types
- Resource-based authorization ensures owners can only access their resources
- Custom authorization handlers support complex business scenarios
- Authentication middleware integrates seamlessly with the request pipeline

### Testing Thoroughness
- All authentication scenarios covered by comprehensive tests
- Security vulnerabilities prevented through rigorous testing
- Performance tests ensure authentication doesn't create bottlenecks
- Integration tests verify end-to-end authentication flows

### API Design Quality
- Consistent API endpoints across user types (owners and customers)
- Comprehensive input validation with clear error messages
- Proper HTTP status codes for all authentication scenarios
- User-friendly error responses that don't expose sensitive information

### Quality Assurance
- All acceptance criteria met and verified
- Security review completed for all authentication components
- Performance requirements met for authentication operations
- Documentation covers all authentication and authorization features

This epic establishes the complete authentication and authorization foundation that secures all subsequent features while providing a smooth user experience for both restaurant owners and customers.

using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantBooking.Api.Configuration;
using RestaurantBooking.Api.Extensions;

namespace RestaurantBooking.Api.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApiServices_Should_Register_All_Required_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services for DI resolution
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["JwtSettings:Key"] = "ThisIsASecretKeyForJwtTokenGenerationAndShouldBeAtLeast256Bits",
                ["JwtSettings:Issuer"] = "RestaurantBookingApi",
                ["JwtSettings:Audience"] = "RestaurantBookingClients",
                ["JwtSettings:ExpirationInMinutes"] = "60",
                ["ApplicationSettings:AppName"] = "Restaurant Booking System",
                ["ApplicationSettings:Version"] = "1.0.0"
            })
            .Build();

        // Act
        services.AddApiServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Authentication services
        serviceProvider.GetService<IAuthenticationSchemeProvider>().Should().NotBeNull();
        
        // Assert - Authorization services
        serviceProvider.GetService<IAuthorizationService>().Should().NotBeNull();
        
        // Assert - Health checks
        serviceProvider.GetService<HealthCheckService>().Should().NotBeNull();
        
        // Assert - Configuration options
        serviceProvider.GetService<IOptionsMonitor<JwtSettings>>().Should().NotBeNull();
        serviceProvider.GetService<IOptionsMonitor<ApplicationSettings>>().Should().NotBeNull();
    }

    [Fact]
    public void AddApiServices_Should_Configure_Correct_Service_Lifetimes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services for DI resolution
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = "ThisIsASecretKeyForJwtTokenGenerationAndShouldBeAtLeast256Bits",
                ["JwtSettings:Issuer"] = "RestaurantBookingApi",
                ["JwtSettings:Audience"] = "RestaurantBookingClients",
                ["JwtSettings:ExpirationInMinutes"] = "60"
            })
            .Build();

        // Act
        services.AddApiServices(configuration);

        // Assert - Check that health checks are registered as singleton
        var healthCheckDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(HealthCheckService));
        healthCheckDescriptor.Should().NotBeNull();
        healthCheckDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddApiServices_Should_Bind_Configuration_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services for DI resolution
        
        var jwtKey = "ThisIsASecretKeyForJwtTokenGenerationAndShouldBeAtLeast256Bits";
        var issuer = "RestaurantBookingApi";
        var audience = "RestaurantBookingClients";
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = jwtKey,
                ["JwtSettings:Issuer"] = issuer,
                ["JwtSettings:Audience"] = audience,
                ["JwtSettings:ExpirationInMinutes"] = "60",
                ["ApplicationSettings:AppName"] = "Restaurant Booking System",
                ["ApplicationSettings:Version"] = "1.0.0"
            })
            .Build();

        // Act
        services.AddApiServices(configuration);
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert - JWT Settings
        var jwtOptions = serviceProvider.GetService<IOptionsMonitor<JwtSettings>>();
        jwtOptions.Should().NotBeNull();
        var jwtSettings = jwtOptions!.CurrentValue;
        jwtSettings.Key.Should().Be(jwtKey);
        jwtSettings.Issuer.Should().Be(issuer);
        jwtSettings.Audience.Should().Be(audience);
        jwtSettings.ExpirationInMinutes.Should().Be(60);
        
        // Assert - Application Settings
        var appOptions = serviceProvider.GetService<IOptionsMonitor<ApplicationSettings>>();
        appOptions.Should().NotBeNull();
        var appSettings = appOptions!.CurrentValue;
        appSettings.AppName.Should().Be("Restaurant Booking System");
        appSettings.Version.Should().Be("1.0.0");
    }

    [Fact]
    public void AddApiServices_Should_Register_Health_Checks()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services for health checks
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        // Act
        services.AddApiServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    [Fact]
    public void AddApiServices_Should_Register_Authorization_Policies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act
        services.AddApiServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authorizationOptions = serviceProvider.GetService<IOptionsMonitor<AuthorizationOptions>>();
        authorizationOptions.Should().NotBeNull();
        
        var options = authorizationOptions!.CurrentValue;
        options.GetPolicy("RestaurantOwnerPolicy").Should().NotBeNull();
        options.GetPolicy("CustomerPolicy").Should().NotBeNull();
        options.GetPolicy("AuthenticatedUserPolicy").Should().NotBeNull();
    }

    [Fact]
    public void AddApiServices_Should_Handle_Missing_Jwt_Configuration_Gracefully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act & Assert - Should not throw
        Action act = () => services.AddApiServices(configuration);
        act.Should().NotThrow();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Should still register other services
        serviceProvider.GetService<IAuthorizationService>().Should().NotBeNull();
        serviceProvider.GetService<HealthCheckService>().Should().NotBeNull();
    }

    [Fact]
    public void AddApiServices_Should_Handle_Missing_Connection_String_Gracefully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = "ThisIsASecretKeyForJwtTokenGenerationAndShouldBeAtLeast256Bits",
                ["JwtSettings:Issuer"] = "RestaurantBookingApi",
                ["JwtSettings:Audience"] = "RestaurantBookingClients"
            })
            .Build();

        // Act & Assert - Should not throw
        Action act = () => services.AddApiServices(configuration);
        act.Should().NotThrow();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Should still register health checks (without database check)
        serviceProvider.GetService<HealthCheckService>().Should().NotBeNull();
    }

    [Fact]
    public void AddApiServices_Should_Register_Cors_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act
        services.AddApiServices(configuration);

        // Assert - CORS services should be registered
        var corsDescriptor = services.FirstOrDefault(s => s.ServiceType.Name.Contains("Cors"));
        corsDescriptor.Should().NotBeNull();
    }
}

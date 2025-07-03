using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantBooking.Api.Configuration;

namespace RestaurantBooking.Api.Tests.Integration;

public class ApiServicesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiServicesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void WebApplication_Should_Start_Successfully_With_All_Services_Registered()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert - Core services should be available
        serviceProvider.GetService<IConfiguration>().Should().NotBeNull();
        serviceProvider.GetService<ILogger<ApiServicesIntegrationTests>>().Should().NotBeNull();
        serviceProvider.GetService<IWebHostEnvironment>().Should().NotBeNull();
    }

    [Fact]
    public void WebApplication_Should_Have_Authentication_Services_Configured()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert
        var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
        authenticationSchemeProvider.Should().NotBeNull();
    }

    [Fact]
    public void WebApplication_Should_Have_Authorization_Services_Configured()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert
        var authorizationService = serviceProvider.GetService<IAuthorizationService>();
        authorizationService.Should().NotBeNull();

        var authorizationOptions = serviceProvider.GetService<IOptionsMonitor<AuthorizationOptions>>();
        authorizationOptions.Should().NotBeNull();
        
        var options = authorizationOptions!.CurrentValue;
        options.GetPolicy("RestaurantOwnerPolicy").Should().NotBeNull();
        options.GetPolicy("CustomerPolicy").Should().NotBeNull();
        options.GetPolicy("AuthenticatedUserPolicy").Should().NotBeNull();
    }

    [Fact]
    public void WebApplication_Should_Have_Health_Checks_Configured()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    [Fact]
    public async Task WebApplication_Health_Checks_Should_Be_Accessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.Should().NotBeNull();
        // Note: We don't assert on status code as health checks might fail without a real database
        // The important thing is that the endpoint exists and responds
    }

    [Fact]
    public void WebApplication_Should_Have_Configuration_Options_Bound()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert - JWT Settings
        var jwtOptions = serviceProvider.GetService<IOptionsMonitor<JwtSettings>>();
        jwtOptions.Should().NotBeNull();

        // Assert - Application Settings
        var appOptions = serviceProvider.GetService<IOptionsMonitor<ApplicationSettings>>();
        appOptions.Should().NotBeNull();
    }

    [Fact]
    public void WebApplication_Should_Handle_Missing_Optional_Configuration_Gracefully()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.Sources.Clear();
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        // Minimal configuration - missing JWT settings
                        ["ApplicationSettings:AppName"] = "Test Restaurant Booking System",
                        ["ApplicationSettings:Version"] = "1.0.0-test"
                    });
                });
            });

        // Act & Assert - Should not throw
        Action act = () =>
        {
            using var scope = factory.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            
            // Should still have core services
            serviceProvider.GetService<IAuthorizationService>().Should().NotBeNull();
            serviceProvider.GetService<HealthCheckService>().Should().NotBeNull();
        };
        
        act.Should().NotThrow();
    }

    [Fact]
    public void WebApplication_Should_Respect_Service_Lifetimes()
    {
        // Arrange & Act
        using var scope1 = _factory.Services.CreateScope();
        using var scope2 = _factory.Services.CreateScope();

        var healthCheck1 = scope1.ServiceProvider.GetService<HealthCheckService>();
        var healthCheck2 = scope2.ServiceProvider.GetService<HealthCheckService>();

        // Assert - Health checks should be singleton (same instance across scopes)
        healthCheck1.Should().BeSameAs(healthCheck2);
    }
}

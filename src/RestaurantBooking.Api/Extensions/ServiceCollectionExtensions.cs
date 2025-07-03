using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestaurantBooking.Api.Configuration;

namespace RestaurantBooking.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Configuration Options
        services.AddConfigurationOptions(configuration);
        
        // Add Authentication
        services.AddAuthenticationServices(configuration);
        
        // Add Authorization
        services.AddAuthorizationServices();
        
        // Add Health Checks
        services.AddHealthChecksServices(configuration);
        
        // Add CORS
        services.AddCorsServices();
        
        return services;
    }
    
    private static IServiceCollection AddConfigurationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure JWT Settings
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));
        
        // Configure Application Settings
        services.Configure<ApplicationSettings>(
            configuration.GetSection(ApplicationSettings.SectionName));
        
        return services;
    }
    
    private static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        
        // Always add authentication services, even without JWT configuration
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        
        // Add JWT Bearer authentication only if JWT settings are available
        if (jwtSettings?.Key != null)
        {
            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
            
            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Set to true in production
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
        
        return services;
    }
    
    private static IServiceCollection AddAuthorizationServices(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Add policy for restaurant owners
            options.AddPolicy("RestaurantOwnerPolicy", policy =>
                policy.RequireClaim("UserType", "RestaurantOwner"));
            
            // Add policy for customers
            options.AddPolicy("CustomerPolicy", policy =>
                policy.RequireClaim("UserType", "Customer"));
            
            // Add policy for authenticated users (both types)
            options.AddPolicy("AuthenticatedUserPolicy", policy =>
                policy.RequireAuthenticatedUser());
        });
        
        return services;
    }
    
    private static IServiceCollection AddHealthChecksServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add health checks if not already added (for unit testing scenarios)
        var healthChecksBuilder = services.AddHealthChecks();
        
        // Add database health check if connection string is available
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            healthChecksBuilder.AddNpgSql(
                connectionString,
                name: "database",
                failureStatus: HealthStatus.Degraded);
        }
        
        return services;
    }
    
    private static IServiceCollection AddCorsServices(
        this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy", builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            
            options.AddPolicy("ProductionPolicy", builder =>
                builder.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials());
        });
        
        return services;
    }
}

using RestaurantBooking.Infrastructure.Extensions;
using RestaurantBooking.Infrastructure.Data;
using RestaurantBooking.Api.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add infrastructure services (EF Core, Repositories, etc.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add API services (Authentication, Authorization, Health Checks, Configuration, CORS)
builder.Services.AddApiServices(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Map default health check endpoint
app.MapDefaultEndpoints();

// Map health check endpoint from our API services
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Use development CORS policy
    app.UseCors("DevelopmentPolicy");
}
else
{
    // Use production CORS policy
    app.UseCors("ProductionPolicy");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Add test endpoint to verify API is working
app.MapGet("/test", () => 
{
    return Results.Ok(new { message = "API Working", status = "success", timestamp = DateTime.UtcNow });
})
.WithName("TestAPI");

// Add database connection test endpoint
app.MapGet("/test/database", async (RestaurantBookingDbContext dbContext) =>
{
    try
    {
        // Test database connection
        var canConnect = await dbContext.Database.CanConnectAsync();
        
        if (canConnect)
        {
            // Get database info
            var connectionString = dbContext.Database.GetConnectionString();
            var provider = dbContext.Database.ProviderName;
            
            // Test a simple query
            var restaurantOwnersCount = await dbContext.RestaurantOwners.CountAsync();
            var restaurantsCount = await dbContext.Restaurants.CountAsync();
            var tablesCount = await dbContext.Tables.CountAsync();
            var customersCount = await dbContext.Customers.CountAsync();
            var reservationsCount = await dbContext.Reservations.CountAsync();
            var operatingHoursCount = await dbContext.OperatingHours.CountAsync();
            var availabilityBlocksCount = await dbContext.AvailabilityBlocks.CountAsync();
            
            return Results.Ok(new
            {
                status = "success",
                message = "Database connection successful",
                database = new
                {
                    connectionString = connectionString?.Replace("Password=", "Password=***"), // Hide password for security
                    provider = provider,
                    canConnect = canConnect
                },
                data = new
                {
                    restaurantOwnersCount,
                    restaurantsCount,
                    tablesCount,
                    customersCount,
                    reservationsCount,
                    operatingHoursCount,
                    availabilityBlocksCount
                },
                timestamp = DateTime.UtcNow
            });
        }
        else
        {
            return Results.Problem(
                detail: "Could not connect to database",
                statusCode: 500,
                title: "Database Connection Failed"
            );
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Database Connection Error"
        );
    }
})
.WithName("TestDatabase");

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
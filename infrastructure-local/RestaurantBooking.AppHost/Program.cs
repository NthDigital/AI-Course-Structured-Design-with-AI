var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL Database Reference with pgAdmin
// Ensure you have 'using Microsoft.DurableTask.DistributedApplication;' or the correct using for your SDK
var postgres = builder.AddPostgres("RestaurantBookingDb")
    .WithPgAdmin();

// Add API project
var api = builder.AddProject<Projects.RestaurantBooking_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithExternalHttpEndpoints();

// Add React frontend with proper API reference
var react = builder.AddNpmApp("react", "../../src/restaurant-booking-frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();

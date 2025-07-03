namespace RestaurantBooking.Api.Configuration;

public class ApplicationSettings
{
    public const string SectionName = "ApplicationSettings";
    
    public string AppName { get; set; } = "Restaurant Booking System";
    public string Version { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
    public bool EnableDetailedErrors { get; set; } = true;
    public bool EnableSwagger { get; set; } = true;
    public int MaxRequestBodySize { get; set; } = 1024 * 1024; // 1MB
}

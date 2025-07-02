using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Interfaces.Services;

public interface IAvailabilityService
{
    Task<bool> IsTableAvailableAsync(int tableId, DateTime startTime, DateTime endTime);
    Task<bool> IsRestaurantOpenAsync(int restaurantId, DateTime dateTime);
    Task<ValidationResult> CheckAvailabilityAsync(int restaurantId, int tableId, DateTime startTime, DateTime endTime);
}

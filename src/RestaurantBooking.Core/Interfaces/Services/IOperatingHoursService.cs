using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Interfaces.Services;

public interface IOperatingHoursService
{
    Task<ValidationResult> ValidateOperatingHoursAsync(int restaurantId, DayOfWeek dayOfWeek, TimeOnly openTime, TimeOnly closeTime, bool isOvernight = false);
    Task<bool> IsWithinOperatingHoursAsync(int restaurantId, DateTime dateTime);
    Task<ValidationResult> ValidateReservationTimeAsync(int restaurantId, DateTime reservationDateTime);
}

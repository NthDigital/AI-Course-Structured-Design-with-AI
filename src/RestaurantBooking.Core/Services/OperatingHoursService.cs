using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;
using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Services;

public class OperatingHoursService : IOperatingHoursService
{
    private readonly IOperatingHoursRepository _operatingHoursRepository;

    public OperatingHoursService(IOperatingHoursRepository operatingHoursRepository)
    {
        _operatingHoursRepository = operatingHoursRepository;
    }

    public Task<ValidationResult> ValidateOperatingHoursAsync(int restaurantId, DayOfWeek dayOfWeek, TimeOnly openTime, TimeOnly closeTime, bool isOvernight = false)
    {
        var validationErrors = new List<string>();

        // Validate restaurant ID
        if (restaurantId <= 0)
        {
            validationErrors.Add("Restaurant ID must be greater than zero");
        }

        // Validate time range for non-overnight hours
        if (!isOvernight && closeTime <= openTime)
        {
            validationErrors.Add("Close time must be after open time for non-overnight hours");
        }

        // For overnight hours, we allow close time to be before open time
        // Additional business rules could be added here

        var result = validationErrors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(validationErrors);

        return Task.FromResult(result);
    }

    public async Task<bool> IsWithinOperatingHoursAsync(int restaurantId, DateTime dateTime)
    {
        var operatingHours = await _operatingHoursRepository.GetByRestaurantAndDayAsync(restaurantId, dateTime.DayOfWeek);
        
        if (operatingHours == null || !operatingHours.IsOpen)
            return false;

        var time = TimeOnly.FromDateTime(dateTime);
        return operatingHours.IsWithinOperatingHours(time);
    }

    public async Task<ValidationResult> ValidateReservationTimeAsync(int restaurantId, DateTime reservationDateTime)
    {
        var validationErrors = new List<string>();

        var operatingHours = await _operatingHoursRepository.GetByRestaurantAndDayAsync(restaurantId, reservationDateTime.DayOfWeek);
        
        if (operatingHours == null || !operatingHours.IsOpen)
        {
            validationErrors.Add($"Restaurant is closed on {reservationDateTime.DayOfWeek}");
            return ValidationResult.Failure(validationErrors);
        }

        var reservationTime = TimeOnly.FromDateTime(reservationDateTime);
        var reservationEndTime = TimeOnly.FromDateTime(reservationDateTime.AddHours(3)); // 3-hour reservation

        // Check if reservation start time is within operating hours
        if (!operatingHours.IsWithinOperatingHours(reservationTime))
        {
            validationErrors.Add("Reservation start time is outside operating hours");
        }

        // Check if reservation end time is within operating hours (considering overnight operations)
        if (!operatingHours.IsOvernight)
        {
            // For regular (non-overnight) hours, reservation must end before closing
            if (reservationEndTime > operatingHours.CloseTime)
            {
                validationErrors.Add("Reservation would extend beyond closing time");
            }
        }
        else
        {
            // For overnight hours, need more complex logic
            // This is a simplified check - in reality, you'd need to handle overnight reservations more carefully
            if (reservationTime >= operatingHours.OpenTime && reservationEndTime <= operatingHours.CloseTime)
            {
                // Reservation starts after opening and ends before closing the next day - this is problematic for overnight
                validationErrors.Add("Reservation spans across overnight hours in an invalid way");
            }
        }

        return validationErrors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(validationErrors);
    }
}

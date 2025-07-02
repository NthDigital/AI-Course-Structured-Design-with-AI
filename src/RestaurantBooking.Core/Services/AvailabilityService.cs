using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;
using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly IAvailabilityBlockRepository _availabilityBlockRepository;

    public AvailabilityService(
        IReservationRepository reservationRepository,
        IOperatingHoursRepository operatingHoursRepository,
        IAvailabilityBlockRepository availabilityBlockRepository)
    {
        _reservationRepository = reservationRepository;
        _operatingHoursRepository = operatingHoursRepository;
        _availabilityBlockRepository = availabilityBlockRepository;
    }

    public async Task<bool> IsTableAvailableAsync(int tableId, DateTime startTime, DateTime endTime)
    {
        var conflictingReservations = await _reservationRepository.GetConflictingReservationsAsync(tableId, startTime, endTime);
        return !conflictingReservations.Any();
    }

    public async Task<bool> IsRestaurantOpenAsync(int restaurantId, DateTime dateTime)
    {
        var operatingHours = await _operatingHoursRepository.GetByRestaurantAndDayAsync(restaurantId, dateTime.DayOfWeek);
        
        if (operatingHours == null || !operatingHours.IsOpen)
            return false;

        var time = TimeOnly.FromDateTime(dateTime);
        return operatingHours.IsWithinOperatingHours(time);
    }

    public async Task<ValidationResult> CheckAvailabilityAsync(int restaurantId, int tableId, DateTime startTime, DateTime endTime)
    {
        var validationErrors = new List<string>();

        // Check if restaurant is open
        var isOpen = await IsRestaurantOpenAsync(restaurantId, startTime);
        if (!isOpen)
        {
            validationErrors.Add("Restaurant is closed during the requested time");
        }

        // Check if table is available
        var isTableAvailable = await IsTableAvailableAsync(tableId, startTime, endTime);
        if (!isTableAvailable)
        {
            validationErrors.Add("Table is not available during the requested time");
        }

        // Check for availability blocks
        var availabilityBlocks = await _availabilityBlockRepository.GetActiveBlocksAsync(restaurantId, startTime);
        if (availabilityBlocks.Any())
        {
            validationErrors.Add("Restaurant has availability restrictions during the requested time");
        }

        return validationErrors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(validationErrors);
    }
}

using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;
using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Services;

public class ReservationValidationService : IReservationValidationService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IOperatingHoursRepository _operatingHoursRepository;

    public ReservationValidationService(
        IRestaurantRepository restaurantRepository,
        ITableRepository tableRepository,
        IReservationRepository reservationRepository,
        IOperatingHoursRepository operatingHoursRepository)
    {
        _restaurantRepository = restaurantRepository;
        _tableRepository = tableRepository;
        _reservationRepository = reservationRepository;
        _operatingHoursRepository = operatingHoursRepository;
    }

    public async Task<ValidationResult> ValidateReservationAsync(int restaurantId, int tableId, DateTime reservationDateTime, int partySize)
    {
        var validationErrors = new List<string>();

        // Validate restaurant exists
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
        if (restaurant == null)
        {
            validationErrors.Add("Restaurant not found");
            return ValidationResult.Failure(validationErrors);
        }

        // Validate table exists
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null)
        {
            validationErrors.Add("Table not found");
            return ValidationResult.Failure(validationErrors);
        }

        // Validate table capacity
        if (table.Capacity < partySize)
        {
            validationErrors.Add("Table capacity is insufficient for party size");
        }

        // Validate table belongs to restaurant
        if (table.RestaurantId != restaurantId)
        {
            validationErrors.Add("Table does not belong to the specified restaurant");
        }

        // Validate operating hours
        var operatingHours = await _operatingHoursRepository.GetByRestaurantAndDayAsync(restaurantId, reservationDateTime.DayOfWeek);
        if (operatingHours == null || !operatingHours.IsOpen)
        {
            validationErrors.Add("Restaurant is closed on the requested day");
        }
        else
        {
            var reservationTime = TimeOnly.FromDateTime(reservationDateTime);
            if (!operatingHours.IsWithinOperatingHours(reservationTime))
            {
                validationErrors.Add("Reservation time is outside operating hours");
            }
        }

        // Check for conflicting reservations
        var endDateTime = reservationDateTime.AddHours(3);
        var conflictingReservations = await _reservationRepository.GetConflictingReservationsAsync(tableId, reservationDateTime, endDateTime);
        if (conflictingReservations.Any())
        {
            validationErrors.Add("Table is not available during the requested time");
        }

        return validationErrors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(validationErrors);
    }
}

using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Interfaces.Services;

public interface IReservationValidationService
{
    Task<ValidationResult> ValidateReservationAsync(int restaurantId, int tableId, DateTime reservationDateTime, int partySize);
}

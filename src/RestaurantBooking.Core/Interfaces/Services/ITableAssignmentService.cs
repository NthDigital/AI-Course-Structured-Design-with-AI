using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Interfaces.Services;

public interface ITableAssignmentService
{
    Task<Table?> FindBestAvailableTableAsync(int restaurantId, DateTime reservationDateTime, int partySize);
    Task<IEnumerable<Table>> GetAvailableTablesAsync(int restaurantId, DateTime reservationDateTime, int partySize);
}

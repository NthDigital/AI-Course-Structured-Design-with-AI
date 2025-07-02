using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Interfaces.Repositories;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int id);
    Task<IEnumerable<Reservation>> GetAllAsync();
    Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Reservation>> GetByRestaurantIdAsync(int restaurantId);
    Task<IEnumerable<Reservation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Reservation>> GetConflictingReservationsAsync(int tableId, DateTime startTime, DateTime endTime);
    Task<Reservation> AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task DeleteAsync(int id);
}

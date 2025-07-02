using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Interfaces.Repositories;

public interface IAvailabilityBlockRepository
{
    Task<AvailabilityBlock?> GetByIdAsync(int id);
    Task<IEnumerable<AvailabilityBlock>> GetAllAsync();
    Task<IEnumerable<AvailabilityBlock>> GetByRestaurantIdAsync(int restaurantId);
    Task<IEnumerable<AvailabilityBlock>> GetByDateRangeAsync(int restaurantId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<AvailabilityBlock>> GetActiveBlocksAsync(int restaurantId, DateTime dateTime);
    Task<AvailabilityBlock> AddAsync(AvailabilityBlock availabilityBlock);
    Task UpdateAsync(AvailabilityBlock availabilityBlock);
    Task DeleteAsync(int id);
}

using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Interfaces.Repositories;

public interface ITableRepository
{
    Task<Table?> GetByIdAsync(int id);
    Task<IEnumerable<Table>> GetAllAsync();
    Task<IEnumerable<Table>> GetByRestaurantIdAsync(int restaurantId);
    Task<IEnumerable<Table>> GetAvailableTablesAsync(int restaurantId, DateTime startTime, DateTime endTime, int minCapacity);
    Task<Table> AddAsync(Table table);
    Task UpdateAsync(Table table);
    Task DeleteAsync(int id);
}

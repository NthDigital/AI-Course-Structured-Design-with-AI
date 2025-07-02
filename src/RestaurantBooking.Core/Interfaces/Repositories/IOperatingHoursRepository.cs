using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Interfaces.Repositories;

public interface IOperatingHoursRepository
{
    Task<OperatingHours?> GetByIdAsync(int id);
    Task<IEnumerable<OperatingHours>> GetAllAsync();
    Task<IEnumerable<OperatingHours>> GetByRestaurantIdAsync(int restaurantId);
    Task<OperatingHours?> GetByRestaurantAndDayAsync(int restaurantId, DayOfWeek dayOfWeek);
    Task<OperatingHours> AddAsync(OperatingHours operatingHours);
    Task UpdateAsync(OperatingHours operatingHours);
    Task DeleteAsync(int id);
}

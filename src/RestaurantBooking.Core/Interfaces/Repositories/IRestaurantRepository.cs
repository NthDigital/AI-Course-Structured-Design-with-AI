using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Interfaces.Repositories;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetByIdAsync(int id);
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(int ownerId);
    Task<Restaurant> AddAsync(Restaurant restaurant);
    Task UpdateAsync(Restaurant restaurant);
    Task DeleteAsync(int id);
}

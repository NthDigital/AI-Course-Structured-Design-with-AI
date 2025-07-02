using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Interfaces.Repositories;

public interface IRestaurantOwnerRepository
{
    Task<RestaurantOwner?> GetByIdAsync(int id);
    Task<RestaurantOwner?> GetByEmailAsync(string email);
    Task<IEnumerable<RestaurantOwner>> GetAllAsync();
    Task<RestaurantOwner> AddAsync(RestaurantOwner restaurantOwner);
    Task UpdateAsync(RestaurantOwner restaurantOwner);
    Task DeleteAsync(int id);
}

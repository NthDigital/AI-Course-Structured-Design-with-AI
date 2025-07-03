using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly RestaurantBookingDbContext _context;

    public RestaurantRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant?> GetByIdAsync(int id)
    {
        return await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        return await _context.Restaurants.ToListAsync();
    }

    public async Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Restaurants
            .Where(r => r.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<Restaurant> AddAsync(Restaurant restaurant)
    {
        _context.Restaurants.Add(restaurant);
        return await Task.FromResult(restaurant);
    }

    public Task UpdateAsync(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant != null)
        {
            _context.Restaurants.Remove(restaurant);
        }
    }
}

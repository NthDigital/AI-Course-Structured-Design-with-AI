using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Repositories;

public class RestaurantOwnerRepository : IRestaurantOwnerRepository
{
    private readonly RestaurantBookingDbContext _context;

    public RestaurantOwnerRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task<RestaurantOwner?> GetByIdAsync(int id)
    {
        return await _context.RestaurantOwners.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<RestaurantOwner>> GetAllAsync()
    {
        return await _context.RestaurantOwners.ToListAsync();
    }

    public async Task<RestaurantOwner?> GetByEmailAsync(string email)
    {
        return await _context.RestaurantOwners.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<RestaurantOwner> AddAsync(RestaurantOwner restaurantOwner)
    {
        await _context.RestaurantOwners.AddAsync(restaurantOwner);
        return restaurantOwner;
    }

    public Task UpdateAsync(RestaurantOwner restaurantOwner)
    {
        _context.RestaurantOwners.Update(restaurantOwner);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var restaurantOwner = await GetByIdAsync(id);
        if (restaurantOwner != null)
        {
            _context.RestaurantOwners.Remove(restaurantOwner);
        }
    }
}

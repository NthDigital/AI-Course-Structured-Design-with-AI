using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Repositories;

public class OperatingHoursRepository : IOperatingHoursRepository
{
    private readonly RestaurantBookingDbContext _context;

    public OperatingHoursRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task<OperatingHours?> GetByIdAsync(int id)
    {
        return await _context.OperatingHours.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<OperatingHours>> GetAllAsync()
    {
        return await _context.OperatingHours.ToListAsync();
    }

    public async Task<IEnumerable<OperatingHours>> GetByRestaurantIdAsync(int restaurantId)
    {
        return await _context.OperatingHours.Where(x => x.RestaurantId == restaurantId).ToListAsync();
    }

    public async Task<OperatingHours?> GetByRestaurantAndDayAsync(int restaurantId, DayOfWeek dayOfWeek)
    {
        return await _context.OperatingHours
            .FirstOrDefaultAsync(x => x.RestaurantId == restaurantId && x.DayOfWeek == dayOfWeek);
    }

    public async Task<OperatingHours> AddAsync(OperatingHours operatingHours)
    {
        await _context.OperatingHours.AddAsync(operatingHours);
        return operatingHours;
    }

    public Task UpdateAsync(OperatingHours operatingHours)
    {
        _context.OperatingHours.Update(operatingHours);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var operatingHours = await GetByIdAsync(id);
        if (operatingHours != null)
        {
            _context.OperatingHours.Remove(operatingHours);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Repositories;

public class AvailabilityBlockRepository : IAvailabilityBlockRepository
{
    private readonly RestaurantBookingDbContext _context;

    public AvailabilityBlockRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task<AvailabilityBlock?> GetByIdAsync(int id)
    {
        return await _context.AvailabilityBlocks.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<AvailabilityBlock>> GetAllAsync()
    {
        return await _context.AvailabilityBlocks.ToListAsync();
    }

    public async Task<IEnumerable<AvailabilityBlock>> GetByRestaurantIdAsync(int restaurantId)
    {
        return await _context.AvailabilityBlocks.Where(x => x.RestaurantId == restaurantId).ToListAsync();
    }

    public async Task<IEnumerable<AvailabilityBlock>> GetByDateRangeAsync(int restaurantId, DateTime startDate, DateTime endDate)
    {
        return await _context.AvailabilityBlocks
            .Where(x => x.RestaurantId == restaurantId && 
                       x.StartDateTime <= endDate && 
                       x.EndDateTime >= startDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<AvailabilityBlock>> GetActiveBlocksAsync(int restaurantId, DateTime dateTime)
    {
        return await _context.AvailabilityBlocks
            .Where(x => x.RestaurantId == restaurantId && 
                       x.StartDateTime <= dateTime && 
                       x.EndDateTime >= dateTime)
            .ToListAsync();
    }

    public async Task<AvailabilityBlock> AddAsync(AvailabilityBlock availabilityBlock)
    {
        await _context.AvailabilityBlocks.AddAsync(availabilityBlock);
        return availabilityBlock;
    }

    public Task UpdateAsync(AvailabilityBlock availabilityBlock)
    {
        _context.AvailabilityBlocks.Update(availabilityBlock);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var availabilityBlock = await GetByIdAsync(id);
        if (availabilityBlock != null)
        {
            _context.AvailabilityBlocks.Remove(availabilityBlock);
        }
    }
}

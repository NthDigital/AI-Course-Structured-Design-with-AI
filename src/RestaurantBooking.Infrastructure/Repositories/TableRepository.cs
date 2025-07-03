using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Repositories;

public class TableRepository : ITableRepository
{
    private readonly RestaurantBookingDbContext _context;

    public TableRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Table?> GetByIdAsync(int id)
    {
        return await _context.Tables.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Table>> GetAllAsync()
    {
        return await _context.Tables.ToListAsync();
    }

    public async Task<IEnumerable<Table>> GetByRestaurantIdAsync(int restaurantId)
    {
        return await _context.Tables.Where(x => x.RestaurantId == restaurantId).ToListAsync();
    }

    public async Task<IEnumerable<Table>> GetAvailableTablesAsync(int restaurantId, DateTime startTime, DateTime endTime, int minCapacity)
    {
        // This is a simplified implementation - would need more complex logic in real scenario
        return await _context.Tables
            .Where(x => x.RestaurantId == restaurantId && x.Capacity >= minCapacity && x.Status == TableStatus.Available)
            .ToListAsync();
    }

    public async Task<Table> AddAsync(Table table)
    {
        await _context.Tables.AddAsync(table);
        return table;
    }

    public Task UpdateAsync(Table table)
    {
        _context.Tables.Update(table);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var table = await GetByIdAsync(id);
        if (table != null)
        {
            _context.Tables.Remove(table);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Infrastructure.Data;

namespace RestaurantBooking.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly RestaurantBookingDbContext _context;

    public ReservationRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _context.Reservations.ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Reservations.Where(x => x.CustomerId == customerId).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByRestaurantIdAsync(int restaurantId)
    {
        return await _context.Reservations.Where(x => x.RestaurantId == restaurantId).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .Where(x => x.ReservationDateTime >= startDate && x.ReservationDateTime <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetConflictingReservationsAsync(int tableId, DateTime startTime, DateTime endTime)
    {
        return await _context.Reservations
            .Where(x => x.TableId == tableId && 
                       x.ReservationDateTime < endTime && 
                       x.ReservationDateTime.AddHours(3) > startTime) // Assuming 3-hour reservation duration
            .ToListAsync();
    }

    public async Task<Reservation> AddAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
        return reservation;
    }

    public Task UpdateAsync(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var reservation = await GetByIdAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
        }
    }
}

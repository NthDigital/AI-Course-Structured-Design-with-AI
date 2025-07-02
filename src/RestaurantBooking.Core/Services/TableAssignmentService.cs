using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;

namespace RestaurantBooking.Core.Services;

public class TableAssignmentService : ITableAssignmentService
{
    private readonly ITableRepository _tableRepository;
    private readonly IReservationRepository _reservationRepository;

    public TableAssignmentService(ITableRepository tableRepository, IReservationRepository reservationRepository)
    {
        _tableRepository = tableRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<Table?> FindBestAvailableTableAsync(int restaurantId, DateTime reservationDateTime, int partySize)
    {
        var availableTables = await GetAvailableTablesAsync(restaurantId, reservationDateTime, partySize);
        
        // Return the table with the smallest capacity that can accommodate the party
        return availableTables
            .Where(t => t.Capacity >= partySize)
            .OrderBy(t => t.Capacity)
            .FirstOrDefault();
    }

    public async Task<IEnumerable<Table>> GetAvailableTablesAsync(int restaurantId, DateTime reservationDateTime, int partySize)
    {
        var allTables = await _tableRepository.GetByRestaurantIdAsync(restaurantId);
        var endDateTime = reservationDateTime.AddHours(3);
        var availableTables = new List<Table>();

        foreach (var table in allTables)
        {
            // Skip tables that don't have sufficient capacity
            if (table.Capacity < partySize)
                continue;

            // Check if the table has any conflicting reservations
            var conflictingReservations = await _reservationRepository.GetConflictingReservationsAsync(
                table.Id, reservationDateTime, endDateTime);

            if (!conflictingReservations.Any())
            {
                availableTables.Add(table);
            }
        }

        return availableTables;
    }
}

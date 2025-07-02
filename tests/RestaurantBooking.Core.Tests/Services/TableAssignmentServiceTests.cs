using Moq;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;
using RestaurantBooking.Core.Services;

namespace RestaurantBooking.Core.Tests.Services;

public class TableAssignmentServiceTests
{
    private readonly Mock<ITableRepository> _mockTableRepository;
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly ITableAssignmentService _service;

    public TableAssignmentServiceTests()
    {
        _mockTableRepository = new Mock<ITableRepository>();
        _mockReservationRepository = new Mock<IReservationRepository>();
        
        _service = new TableAssignmentService(_mockTableRepository.Object, _mockReservationRepository.Object);
    }

    [Fact]
    public async Task FindBestAvailableTableAsync_Should_Return_Smallest_Available_Table_That_Fits_PartySize()
    {
        // Arrange
        var restaurantId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(12);
        var partySize = 3;
        var endDateTime = reservationDateTime.AddHours(3);

        var tables = new List<Table>
        {
            new Table("1", 2, restaurantId), // Too small
            new Table("2", 4, restaurantId), // Perfect fit (smallest that accommodates party)
            new Table("3", 6, restaurantId), // Larger than needed
            new Table("4", 4, restaurantId)  // Same size as table 2
        };

        _mockTableRepository.Setup(r => r.GetByRestaurantIdAsync(restaurantId))
            .ReturnsAsync(tables);

        // Mock that all tables are available (no conflicting reservations)
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(It.IsAny<int>(), reservationDateTime, endDateTime))
            .ReturnsAsync(new List<Reservation>());

        // Act
        var result = await _service.FindBestAvailableTableAsync(restaurantId, reservationDateTime, partySize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Capacity); // Should return the 4-person table (smallest that fits)
        Assert.True(result.Capacity >= partySize);
    }

    [Fact]
    public async Task FindBestAvailableTableAsync_Should_Return_Null_When_No_Tables_Available()
    {
        // Arrange
        var restaurantId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(12);
        var partySize = 4;
        var endDateTime = reservationDateTime.AddHours(3);

        var tables = new List<Table>
        {
            new Table("1", 4, restaurantId),
            new Table("2", 6, restaurantId)
        };

        _mockTableRepository.Setup(r => r.GetByRestaurantIdAsync(restaurantId))
            .ReturnsAsync(tables);

        // Mock that all tables have conflicting reservations
        var conflictingReservation = new Reservation(1, restaurantId, 1, reservationDateTime, 4, null);
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(It.IsAny<int>(), reservationDateTime, endDateTime))
            .ReturnsAsync(new List<Reservation> { conflictingReservation });

        // Act
        var result = await _service.FindBestAvailableTableAsync(restaurantId, reservationDateTime, partySize);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindBestAvailableTableAsync_Should_Return_Null_When_No_Tables_Have_Sufficient_Capacity()
    {
        // Arrange
        var restaurantId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(12);
        var partySize = 8; // Large party
        var endDateTime = reservationDateTime.AddHours(3);

        var tables = new List<Table>
        {
            new Table("1", 2, restaurantId),
            new Table("2", 4, restaurantId),
            new Table("3", 6, restaurantId) // All too small
        };

        _mockTableRepository.Setup(r => r.GetByRestaurantIdAsync(restaurantId))
            .ReturnsAsync(tables);

        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(It.IsAny<int>(), reservationDateTime, endDateTime))
            .ReturnsAsync(new List<Reservation>());

        // Act
        var result = await _service.FindBestAvailableTableAsync(restaurantId, reservationDateTime, partySize);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAvailableTablesAsync_Should_Return_All_Available_Tables_With_Sufficient_Capacity()
    {
        // Arrange
        var restaurantId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(12);
        var partySize = 3;
        var endDateTime = reservationDateTime.AddHours(3);

        var tables = new List<Table>
        {
            new Table("1", 2, restaurantId), // Too small
            new Table("2", 4, restaurantId), // Available and sufficient
            new Table("3", 6, restaurantId), // Available and sufficient
            new Table("4", 4, restaurantId)  // Available and sufficient
        };

        _mockTableRepository.Setup(r => r.GetByRestaurantIdAsync(restaurantId))
            .ReturnsAsync(tables);

        // Mock that tables 2, 3, and 4 are available
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(2, reservationDateTime, endDateTime))
            .ReturnsAsync(new List<Reservation>());
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(3, reservationDateTime, endDateTime))
            .ReturnsAsync(new List<Reservation>());
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(4, reservationDateTime, endDateTime))
            .ReturnsAsync(new List<Reservation>());

        // Table 1 has conflicts (or we don't set it up, which will return empty by default)
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(1, reservationDateTime, endDateTime))
            .ReturnsAsync(new List<Reservation>());

        // Act
        var result = await _service.GetAvailableTablesAsync(restaurantId, reservationDateTime, partySize);

        // Assert
        var availableTables = result.ToList();
        Assert.Equal(3, availableTables.Count); // Tables 2, 3, and 4
        Assert.All(availableTables, table => Assert.True(table.Capacity >= partySize));
    }
}

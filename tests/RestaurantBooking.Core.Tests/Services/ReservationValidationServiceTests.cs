using Moq;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;
using RestaurantBooking.Core.Services;

namespace RestaurantBooking.Core.Tests.Services;

public class ReservationValidationServiceTests
{
    private readonly Mock<IRestaurantRepository> _mockRestaurantRepository;
    private readonly Mock<ITableRepository> _mockTableRepository;
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly Mock<IOperatingHoursRepository> _mockOperatingHoursRepository;
    private readonly IReservationValidationService _service;

    public ReservationValidationServiceTests()
    {
        _mockRestaurantRepository = new Mock<IRestaurantRepository>();
        _mockTableRepository = new Mock<ITableRepository>();
        _mockReservationRepository = new Mock<IReservationRepository>();
        _mockOperatingHoursRepository = new Mock<IOperatingHoursRepository>();
        
        _service = new ReservationValidationService(
            _mockRestaurantRepository.Object,
            _mockTableRepository.Object,
            _mockReservationRepository.Object,
            _mockOperatingHoursRepository.Object);
    }

    [Fact]
    public async Task ValidateReservationAsync_Should_Return_True_When_All_Validations_Pass()
    {
        // Arrange
        var restaurantId = 1;
        var tableId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(12); // Tomorrow at noon - within operating hours
        var partySize = 4;

        var restaurant = new Restaurant("Test Restaurant", "Italian", "Great food", "123 Main St", "555-1234", 1);
        var table = new Table("1", 4, restaurantId); // Table belongs to the restaurant
        var operatingHours = new OperatingHours(restaurantId, reservationDateTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(22, 0));

        _mockRestaurantRepository.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);
        _mockTableRepository.Setup(t => t.GetByIdAsync(tableId))
            .ReturnsAsync(table);
        _mockOperatingHoursRepository.Setup(o => o.GetByRestaurantAndDayAsync(restaurantId, reservationDateTime.DayOfWeek))
            .ReturnsAsync(operatingHours);
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(tableId, reservationDateTime, reservationDateTime.AddHours(3)))
            .ReturnsAsync(new List<Reservation>());

        // Act
        var result = await _service.ValidateReservationAsync(restaurantId, tableId, reservationDateTime, partySize);

        // Assert
        Assert.True(result.IsValid, $"Validation failed with errors: {string.Join(", ", result.ValidationErrors)}");
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public async Task ValidateReservationAsync_Should_Return_False_When_Restaurant_Does_Not_Exist()
    {
        // Arrange
        var restaurantId = 1;
        var tableId = 1;
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var partySize = 4;

        _mockRestaurantRepository.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync((Restaurant?)null);

        // Act
        var result = await _service.ValidateReservationAsync(restaurantId, tableId, reservationDateTime, partySize);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Restaurant not found", result.ValidationErrors);
    }

    [Fact]
    public async Task ValidateReservationAsync_Should_Return_False_When_Table_Does_Not_Exist()
    {
        // Arrange
        var restaurantId = 1;
        var tableId = 1;
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var partySize = 4;

        var restaurant = new Restaurant("Test Restaurant", "Italian", "Great food", "123 Main St", "555-1234", 1);

        _mockRestaurantRepository.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);
        _mockTableRepository.Setup(t => t.GetByIdAsync(tableId))
            .ReturnsAsync((Table?)null);

        // Act
        var result = await _service.ValidateReservationAsync(restaurantId, tableId, reservationDateTime, partySize);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Table not found", result.ValidationErrors);
    }

    [Fact]
    public async Task ValidateReservationAsync_Should_Return_False_When_Table_Capacity_Is_Insufficient()
    {
        // Arrange
        var restaurantId = 1;
        var tableId = 1;
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var partySize = 6;

        var restaurant = new Restaurant("Test Restaurant", "Italian", "Great food", "123 Main St", "555-1234", 1);
        var table = new Table("1", 4, 1); // Capacity of 4, but party size is 6

        _mockRestaurantRepository.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);
        _mockTableRepository.Setup(t => t.GetByIdAsync(tableId))
            .ReturnsAsync(table);

        // Act
        var result = await _service.ValidateReservationAsync(restaurantId, tableId, reservationDateTime, partySize);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Table capacity is insufficient for party size", result.ValidationErrors);
    }
}

using Moq;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;
using RestaurantBooking.Core.Services;

namespace RestaurantBooking.Core.Tests.Services;

public class AvailabilityServiceTests
{
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly Mock<IOperatingHoursRepository> _mockOperatingHoursRepository;
    private readonly Mock<IAvailabilityBlockRepository> _mockAvailabilityBlockRepository;
    private readonly IAvailabilityService _service;

    public AvailabilityServiceTests()
    {
        _mockReservationRepository = new Mock<IReservationRepository>();
        _mockOperatingHoursRepository = new Mock<IOperatingHoursRepository>();
        _mockAvailabilityBlockRepository = new Mock<IAvailabilityBlockRepository>();
        
        _service = new AvailabilityService(
            _mockReservationRepository.Object,
            _mockOperatingHoursRepository.Object,
            _mockAvailabilityBlockRepository.Object);
    }

    [Fact]
    public async Task IsTableAvailableAsync_Should_Return_True_When_No_Conflicting_Reservations()
    {
        // Arrange
        var tableId = 1;
        var startTime = DateTime.Today.AddDays(1).AddHours(12);
        var endTime = startTime.AddHours(3);

        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(tableId, startTime, endTime))
            .ReturnsAsync(new List<Reservation>());

        // Act
        var result = await _service.IsTableAvailableAsync(tableId, startTime, endTime);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsTableAvailableAsync_Should_Return_False_When_Conflicting_Reservations_Exist()
    {
        // Arrange
        var tableId = 1;
        var startTime = DateTime.Today.AddDays(1).AddHours(12);
        var endTime = startTime.AddHours(3);

        var conflictingReservation = new Reservation(1, 1, tableId, startTime, 4, null);
        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(tableId, startTime, endTime))
            .ReturnsAsync(new List<Reservation> { conflictingReservation });

        // Act
        var result = await _service.IsTableAvailableAsync(tableId, startTime, endTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRestaurantOpenAsync_Should_Return_True_When_Restaurant_Is_Open()
    {
        // Arrange
        var restaurantId = 1;
        var dateTime = DateTime.Today.AddDays(1).AddHours(12); // Monday at noon

        var operatingHours = new OperatingHours(restaurantId, dateTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(22, 0));
        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, dateTime.DayOfWeek))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _service.IsRestaurantOpenAsync(restaurantId, dateTime);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsRestaurantOpenAsync_Should_Return_False_When_Restaurant_Is_Closed()
    {
        // Arrange
        var restaurantId = 1;
        var dateTime = DateTime.Today.AddDays(1).AddHours(12);

        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, dateTime.DayOfWeek))
            .ReturnsAsync((OperatingHours?)null); // No operating hours = closed

        // Act
        var result = await _service.IsRestaurantOpenAsync(restaurantId, dateTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRestaurantOpenAsync_Should_Return_False_When_Outside_Operating_Hours()
    {
        // Arrange
        var restaurantId = 1;
        var dateTime = DateTime.Today.AddDays(1).AddHours(6); // 6 AM - before opening

        var operatingHours = new OperatingHours(restaurantId, dateTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(22, 0));
        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, dateTime.DayOfWeek))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _service.IsRestaurantOpenAsync(restaurantId, dateTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_Should_Return_Valid_When_All_Checks_Pass()
    {
        // Arrange
        var restaurantId = 1;
        var tableId = 1;
        var startTime = DateTime.Today.AddDays(1).AddHours(12);
        var endTime = startTime.AddHours(3);

        var operatingHours = new OperatingHours(restaurantId, startTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(22, 0));
        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, startTime.DayOfWeek))
            .ReturnsAsync(operatingHours);

        _mockReservationRepository.Setup(r => r.GetConflictingReservationsAsync(tableId, startTime, endTime))
            .ReturnsAsync(new List<Reservation>());

        _mockAvailabilityBlockRepository.Setup(r => r.GetActiveBlocksAsync(restaurantId, startTime))
            .ReturnsAsync(new List<AvailabilityBlock>());

        // Act
        var result = await _service.CheckAvailabilityAsync(restaurantId, tableId, startTime, endTime);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_Should_Return_Invalid_When_Restaurant_Is_Closed()
    {
        // Arrange
        var restaurantId = 1;
        var tableId = 1;
        var startTime = DateTime.Today.AddDays(1).AddHours(12);
        var endTime = startTime.AddHours(3);

        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, startTime.DayOfWeek))
            .ReturnsAsync((OperatingHours?)null);

        // Act
        var result = await _service.CheckAvailabilityAsync(restaurantId, tableId, startTime, endTime);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Restaurant is closed during the requested time", result.ValidationErrors);
    }
}

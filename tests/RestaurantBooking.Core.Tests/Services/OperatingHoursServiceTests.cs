using Moq;
using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Interfaces.Repositories;
using RestaurantBooking.Core.Interfaces.Services;
using RestaurantBooking.Core.Services;

namespace RestaurantBooking.Core.Tests.Services;

public class OperatingHoursServiceTests
{
    private readonly Mock<IOperatingHoursRepository> _mockOperatingHoursRepository;
    private readonly IOperatingHoursService _service;

    public OperatingHoursServiceTests()
    {
        _mockOperatingHoursRepository = new Mock<IOperatingHoursRepository>();
        _service = new OperatingHoursService(_mockOperatingHoursRepository.Object);
    }

    [Fact]
    public async Task ValidateOperatingHoursAsync_Should_Return_Valid_For_Valid_Hours()
    {
        // Arrange
        var restaurantId = 1;
        var dayOfWeek = DayOfWeek.Monday;
        var openTime = new TimeOnly(9, 0);
        var closeTime = new TimeOnly(22, 0);

        // Act
        var result = await _service.ValidateOperatingHoursAsync(restaurantId, dayOfWeek, openTime, closeTime);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public async Task ValidateOperatingHoursAsync_Should_Return_Invalid_For_Invalid_Time_Range()
    {
        // Arrange
        var restaurantId = 1;
        var dayOfWeek = DayOfWeek.Monday;
        var openTime = new TimeOnly(22, 0);  // Opens at 10 PM
        var closeTime = new TimeOnly(9, 0);  // Closes at 9 AM (invalid unless overnight)

        // Act
        var result = await _service.ValidateOperatingHoursAsync(restaurantId, dayOfWeek, openTime, closeTime, false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Close time must be after open time for non-overnight hours", result.ValidationErrors);
    }

    [Fact]
    public async Task ValidateOperatingHoursAsync_Should_Return_Valid_For_Overnight_Hours()
    {
        // Arrange
        var restaurantId = 1;
        var dayOfWeek = DayOfWeek.Friday;
        var openTime = new TimeOnly(22, 0);  // Opens at 10 PM
        var closeTime = new TimeOnly(2, 0);  // Closes at 2 AM next day

        // Act
        var result = await _service.ValidateOperatingHoursAsync(restaurantId, dayOfWeek, openTime, closeTime, true);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public async Task IsWithinOperatingHoursAsync_Should_Return_True_When_Within_Hours()
    {
        // Arrange
        var restaurantId = 1;
        var dateTime = DateTime.Today.AddDays(1).AddHours(12); // Monday at noon
        var operatingHours = new OperatingHours(restaurantId, dateTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(22, 0));

        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, dateTime.DayOfWeek))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _service.IsWithinOperatingHoursAsync(restaurantId, dateTime);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsWithinOperatingHoursAsync_Should_Return_False_When_Outside_Hours()
    {
        // Arrange
        var restaurantId = 1;
        var dateTime = DateTime.Today.AddDays(1).AddHours(6); // Monday at 6 AM - before opening
        var operatingHours = new OperatingHours(restaurantId, dateTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(22, 0));

        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, dateTime.DayOfWeek))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _service.IsWithinOperatingHoursAsync(restaurantId, dateTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateReservationTimeAsync_Should_Return_Valid_When_Within_Operating_Hours()
    {
        // Arrange
        var restaurantId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(18); // Monday at 6 PM
        var operatingHours = new OperatingHours(restaurantId, reservationDateTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(22, 0));

        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, reservationDateTime.DayOfWeek))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _service.ValidateReservationTimeAsync(restaurantId, reservationDateTime);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public async Task ValidateReservationTimeAsync_Should_Return_Invalid_When_Restaurant_Is_Closed()
    {
        // Arrange
        var restaurantId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(12);

        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, reservationDateTime.DayOfWeek))
            .ReturnsAsync((OperatingHours?)null); // No operating hours = closed

        // Act
        var result = await _service.ValidateReservationTimeAsync(restaurantId, reservationDateTime);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Restaurant is closed on", result.ValidationErrors.First());
    }

    [Fact]
    public async Task ValidateReservationTimeAsync_Should_Return_Invalid_When_Reservation_Ends_After_Closing()
    {
        // Arrange
        var restaurantId = 1;
        var reservationDateTime = DateTime.Today.AddDays(1).AddHours(20); // 8 PM reservation
        var operatingHours = new OperatingHours(restaurantId, reservationDateTime.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(21, 0)); // Closes at 9 PM

        _mockOperatingHoursRepository.Setup(r => r.GetByRestaurantAndDayAsync(restaurantId, reservationDateTime.DayOfWeek))
            .ReturnsAsync(operatingHours);

        // Act (3-hour reservation would end at 11 PM, after closing)
        var result = await _service.ValidateReservationTimeAsync(restaurantId, reservationDateTime);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Reservation would extend beyond closing time", result.ValidationErrors);
    }
}

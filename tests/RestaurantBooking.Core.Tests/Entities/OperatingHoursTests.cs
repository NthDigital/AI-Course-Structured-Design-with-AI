using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Tests.Entities;

public class OperatingHoursTests
{
    [Fact]
    public void Should_Create_OperatingHours_When_Given_Valid_Data()
    {
        // Arrange
        var restaurantId = 1;
        var dayOfWeek = DayOfWeek.Monday;
        var openTime = new TimeOnly(9, 0);
        var closeTime = new TimeOnly(22, 0);

        // Act
        var operatingHours = new OperatingHours(restaurantId, dayOfWeek, openTime, closeTime);

        // Assert
        Assert.Equal(restaurantId, operatingHours.RestaurantId);
        Assert.Equal(dayOfWeek, operatingHours.DayOfWeek);
        Assert.Equal(openTime, operatingHours.OpenTime);
        Assert.Equal(closeTime, operatingHours.CloseTime);
        Assert.True(operatingHours.IsOpen);
        Assert.True(operatingHours.Id > 0);
        Assert.True(operatingHours.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_RestaurantId(int invalidRestaurantId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new OperatingHours(invalidRestaurantId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(22, 0)));
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_CloseTime_Before_OpenTime()
    {
        // Arrange
        var openTime = new TimeOnly(22, 0);
        var closeTime = new TimeOnly(9, 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new OperatingHours(1, DayOfWeek.Monday, openTime, closeTime));
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_CloseTime_Equals_OpenTime()
    {
        // Arrange
        var time = new TimeOnly(12, 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new OperatingHours(1, DayOfWeek.Monday, time, time));
    }

    [Fact]
    public void Should_Create_Closed_OperatingHours()
    {
        // Arrange
        var restaurantId = 1;
        var dayOfWeek = DayOfWeek.Sunday;

        // Act
        var operatingHours = OperatingHours.CreateClosed(restaurantId, dayOfWeek);

        // Assert
        Assert.Equal(restaurantId, operatingHours.RestaurantId);
        Assert.Equal(dayOfWeek, operatingHours.DayOfWeek);
        Assert.False(operatingHours.IsOpen);
        Assert.Equal(TimeOnly.MinValue, operatingHours.OpenTime);
        Assert.Equal(TimeOnly.MinValue, operatingHours.CloseTime);
    }

    [Fact]
    public void Should_Update_Hours_When_Given_Valid_Times()
    {
        // Arrange
        var operatingHours = new OperatingHours(1, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(22, 0));
        var newOpenTime = new TimeOnly(10, 0);
        var newCloseTime = new TimeOnly(23, 0);

        // Act
        operatingHours.UpdateHours(newOpenTime, newCloseTime);

        // Assert
        Assert.Equal(newOpenTime, operatingHours.OpenTime);
        Assert.Equal(newCloseTime, operatingHours.CloseTime);
        Assert.True(operatingHours.IsOpen);
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Time_Range()
    {
        // Arrange
        var operatingHours = new OperatingHours(1, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(22, 0));
        var invalidOpenTime = new TimeOnly(23, 0);
        var invalidCloseTime = new TimeOnly(10, 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            operatingHours.UpdateHours(invalidOpenTime, invalidCloseTime));
    }

    [Fact]
    public void Should_Set_To_Closed()
    {
        // Arrange
        var operatingHours = new OperatingHours(1, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(22, 0));

        // Act
        operatingHours.SetClosed();

        // Assert
        Assert.False(operatingHours.IsOpen);
        Assert.Equal(TimeOnly.MinValue, operatingHours.OpenTime);
        Assert.Equal(TimeOnly.MinValue, operatingHours.CloseTime);
    }

    [Fact]
    public void Should_Check_If_Time_Is_Within_Operating_Hours()
    {
        // Arrange
        var operatingHours = new OperatingHours(1, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(22, 0));

        // Act & Assert
        Assert.True(operatingHours.IsWithinOperatingHours(new TimeOnly(12, 0)));
        Assert.True(operatingHours.IsWithinOperatingHours(new TimeOnly(9, 0)));
        Assert.False(operatingHours.IsWithinOperatingHours(new TimeOnly(22, 0)));
        Assert.False(operatingHours.IsWithinOperatingHours(new TimeOnly(8, 59)));
        Assert.False(operatingHours.IsWithinOperatingHours(new TimeOnly(23, 0)));
    }

    [Fact]
    public void Should_Return_False_For_IsWithinOperatingHours_When_Closed()
    {
        // Arrange
        var operatingHours = OperatingHours.CreateClosed(1, DayOfWeek.Sunday);

        // Act & Assert
        Assert.False(operatingHours.IsWithinOperatingHours(new TimeOnly(12, 0)));
    }

    [Fact]
    public void Should_Handle_Overnight_Operating_Hours()
    {
        // Arrange
        var openTime = new TimeOnly(22, 0);
        var closeTime = new TimeOnly(2, 0); // Next day

        // Act
        var operatingHours = new OperatingHours(1, DayOfWeek.Friday, openTime, closeTime, isOvernight: true);

        // Assert
        Assert.Equal(openTime, operatingHours.OpenTime);
        Assert.Equal(closeTime, operatingHours.CloseTime);
        Assert.True(operatingHours.IsOvernight);
    }

    [Fact]
    public void Should_Check_Overnight_Operating_Hours_Correctly()
    {
        // Arrange
        var operatingHours = new OperatingHours(1, DayOfWeek.Friday, new TimeOnly(22, 0), new TimeOnly(2, 0), isOvernight: true);

        // Act & Assert
        Assert.True(operatingHours.IsWithinOperatingHours(new TimeOnly(23, 0)));
        Assert.True(operatingHours.IsWithinOperatingHours(new TimeOnly(1, 0)));
        Assert.False(operatingHours.IsWithinOperatingHours(new TimeOnly(3, 0)));
        Assert.False(operatingHours.IsWithinOperatingHours(new TimeOnly(21, 0)));
    }
}

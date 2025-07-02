using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Tests.Entities;

public class ReservationTests
{
    [Fact]
    public void Should_Create_Reservation_When_Given_Valid_Data()
    {
        // Arrange
        var customerId = 1;
        var restaurantId = 1;
        var tableId = 1;
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var partySize = 4;
        var specialRequests = "Window seat preferred";

        // Act
        var reservation = new Reservation(customerId, restaurantId, tableId, reservationDateTime, partySize, specialRequests);

        // Assert
        Assert.Equal(customerId, reservation.CustomerId);
        Assert.Equal(restaurantId, reservation.RestaurantId);
        Assert.Equal(tableId, reservation.TableId);
        Assert.Equal(reservationDateTime, reservation.ReservationDateTime);
        Assert.Equal(partySize, reservation.PartySize);
        Assert.Equal(specialRequests, reservation.SpecialRequests);
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        Assert.Equal(reservationDateTime.AddHours(3), reservation.EndDateTime);
        Assert.True(reservation.Id > 0);
        Assert.True(reservation.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_CustomerId(int invalidCustomerId)
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Reservation(invalidCustomerId, 1, 1, reservationDateTime, 4, ""));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_RestaurantId(int invalidRestaurantId)
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Reservation(1, invalidRestaurantId, 1, reservationDateTime, 4, ""));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_TableId(int invalidTableId)
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Reservation(1, 1, invalidTableId, reservationDateTime, 4, ""));
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Given_Past_DateTime()
    {
        // Arrange
        var pastDateTime = DateTime.UtcNow.AddHours(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Reservation(1, 1, 1, pastDateTime, 4, ""));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_PartySize(int invalidPartySize)
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Reservation(1, 1, 1, reservationDateTime, invalidPartySize, ""));
    }

    [Fact]
    public void Should_Create_Reservation_With_Null_SpecialRequests()
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);

        // Act
        var reservation = new Reservation(1, 1, 1, reservationDateTime, 4, null);

        // Assert
        Assert.Empty(reservation.SpecialRequests);
    }

    [Fact]
    public void Should_Update_Status_To_Cancelled()
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var reservation = new Reservation(1, 1, 1, reservationDateTime, 4, "");

        // Act
        reservation.UpdateStatus(ReservationStatus.Cancelled);

        // Assert
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }

    [Fact]
    public void Should_Update_Status_To_Completed()
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var reservation = new Reservation(1, 1, 1, reservationDateTime, 4, "");

        // Act
        reservation.UpdateStatus(ReservationStatus.Completed);

        // Assert
        Assert.Equal(ReservationStatus.Completed, reservation.Status);
    }

    [Fact]
    public void Should_Update_Status_To_NoShow()
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var reservation = new Reservation(1, 1, 1, reservationDateTime, 4, "");

        // Act
        reservation.UpdateStatus(ReservationStatus.NoShow);

        // Assert
        Assert.Equal(ReservationStatus.NoShow, reservation.Status);
    }

    [Fact]
    public void Should_Throw_InvalidOperationException_When_Updating_Cancelled_Reservation()
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var reservation = new Reservation(1, 1, 1, reservationDateTime, 4, "");
        reservation.UpdateStatus(ReservationStatus.Cancelled);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            reservation.UpdateStatus(ReservationStatus.Confirmed));
    }

    [Fact]
    public void Should_Update_SpecialRequests()
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);
        var reservation = new Reservation(1, 1, 1, reservationDateTime, 4, "");
        var newSpecialRequests = "Allergy to nuts";

        // Act
        reservation.UpdateSpecialRequests(newSpecialRequests);

        // Assert
        Assert.Equal(newSpecialRequests, reservation.SpecialRequests);
    }

    [Fact]
    public void Should_Calculate_EndDateTime_As_Three_Hours_After_Start()
    {
        // Arrange
        var reservationDateTime = DateTime.UtcNow.AddHours(2);

        // Act
        var reservation = new Reservation(1, 1, 1, reservationDateTime, 4, "");

        // Assert
        Assert.Equal(reservationDateTime.AddHours(3), reservation.EndDateTime);
    }

    [Fact]
    public void Should_Allow_Minimum_One_Hour_Future_Reservation()
    {
        // Arrange
        var futureDateTime = DateTime.UtcNow.AddMinutes(61); // Just over 1 hour

        // Act
        var reservation = new Reservation(1, 1, 1, futureDateTime, 4, "");

        // Assert
        Assert.Equal(futureDateTime, reservation.ReservationDateTime);
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Reservation_Less_Than_One_Hour_Future()
    {
        // Arrange
        var tooSoonDateTime = DateTime.UtcNow.AddMinutes(30); // Only 30 minutes future

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Reservation(1, 1, 1, tooSoonDateTime, 4, ""));
    }
}

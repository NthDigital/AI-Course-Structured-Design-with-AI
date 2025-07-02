using RestaurantBooking.Core.Entities;
using RestaurantBooking.Core.Specifications;

namespace RestaurantBooking.Core.Tests.Specifications;

public class ReservationSpecificationTests
{
    [Fact]
    public void ReservationByRestaurantSpecification_Should_Match_Reservations_For_Restaurant()
    {
        // Arrange
        var restaurantId = 1;
        var specification = new ReservationByRestaurantSpecification(restaurantId);
        
        var matchingReservation = new Reservation(1, restaurantId, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        var nonMatchingReservation = new Reservation(2, 2, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);

        // Act & Assert
        Assert.True(specification.IsSatisfiedBy(matchingReservation));
        Assert.False(specification.IsSatisfiedBy(nonMatchingReservation));
    }

    [Fact]
    public void ReservationByDateRangeSpecification_Should_Match_Reservations_In_Range()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);
        var specification = new ReservationByDateRangeSpecification(startDate, endDate);
        
        var matchingReservation = new Reservation(1, 1, 1, DateTime.Today.AddDays(3).AddHours(12), 4, null);
        var nonMatchingReservation = new Reservation(2, 1, 1, DateTime.Today.AddDays(10).AddHours(12), 4, null);

        // Act & Assert
        Assert.True(specification.IsSatisfiedBy(matchingReservation));
        Assert.False(specification.IsSatisfiedBy(nonMatchingReservation));
    }

    [Fact]
    public void ReservationByStatusSpecification_Should_Match_Reservations_With_Status()
    {
        // Arrange
        var status = ReservationStatus.Confirmed;
        var specification = new ReservationByStatusSpecification(status);
        
        var matchingReservation = new Reservation(1, 1, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        var nonMatchingReservation = new Reservation(2, 1, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        nonMatchingReservation.UpdateStatus(ReservationStatus.Cancelled);

        // Act & Assert
        Assert.True(specification.IsSatisfiedBy(matchingReservation));
        Assert.False(specification.IsSatisfiedBy(nonMatchingReservation));
    }

    [Fact]
    public void ReservationConflictSpecification_Should_Detect_Overlapping_Reservations()
    {
        // Arrange
        var tableId = 1;
        var requestedStart = DateTime.Today.AddDays(1).AddHours(12);
        var requestedEnd = requestedStart.AddHours(3);
        var specification = new ReservationConflictSpecification(tableId, requestedStart, requestedEnd);
        
        // Existing reservation that overlaps
        var conflictingReservation = new Reservation(1, 1, tableId, requestedStart.AddHours(1), 4, null);
        
        // Existing reservation that doesn't overlap
        var nonConflictingReservation = new Reservation(2, 1, tableId, requestedEnd.AddHours(1), 4, null);
        
        // Cancelled reservation (should not conflict)
        var cancelledReservation = new Reservation(3, 1, tableId, requestedStart.AddHours(1), 4, null);
        cancelledReservation.UpdateStatus(ReservationStatus.Cancelled);

        // Act & Assert
        Assert.True(specification.IsSatisfiedBy(conflictingReservation));
        Assert.False(specification.IsSatisfiedBy(nonConflictingReservation));
        Assert.False(specification.IsSatisfiedBy(cancelledReservation));
    }

    [Fact]
    public void Specifications_Can_Be_Combined_Using_And_Operator()
    {
        // Arrange
        var restaurantId = 1;
        var status = ReservationStatus.Confirmed;
        var specification = new ReservationByRestaurantSpecification(restaurantId)
            .And(new ReservationByStatusSpecification(status));
        
        var matchingReservation = new Reservation(1, restaurantId, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        var nonMatchingReservation = new Reservation(2, 2, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null); // Different restaurant
        var anotherNonMatchingReservation = new Reservation(3, restaurantId, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        anotherNonMatchingReservation.UpdateStatus(ReservationStatus.Cancelled); // Different status

        // Act & Assert
        Assert.True(specification.IsSatisfiedBy(matchingReservation));
        Assert.False(specification.IsSatisfiedBy(nonMatchingReservation));
        Assert.False(specification.IsSatisfiedBy(anotherNonMatchingReservation));
    }

    [Fact]
    public void Specifications_Can_Be_Combined_Using_Or_Operator()
    {
        // Arrange
        var confirmedStatus = ReservationStatus.Confirmed;
        var completedStatus = ReservationStatus.Completed;
        var specification = new ReservationByStatusSpecification(confirmedStatus)
            .Or(new ReservationByStatusSpecification(completedStatus));
        
        var confirmedReservation = new Reservation(1, 1, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        var completedReservation = new Reservation(2, 1, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        completedReservation.UpdateStatus(ReservationStatus.Completed);
        var cancelledReservation = new Reservation(3, 1, 1, DateTime.Today.AddDays(1).AddHours(12), 4, null);
        cancelledReservation.UpdateStatus(ReservationStatus.Cancelled);

        // Act & Assert
        Assert.True(specification.IsSatisfiedBy(confirmedReservation));
        Assert.True(specification.IsSatisfiedBy(completedReservation));
        Assert.False(specification.IsSatisfiedBy(cancelledReservation));
    }
}

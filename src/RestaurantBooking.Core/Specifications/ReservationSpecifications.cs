using System.Linq.Expressions;
using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Specifications;

public class ReservationByRestaurantSpecification : Specification<Reservation>
{
    private readonly int _restaurantId;

    public ReservationByRestaurantSpecification(int restaurantId)
    {
        _restaurantId = restaurantId;
    }

    public override Expression<Func<Reservation, bool>> ToExpression()
    {
        return reservation => reservation.RestaurantId == _restaurantId;
    }
}

public class ReservationByDateRangeSpecification : Specification<Reservation>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public ReservationByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<Reservation, bool>> ToExpression()
    {
        return reservation => reservation.ReservationDateTime >= _startDate && 
                             reservation.ReservationDateTime <= _endDate;
    }
}

public class ReservationByStatusSpecification : Specification<Reservation>
{
    private readonly ReservationStatus _status;

    public ReservationByStatusSpecification(ReservationStatus status)
    {
        _status = status;
    }

    public override Expression<Func<Reservation, bool>> ToExpression()
    {
        return reservation => reservation.Status == _status;
    }
}

public class ReservationConflictSpecification : Specification<Reservation>
{
    private readonly int _tableId;
    private readonly DateTime _startTime;
    private readonly DateTime _endTime;

    public ReservationConflictSpecification(int tableId, DateTime startTime, DateTime endTime)
    {
        _tableId = tableId;
        _startTime = startTime;
        _endTime = endTime;
    }

    public override Expression<Func<Reservation, bool>> ToExpression()
    {
        return reservation => reservation.TableId == _tableId &&
                             reservation.Status != ReservationStatus.Cancelled &&
                             reservation.ReservationDateTime < _endTime &&
                             reservation.EndDateTime > _startTime;
    }
}

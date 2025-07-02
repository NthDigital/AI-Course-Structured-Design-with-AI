using System.Linq.Expressions;
using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Specifications;

public class TableByRestaurantSpecification : Specification<Table>
{
    private readonly int _restaurantId;

    public TableByRestaurantSpecification(int restaurantId)
    {
        _restaurantId = restaurantId;
    }

    public override Expression<Func<Table, bool>> ToExpression()
    {
        return table => table.RestaurantId == _restaurantId;
    }
}

public class TableByMinimumCapacitySpecification : Specification<Table>
{
    private readonly int _minimumCapacity;

    public TableByMinimumCapacitySpecification(int minimumCapacity)
    {
        _minimumCapacity = minimumCapacity;
    }

    public override Expression<Func<Table, bool>> ToExpression()
    {
        return table => table.Capacity >= _minimumCapacity;
    }
}

public class TableByStatusSpecification : Specification<Table>
{
    private readonly TableStatus _status;

    public TableByStatusSpecification(TableStatus status)
    {
        _status = status;
    }

    public override Expression<Func<Table, bool>> ToExpression()
    {
        return table => table.Status == _status;
    }
}

public class AvailableTableForReservationSpecification : Specification<Table>
{
    private readonly int _restaurantId;
    private readonly int _minimumCapacity;
    private readonly TableStatus _status;

    public AvailableTableForReservationSpecification(int restaurantId, int minimumCapacity, TableStatus status = TableStatus.Available)
    {
        _restaurantId = restaurantId;
        _minimumCapacity = minimumCapacity;
        _status = status;
    }

    public override Expression<Func<Table, bool>> ToExpression()
    {
        return table => table.RestaurantId == _restaurantId &&
                       table.Capacity >= _minimumCapacity &&
                       table.Status == _status;
    }
}

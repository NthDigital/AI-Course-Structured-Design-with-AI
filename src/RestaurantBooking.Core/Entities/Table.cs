namespace RestaurantBooking.Core.Entities;

public class Table
{
    private static int _nextId = 1;

    public int Id { get; private set; }
    public string TableNumber { get; private set; }
    public int Capacity { get; private set; }
    public int RestaurantId { get; private set; }
    public TableStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Table(string tableNumber, int capacity, int restaurantId)
    {
        ValidateTableNumber(tableNumber);
        ValidateCapacity(capacity);
        ValidateRestaurantId(restaurantId);

        Id = _nextId++;
        TableNumber = tableNumber;
        Capacity = capacity;
        RestaurantId = restaurantId;
        Status = TableStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(TableStatus newStatus)
    {
        Status = newStatus;
    }

    public void UpdateCapacity(int newCapacity)
    {
        ValidateCapacity(newCapacity);
        Capacity = newCapacity;
    }

    private static void ValidateTableNumber(string tableNumber)
    {
        if (string.IsNullOrWhiteSpace(tableNumber))
            throw new ArgumentException("Table number is required.", nameof(tableNumber));
    }

    private static void ValidateCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Table capacity must be greater than zero.", nameof(capacity));
    }

    private static void ValidateRestaurantId(int restaurantId)
    {
        if (restaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be greater than zero.", nameof(restaurantId));
    }
}

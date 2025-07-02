namespace RestaurantBooking.Core.Entities;

public class AvailabilityBlock
{
    private static int _nextId = 1;

    public int Id { get; private set; }
    public int RestaurantId { get; private set; }
    public int? TableId { get; private set; }
    public DateTime StartDateTime { get; private set; }
    public DateTime EndDateTime { get; private set; }
    public string Reason { get; private set; }
    public BlockType BlockType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AvailabilityBlock(int restaurantId, int? tableId, DateTime startDateTime, DateTime endDateTime, string reason)
    {
        ValidateRestaurantId(restaurantId);
        ValidateTableId(tableId);
        ValidateTimeRange(startDateTime, endDateTime);
        ValidateReason(reason);

        Id = _nextId++;
        RestaurantId = restaurantId;
        TableId = tableId;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Reason = reason;
        BlockType = tableId.HasValue ? BlockType.TableMaintenance : BlockType.RestaurantClosure;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePeriod(DateTime startDateTime, DateTime endDateTime)
    {
        ValidateTimeRange(startDateTime, endDateTime);
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }

    public void UpdateReason(string reason)
    {
        ValidateReason(reason);
        Reason = reason;
    }

    public bool ConflictsWith(DateTime startDateTime, DateTime endDateTime)
    {
        return !(endDateTime <= StartDateTime || startDateTime >= EndDateTime);
    }

    public bool IsActiveAt(DateTime dateTime)
    {
        return dateTime >= StartDateTime && dateTime < EndDateTime;
    }

    private static void ValidateRestaurantId(int restaurantId)
    {
        if (restaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be greater than zero.", nameof(restaurantId));
    }

    private static void ValidateTableId(int? tableId)
    {
        if (tableId.HasValue && tableId.Value <= 0)
            throw new ArgumentException("Table ID must be greater than zero when provided.", nameof(tableId));
    }

    private static void ValidateTimeRange(DateTime startDateTime, DateTime endDateTime)
    {
        if (endDateTime <= startDateTime)
            throw new ArgumentException("End time must be after start time.", nameof(endDateTime));
    }

    private static void ValidateReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required.", nameof(reason));
    }
}

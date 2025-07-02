namespace RestaurantBooking.Core.Entities;

public class Reservation
{
    private static int _nextId = 1;

    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public int RestaurantId { get; private set; }
    public int TableId { get; private set; }
    public DateTime ReservationDateTime { get; private set; }
    public DateTime EndDateTime { get; private set; }
    public int PartySize { get; private set; }
    public string SpecialRequests { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Reservation(int customerId, int restaurantId, int tableId, DateTime reservationDateTime, int partySize, string? specialRequests)
    {
        ValidateCustomerId(customerId);
        ValidateRestaurantId(restaurantId);
        ValidateTableId(tableId);
        ValidateReservationDateTime(reservationDateTime);
        ValidatePartySize(partySize);

        Id = _nextId++;
        CustomerId = customerId;
        RestaurantId = restaurantId;
        TableId = tableId;
        ReservationDateTime = reservationDateTime;
        EndDateTime = reservationDateTime.AddHours(3); // 3-hour duration
        PartySize = partySize;
        SpecialRequests = specialRequests ?? string.Empty;
        Status = ReservationStatus.Confirmed;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ReservationStatus newStatus)
    {
        ValidateStatusTransition(newStatus);
        Status = newStatus;
    }

    public void UpdateSpecialRequests(string specialRequests)
    {
        SpecialRequests = specialRequests ?? string.Empty;
    }

    private static void ValidateCustomerId(int customerId)
    {
        if (customerId <= 0)
            throw new ArgumentException("Customer ID must be greater than zero.", nameof(customerId));
    }

    private static void ValidateRestaurantId(int restaurantId)
    {
        if (restaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be greater than zero.", nameof(restaurantId));
    }

    private static void ValidateTableId(int tableId)
    {
        if (tableId <= 0)
            throw new ArgumentException("Table ID must be greater than zero.", nameof(tableId));
    }

    private static void ValidateReservationDateTime(DateTime reservationDateTime)
    {
        var minimumFutureTime = DateTime.UtcNow.AddHours(1);
        if (reservationDateTime <= minimumFutureTime)
            throw new ArgumentException("Reservation must be at least one hour in the future.", nameof(reservationDateTime));
    }

    private static void ValidatePartySize(int partySize)
    {
        if (partySize <= 0)
            throw new ArgumentException("Party size must be greater than zero.", nameof(partySize));
    }

    private void ValidateStatusTransition(ReservationStatus newStatus)
    {
        if (Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Cannot update status of a cancelled reservation.");
        
        if (Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Cannot update status of a completed reservation.");
        
        if (Status == ReservationStatus.NoShow)
            throw new InvalidOperationException("Cannot update status of a no-show reservation.");
    }
}

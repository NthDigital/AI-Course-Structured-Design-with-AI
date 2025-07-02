namespace RestaurantBooking.Core.Entities;

public class OperatingHours
{
    private static int _nextId = 1;

    public int Id { get; private set; }
    public int RestaurantId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly OpenTime { get; private set; }
    public TimeOnly CloseTime { get; private set; }
    public bool IsOpen { get; private set; }
    public bool IsOvernight { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public OperatingHours(int restaurantId, DayOfWeek dayOfWeek, TimeOnly openTime, TimeOnly closeTime, bool isOvernight = false)
    {
        ValidateRestaurantId(restaurantId);
        
        if (!isOvernight)
        {
            ValidateTimeRange(openTime, closeTime);
        }

        Id = _nextId++;
        RestaurantId = restaurantId;
        DayOfWeek = dayOfWeek;
        OpenTime = openTime;
        CloseTime = closeTime;
        IsOpen = true;
        IsOvernight = isOvernight;
        CreatedAt = DateTime.UtcNow;
    }

    private OperatingHours(int restaurantId, DayOfWeek dayOfWeek)
    {
        ValidateRestaurantId(restaurantId);

        Id = _nextId++;
        RestaurantId = restaurantId;
        DayOfWeek = dayOfWeek;
        OpenTime = TimeOnly.MinValue;
        CloseTime = TimeOnly.MinValue;
        IsOpen = false;
        IsOvernight = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static OperatingHours CreateClosed(int restaurantId, DayOfWeek dayOfWeek)
    {
        return new OperatingHours(restaurantId, dayOfWeek);
    }

    public void UpdateHours(TimeOnly openTime, TimeOnly closeTime, bool isOvernight = false)
    {
        if (!isOvernight)
        {
            ValidateTimeRange(openTime, closeTime);
        }

        OpenTime = openTime;
        CloseTime = closeTime;
        IsOpen = true;
        IsOvernight = isOvernight;
    }

    public void SetClosed()
    {
        OpenTime = TimeOnly.MinValue;
        CloseTime = TimeOnly.MinValue;
        IsOpen = false;
        IsOvernight = false;
    }

    public bool IsWithinOperatingHours(TimeOnly time)
    {
        if (!IsOpen)
            return false;

        if (!IsOvernight)
        {
            return time >= OpenTime && time < CloseTime;
        }
        else
        {
            // For overnight hours (e.g., 22:00 to 02:00)
            return time >= OpenTime || time < CloseTime;
        }
    }

    private static void ValidateRestaurantId(int restaurantId)
    {
        if (restaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be greater than zero.", nameof(restaurantId));
    }

    private static void ValidateTimeRange(TimeOnly openTime, TimeOnly closeTime)
    {
        if (closeTime <= openTime)
            throw new ArgumentException("Close time must be after open time.", nameof(closeTime));
    }
}

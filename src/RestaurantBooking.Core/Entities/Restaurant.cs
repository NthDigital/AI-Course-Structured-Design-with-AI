namespace RestaurantBooking.Core.Entities;

public class Restaurant
{
    private static int _nextId = 1;

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string CuisineType { get; private set; }
    public string Description { get; private set; }
    public string Address { get; private set; }
    public string PhoneNumber { get; private set; }
    public int OwnerId { get; private set; }
    public RestaurantStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Restaurant(string name, string cuisineType, string description, string address, string phoneNumber, int ownerId)
    {
        ValidateRequiredFields(name, cuisineType, description, address, phoneNumber);
        ValidateOwnerId(ownerId);
        ValidateNameLength(name);

        Id = _nextId++;
        Name = name;
        CuisineType = cuisineType;
        Description = description;
        Address = address;
        PhoneNumber = phoneNumber;
        OwnerId = ownerId;
        Status = RestaurantStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(RestaurantStatus newStatus)
    {
        ValidateStatusTransition(newStatus);
        Status = newStatus;
    }

    public void UpdateDetails(string name, string description, string address, string phoneNumber)
    {
        ValidateRequiredField(name, nameof(name));
        ValidateRequiredField(description, nameof(description));
        ValidateRequiredField(address, nameof(address));
        ValidateRequiredField(phoneNumber, nameof(phoneNumber));
        ValidateNameLength(name);

        Name = name;
        Description = description;
        Address = address;
        PhoneNumber = phoneNumber;
    }

    private static void ValidateRequiredFields(string name, string cuisineType, string description, string address, string phoneNumber)
    {
        ValidateRequiredField(name, nameof(name));
        ValidateRequiredField(cuisineType, nameof(cuisineType));
        ValidateRequiredField(description, nameof(description));
        ValidateRequiredField(address, nameof(address));
        ValidateRequiredField(phoneNumber, nameof(phoneNumber));
    }

    private static void ValidateRequiredField(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.", fieldName);
    }

    private static void ValidateOwnerId(int ownerId)
    {
        if (ownerId <= 0)
            throw new ArgumentException("Owner ID must be greater than zero.", nameof(ownerId));
    }

    private static void ValidateNameLength(string name)
    {
        if (name.Length < 3)
            throw new ArgumentException("Restaurant name must be at least 3 characters long.", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Restaurant name must not exceed 100 characters.", nameof(name));
    }

    private void ValidateStatusTransition(RestaurantStatus newStatus)
    {
        if (Status == RestaurantStatus.Suspended && newStatus == RestaurantStatus.Active)
            throw new InvalidOperationException("Cannot transition directly from Suspended to Active status.");
    }
}

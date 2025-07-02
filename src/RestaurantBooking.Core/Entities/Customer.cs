using System.Text.RegularExpressions;

namespace RestaurantBooking.Core.Entities;

public class Customer
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new Regex(
        @"^(\+\d{1,3}[\s-]?)?(\(?\d{1,4}\)?[\s-]?)?[\d\s-]{6,}$",
        RegexOptions.Compiled);

    private static int _nextId = 1;

    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Customer(string firstName, string lastName, string email, string phoneNumber)
    {
        ValidateRequiredFields(firstName, lastName, email, phoneNumber);
        ValidateEmail(email);
        ValidatePhoneNumber(phoneNumber);

        Id = _nextId++;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        ValidateEmail(email);
        Email = email;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        ValidatePhoneNumber(phoneNumber);
        PhoneNumber = phoneNumber;
    }

    private static void ValidateRequiredFields(string firstName, string lastName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Email format is invalid.", nameof(email));
    }

    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

        if (!PhoneRegex.IsMatch(phoneNumber))
            throw new ArgumentException("Phone number format is invalid.", nameof(phoneNumber));
    }
}

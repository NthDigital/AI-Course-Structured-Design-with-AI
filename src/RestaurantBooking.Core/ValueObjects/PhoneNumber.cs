using System.Text.RegularExpressions;

namespace RestaurantBooking.Core.ValueObjects;

public readonly record struct PhoneNumber
{
    private static readonly Regex PhoneRegex = new Regex(
        @"^(\+\d{1,3}[\s-]?)?(\(?\d{1,4}\)?[\s-]?)?[\d\s-]{6,}$",
        RegexOptions.Compiled);

    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number is required.", nameof(value));

        if (!PhoneRegex.IsMatch(value))
            throw new ArgumentException("Phone number format is invalid.", nameof(value));

        Value = value;
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    public static implicit operator PhoneNumber(string phoneNumber) => new(phoneNumber);

    public override string ToString() => Value;
}

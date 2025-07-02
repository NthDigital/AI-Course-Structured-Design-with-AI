using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+44 20 7946 0958")]
    [InlineData("(555) 123-4567")]
    [InlineData("555-123-4567")]
    public void Should_Create_PhoneNumber_When_Given_Valid_Number(string validPhone)
    {
        // Act
        var phoneNumber = new PhoneNumber(validPhone);

        // Assert
        Assert.Equal(validPhone, phoneNumber.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("abc")]
    [InlineData("123abc")]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Phone(string invalidPhone)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PhoneNumber(invalidPhone));
    }

    [Fact]
    public void Should_Be_Equal_When_Same_Phone_Number()
    {
        // Arrange
        var phone1 = new PhoneNumber("+1234567890");
        var phone2 = new PhoneNumber("+1234567890");

        // Act & Assert
        Assert.Equal(phone1, phone2);
        Assert.True(phone1 == phone2);
        Assert.False(phone1 != phone2);
        Assert.Equal(phone1.GetHashCode(), phone2.GetHashCode());
    }

    [Fact]
    public void Should_Not_Be_Equal_When_Different_Phone_Number()
    {
        // Arrange
        var phone1 = new PhoneNumber("+1234567890");
        var phone2 = new PhoneNumber("+9876543210");

        // Act & Assert
        Assert.NotEqual(phone1, phone2);
        Assert.False(phone1 == phone2);
        Assert.True(phone1 != phone2);
    }

    [Fact]
    public void Should_Convert_To_String_Implicitly()
    {
        // Arrange
        var phoneValue = "+1234567890";
        var phoneNumber = new PhoneNumber(phoneValue);

        // Act
        string result = phoneNumber;

        // Assert
        Assert.Equal(phoneValue, result);
    }

    [Fact]
    public void Should_Convert_From_String_Implicitly()
    {
        // Arrange
        var phoneValue = "+1234567890";

        // Act
        PhoneNumber phoneNumber = phoneValue;

        // Assert
        Assert.Equal(phoneValue, phoneNumber.Value);
    }

    [Fact]
    public void Should_Return_String_Representation()
    {
        // Arrange
        var phoneValue = "+1234567890";
        var phoneNumber = new PhoneNumber(phoneValue);

        // Act
        var result = phoneNumber.ToString();

        // Assert
        Assert.Equal(phoneValue, result);
    }
}

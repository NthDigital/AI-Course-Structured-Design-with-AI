using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Should_Create_Customer_When_Given_Valid_Data()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "+1234567890";

        // Act
        var customer = new Customer(firstName, lastName, email, phoneNumber);

        // Assert
        Assert.Equal(firstName, customer.FirstName);
        Assert.Equal(lastName, customer.LastName);
        Assert.Equal(email, customer.Email);
        Assert.Equal(phoneNumber, customer.PhoneNumber);
        Assert.True(customer.Id > 0);
        Assert.True(customer.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("", "Doe", "john.doe@example.com", "+1234567890")]
    [InlineData("John", "", "john.doe@example.com", "+1234567890")]
    [InlineData("John", "Doe", "", "+1234567890")]
    [InlineData("John", "Doe", "john.doe@example.com", "")]
    [InlineData(null, "Doe", "john.doe@example.com", "+1234567890")]
    [InlineData("John", null, "john.doe@example.com", "+1234567890")]
    [InlineData("John", "Doe", null, "+1234567890")]
    [InlineData("John", "Doe", "john.doe@example.com", null)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Required_Fields(
        string firstName, string lastName, string email, string phoneNumber)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Customer(firstName, lastName, email, phoneNumber));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("john@")]
    [InlineData("john.doe")]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Email_Format(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Customer("John", "Doe", invalidEmail, "+1234567890"));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abc")]
    [InlineData("123abc")]
    [InlineData("")]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Phone_Format(string invalidPhone)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Customer("John", "Doe", "john.doe@example.com", invalidPhone));
    }

    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+44 20 7946 0958")]
    [InlineData("+33 1 42 86 83 26")]
    [InlineData("(555) 123-4567")]
    [InlineData("555-123-4567")]
    public void Should_Accept_Valid_Phone_Formats(string validPhone)
    {
        // Act
        var customer = new Customer("John", "Doe", "john.doe@example.com", validPhone);

        // Assert
        Assert.Equal(validPhone, customer.PhoneNumber);
    }

    [Fact]
    public void Should_Update_Email_When_Given_Valid_Email()
    {
        // Arrange
        var customer = new Customer("John", "Doe", "john@example.com", "+1234567890");
        var newEmail = "john.newemail@example.com";

        // Act
        customer.UpdateEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, customer.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var customer = new Customer("John", "Doe", "john@example.com", "+1234567890");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => customer.UpdateEmail(invalidEmail));
    }

    [Fact]
    public void Should_Update_Phone_Number_When_Given_Valid_Phone()
    {
        // Arrange
        var customer = new Customer("John", "Doe", "john@example.com", "+1234567890");
        var newPhoneNumber = "+9876543210";

        // Act
        customer.UpdatePhoneNumber(newPhoneNumber);

        // Assert
        Assert.Equal(newPhoneNumber, customer.PhoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-phone")]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Phone(string invalidPhone)
    {
        // Arrange
        var customer = new Customer("John", "Doe", "john@example.com", "+1234567890");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => customer.UpdatePhoneNumber(invalidPhone));
    }

    [Fact]
    public void Should_Generate_Unique_Ids_For_Different_Customers()
    {
        // Arrange & Act
        var customer1 = new Customer("John", "Doe", "john@example.com", "+1234567890");
        var customer2 = new Customer("Jane", "Smith", "jane@example.com", "+9876543210");

        // Assert
        Assert.NotEqual(customer1.Id, customer2.Id);
    }
}

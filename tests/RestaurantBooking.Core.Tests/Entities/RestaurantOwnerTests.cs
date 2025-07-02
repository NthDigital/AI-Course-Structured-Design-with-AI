using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Tests.Entities;

public class RestaurantOwnerTests
{
    [Fact]
    public void Should_Create_RestaurantOwner_When_Given_Valid_Data()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "+1234567890";

        // Act
        var owner = new RestaurantOwner(firstName, lastName, email, phoneNumber);

        // Assert
        Assert.Equal(firstName, owner.FirstName);
        Assert.Equal(lastName, owner.LastName);
        Assert.Equal(email, owner.Email);
        Assert.Equal(phoneNumber, owner.PhoneNumber);
        Assert.True(owner.Id > 0);
        Assert.True(owner.CreatedAt <= DateTime.UtcNow);
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
            new RestaurantOwner(firstName, lastName, email, phoneNumber));
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
            new RestaurantOwner("John", "Doe", invalidEmail, "+1234567890"));
    }

    [Fact]
    public void Should_Generate_Unique_Ids_For_Different_Owners()
    {
        // Arrange & Act
        var owner1 = new RestaurantOwner("John", "Doe", "john@example.com", "+1234567890");
        var owner2 = new RestaurantOwner("Jane", "Smith", "jane@example.com", "+1987654321");

        // Assert
        Assert.NotEqual(owner1.Id, owner2.Id);
    }

    [Fact]
    public void Should_Update_Email_When_Given_Valid_Email()
    {
        // Arrange
        var owner = new RestaurantOwner("John", "Doe", "john@example.com", "+1234567890");
        var newEmail = "john.newemail@example.com";

        // Act
        owner.UpdateEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, owner.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var owner = new RestaurantOwner("John", "Doe", "john@example.com", "+1234567890");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => owner.UpdateEmail(invalidEmail));
    }
}

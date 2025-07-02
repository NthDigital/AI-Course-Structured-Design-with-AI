using RestaurantBooking.Core.ValueObjects;

namespace RestaurantBooking.Core.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Should_Create_Email_When_Given_Valid_Address()
    {
        // Arrange
        var validEmail = "john.doe@example.com";

        // Act
        var email = new Email(validEmail);

        // Assert
        Assert.Equal(validEmail, email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("john@")]
    [InlineData("john.doe")]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Email(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
    }

    [Fact]
    public void Should_Be_Equal_When_Same_Email_Address()
    {
        // Arrange
        var email1 = new Email("john@example.com");
        var email2 = new Email("john@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
        Assert.False(email1 != email2);
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }

    [Fact]
    public void Should_Not_Be_Equal_When_Different_Email_Address()
    {
        // Arrange
        var email1 = new Email("john@example.com");
        var email2 = new Email("jane@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.False(email1 == email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void Should_Convert_To_String_Implicitly()
    {
        // Arrange
        var emailValue = "john@example.com";
        var email = new Email(emailValue);

        // Act
        string result = email;

        // Assert
        Assert.Equal(emailValue, result);
    }

    [Fact]
    public void Should_Convert_From_String_Implicitly()
    {
        // Arrange
        var emailValue = "john@example.com";

        // Act
        Email email = emailValue;

        // Assert
        Assert.Equal(emailValue, email.Value);
    }

    [Fact]
    public void Should_Return_String_Representation()
    {
        // Arrange
        var emailValue = "john@example.com";
        var email = new Email(emailValue);

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal(emailValue, result);
    }

    [Fact]
    public void Should_Handle_Case_Insensitive_Comparison()
    {
        // Arrange
        var email1 = new Email("John@Example.COM");
        var email2 = new Email("john@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }
}

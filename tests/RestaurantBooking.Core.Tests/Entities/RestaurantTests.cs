using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Tests.Entities;

public class RestaurantTests
{
    [Fact]
    public void Should_Create_Restaurant_When_Given_Valid_Data()
    {
        // Arrange
        var name = "The Italian Place";
        var cuisineType = "Italian";
        var description = "Authentic Italian cuisine";
        var address = "123 Main St";
        var phoneNumber = "+1234567890";
        var ownerId = 1;

        // Act
        var restaurant = new Restaurant(name, cuisineType, description, address, phoneNumber, ownerId);

        // Assert
        Assert.Equal(name, restaurant.Name);
        Assert.Equal(cuisineType, restaurant.CuisineType);
        Assert.Equal(description, restaurant.Description);
        Assert.Equal(address, restaurant.Address);
        Assert.Equal(phoneNumber, restaurant.PhoneNumber);
        Assert.Equal(ownerId, restaurant.OwnerId);
        Assert.Equal(RestaurantStatus.Active, restaurant.Status);
        Assert.True(restaurant.Id > 0);
        Assert.True(restaurant.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("", "Italian", "Description", "Address", "+1234567890", 1)]
    [InlineData("Restaurant", "", "Description", "Address", "+1234567890", 1)]
    [InlineData("Restaurant", "Italian", "", "Address", "+1234567890", 1)]
    [InlineData("Restaurant", "Italian", "Description", "", "+1234567890", 1)]
    [InlineData("Restaurant", "Italian", "Description", "Address", "", 1)]
    [InlineData(null, "Italian", "Description", "Address", "+1234567890", 1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Required_Fields(
        string name, string cuisineType, string description, string address, string phoneNumber, int ownerId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Restaurant(name, cuisineType, description, address, phoneNumber, ownerId));
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void Should_Throw_ArgumentException_When_Name_Too_Short(string shortName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Restaurant(shortName, "Italian", "Description", "Address", "+1234567890", 1));
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Name_Too_Long()
    {
        // Arrange
        var longName = new string('A', 101); // 101 characters

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Restaurant(longName, "Italian", "Description", "Address", "+1234567890", 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Invalid_OwnerId(int invalidOwnerId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Restaurant("Restaurant", "Italian", "Description", "Address", "+1234567890", invalidOwnerId));
    }

    [Fact]
    public void Should_Update_Status_To_Inactive()
    {
        // Arrange
        var restaurant = new Restaurant("Restaurant", "Italian", "Description", "Address", "+1234567890", 1);

        // Act
        restaurant.UpdateStatus(RestaurantStatus.Inactive);

        // Assert
        Assert.Equal(RestaurantStatus.Inactive, restaurant.Status);
    }

    [Fact]
    public void Should_Update_Status_To_Suspended()
    {
        // Arrange
        var restaurant = new Restaurant("Restaurant", "Italian", "Description", "Address", "+1234567890", 1);

        // Act
        restaurant.UpdateStatus(RestaurantStatus.Suspended);

        // Assert
        Assert.Equal(RestaurantStatus.Suspended, restaurant.Status);
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Transitioning_From_Suspended_To_Active_Directly()
    {
        // Arrange
        var restaurant = new Restaurant("Restaurant", "Italian", "Description", "Address", "+1234567890", 1);
        restaurant.UpdateStatus(RestaurantStatus.Suspended);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            restaurant.UpdateStatus(RestaurantStatus.Active));
    }

    [Fact]
    public void Should_Allow_Transition_From_Suspended_To_Inactive()
    {
        // Arrange
        var restaurant = new Restaurant("Restaurant", "Italian", "Description", "Address", "+1234567890", 1);
        restaurant.UpdateStatus(RestaurantStatus.Suspended);

        // Act
        restaurant.UpdateStatus(RestaurantStatus.Inactive);

        // Assert
        Assert.Equal(RestaurantStatus.Inactive, restaurant.Status);
    }

    [Fact]
    public void Should_Allow_Reactivation_From_Inactive_To_Active()
    {
        // Arrange
        var restaurant = new Restaurant("Restaurant", "Italian", "Description", "Address", "+1234567890", 1);
        restaurant.UpdateStatus(RestaurantStatus.Inactive);

        // Act
        restaurant.UpdateStatus(RestaurantStatus.Active);

        // Assert
        Assert.Equal(RestaurantStatus.Active, restaurant.Status);
    }

    [Fact]
    public void Should_Update_Restaurant_Details()
    {
        // Arrange
        var restaurant = new Restaurant("Old Name", "Italian", "Old Description", "Old Address", "+1234567890", 1);
        var newName = "New Name";
        var newDescription = "New Description";
        var newAddress = "New Address";
        var newPhoneNumber = "+9876543210";

        // Act
        restaurant.UpdateDetails(newName, newDescription, newAddress, newPhoneNumber);

        // Assert
        Assert.Equal(newName, restaurant.Name);
        Assert.Equal(newDescription, restaurant.Description);
        Assert.Equal(newAddress, restaurant.Address);
        Assert.Equal(newPhoneNumber, restaurant.PhoneNumber);
    }
}

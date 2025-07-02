using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Tests.Entities;

public class AvailabilityBlockTests
{
    [Fact]
    public void Should_Create_AvailabilityBlock_When_Given_Valid_Data()
    {
        // Arrange
        var restaurantId = 1;
        var tableId = 1;
        var startDateTime = DateTime.UtcNow.AddHours(1);
        var endDateTime = DateTime.UtcNow.AddHours(3);
        var reason = "Maintenance";

        // Act
        var block = new AvailabilityBlock(restaurantId, tableId, startDateTime, endDateTime, reason);

        // Assert
        Assert.Equal(restaurantId, block.RestaurantId);
        Assert.Equal(tableId, block.TableId);
        Assert.Equal(startDateTime, block.StartDateTime);
        Assert.Equal(endDateTime, block.EndDateTime);
        Assert.Equal(reason, block.Reason);
        Assert.Equal(BlockType.TableMaintenance, block.BlockType);
        Assert.True(block.Id > 0);
        Assert.True(block.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_RestaurantId(int invalidRestaurantId)
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(1);
        var endDateTime = DateTime.UtcNow.AddHours(3);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new AvailabilityBlock(invalidRestaurantId, 1, startDateTime, endDateTime, "Reason"));
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_EndTime_Before_StartTime()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(3);
        var endDateTime = DateTime.UtcNow.AddHours(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new AvailabilityBlock(1, 1, startDateTime, endDateTime, "Reason"));
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_EndTime_Equals_StartTime()
    {
        // Arrange
        var dateTime = DateTime.UtcNow.AddHours(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new AvailabilityBlock(1, 1, dateTime, dateTime, "Reason"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Reason(string invalidReason)
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(1);
        var endDateTime = DateTime.UtcNow.AddHours(3);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new AvailabilityBlock(1, 1, startDateTime, endDateTime, invalidReason));
    }

    [Fact]
    public void Should_Create_Restaurant_Wide_Block_When_TableId_Is_Null()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(1);
        var endDateTime = DateTime.UtcNow.AddHours(3);

        // Act
        var block = new AvailabilityBlock(1, null, startDateTime, endDateTime, "Restaurant closure");

        // Assert
        Assert.Null(block.TableId);
        Assert.Equal(BlockType.RestaurantClosure, block.BlockType);
    }

    [Fact]
    public void Should_Create_Table_Specific_Block_When_TableId_Is_Provided()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(1);
        var endDateTime = DateTime.UtcNow.AddHours(3);

        // Act
        var block = new AvailabilityBlock(1, 5, startDateTime, endDateTime, "Table maintenance");

        // Assert
        Assert.Equal(5, block.TableId);
        Assert.Equal(BlockType.TableMaintenance, block.BlockType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_TableId(int invalidTableId)
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(1);
        var endDateTime = DateTime.UtcNow.AddHours(3);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new AvailabilityBlock(1, invalidTableId, startDateTime, endDateTime, "Reason"));
    }

    [Fact]
    public void Should_Update_Block_Period()
    {
        // Arrange
        var originalStart = DateTime.UtcNow.AddHours(1);
        var originalEnd = DateTime.UtcNow.AddHours(3);
        var block = new AvailabilityBlock(1, 1, originalStart, originalEnd, "Maintenance");
        
        var newStart = DateTime.UtcNow.AddHours(2);
        var newEnd = DateTime.UtcNow.AddHours(4);

        // Act
        block.UpdatePeriod(newStart, newEnd);

        // Assert
        Assert.Equal(newStart, block.StartDateTime);
        Assert.Equal(newEnd, block.EndDateTime);
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Period()
    {
        // Arrange
        var block = new AvailabilityBlock(1, 1, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3), "Maintenance");
        var invalidStart = DateTime.UtcNow.AddHours(4);
        var invalidEnd = DateTime.UtcNow.AddHours(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            block.UpdatePeriod(invalidStart, invalidEnd));
    }

    [Fact]
    public void Should_Update_Reason()
    {
        // Arrange
        var block = new AvailabilityBlock(1, 1, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3), "Original reason");
        var newReason = "Updated reason";

        // Act
        block.UpdateReason(newReason);

        // Assert
        Assert.Equal(newReason, block.Reason);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Reason(string invalidReason)
    {
        // Arrange
        var block = new AvailabilityBlock(1, 1, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3), "Original reason");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => block.UpdateReason(invalidReason));
    }

    [Fact]
    public void Should_Check_If_DateTime_Conflicts_With_Block()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(2);
        var endDateTime = DateTime.UtcNow.AddHours(4);
        var block = new AvailabilityBlock(1, 1, startDateTime, endDateTime, "Maintenance");

        // Act & Assert
        Assert.True(block.ConflictsWith(startDateTime.AddMinutes(30), startDateTime.AddMinutes(90)));
        Assert.True(block.ConflictsWith(startDateTime.AddMinutes(-30), startDateTime.AddMinutes(30)));
        Assert.True(block.ConflictsWith(endDateTime.AddMinutes(-30), endDateTime.AddMinutes(30)));
        Assert.False(block.ConflictsWith(startDateTime.AddMinutes(-60), startDateTime.AddMinutes(-30)));
        Assert.False(block.ConflictsWith(endDateTime.AddMinutes(30), endDateTime.AddMinutes(60)));
    }

    [Fact]
    public void Should_Check_If_Exact_DateTime_Range_Conflicts()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(2);
        var endDateTime = DateTime.UtcNow.AddHours(4);
        var block = new AvailabilityBlock(1, 1, startDateTime, endDateTime, "Maintenance");

        // Act & Assert
        Assert.True(block.ConflictsWith(startDateTime, endDateTime));
    }

    [Fact]
    public void Should_Check_If_Block_Is_Active_At_Specific_Time()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(2);
        var endDateTime = DateTime.UtcNow.AddHours(4);
        var block = new AvailabilityBlock(1, 1, startDateTime, endDateTime, "Maintenance");

        // Act & Assert
        Assert.True(block.IsActiveAt(startDateTime.AddMinutes(30)));
        Assert.True(block.IsActiveAt(startDateTime));
        Assert.False(block.IsActiveAt(endDateTime));
        Assert.False(block.IsActiveAt(startDateTime.AddMinutes(-30)));
        Assert.False(block.IsActiveAt(endDateTime.AddMinutes(30)));
    }
}

using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Core.Tests.Entities;

public class TableTests
{
    [Fact]
    public void Should_Create_Table_When_Given_Valid_Data()
    {
        // Arrange
        var tableNumber = "T-001";
        var capacity = 4;
        var restaurantId = 1;

        // Act
        var table = new Table(tableNumber, capacity, restaurantId);

        // Assert
        Assert.Equal(tableNumber, table.TableNumber);
        Assert.Equal(capacity, table.Capacity);
        Assert.Equal(restaurantId, table.RestaurantId);
        Assert.Equal(TableStatus.Available, table.Status);
        Assert.True(table.Id > 0);
        Assert.True(table.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("", 4, 1)]
    [InlineData(null, 4, 1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_TableNumber(string invalidTableNumber, int capacity, int restaurantId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Table(invalidTableNumber, capacity, restaurantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_Capacity(int invalidCapacity)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Table("T-001", invalidCapacity, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Given_Invalid_RestaurantId(int invalidRestaurantId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Table("T-001", 4, invalidRestaurantId));
    }

    [Fact]
    public void Should_Update_Status_To_Occupied()
    {
        // Arrange
        var table = new Table("T-001", 4, 1);

        // Act
        table.UpdateStatus(TableStatus.Occupied);

        // Assert
        Assert.Equal(TableStatus.Occupied, table.Status);
    }

    [Fact]
    public void Should_Update_Status_To_Reserved()
    {
        // Arrange
        var table = new Table("T-001", 4, 1);

        // Act
        table.UpdateStatus(TableStatus.Reserved);

        // Assert
        Assert.Equal(TableStatus.Reserved, table.Status);
    }

    [Fact]
    public void Should_Update_Status_To_OutOfService()
    {
        // Arrange
        var table = new Table("T-001", 4, 1);

        // Act
        table.UpdateStatus(TableStatus.OutOfService);

        // Assert
        Assert.Equal(TableStatus.OutOfService, table.Status);
    }

    [Fact]
    public void Should_Update_Status_Back_To_Available()
    {
        // Arrange
        var table = new Table("T-001", 4, 1);
        table.UpdateStatus(TableStatus.Occupied);

        // Act
        table.UpdateStatus(TableStatus.Available);

        // Assert
        Assert.Equal(TableStatus.Available, table.Status);
    }

    [Fact]
    public void Should_Update_Capacity()
    {
        // Arrange
        var table = new Table("T-001", 4, 1);
        var newCapacity = 6;

        // Act
        table.UpdateCapacity(newCapacity);

        // Assert
        Assert.Equal(newCapacity, table.Capacity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Capacity(int invalidCapacity)
    {
        // Arrange
        var table = new Table("T-001", 4, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => table.UpdateCapacity(invalidCapacity));
    }

    [Fact]
    public void Should_Generate_Unique_Ids_For_Different_Tables()
    {
        // Arrange & Act
        var table1 = new Table("T-001", 4, 1);
        var table2 = new Table("T-002", 6, 1);

        // Assert
        Assert.NotEqual(table1.Id, table2.Id);
    }

    [Fact]
    public void Should_Allow_Same_Table_Number_For_Different_Restaurants()
    {
        // Arrange & Act
        var table1 = new Table("T-001", 4, 1);
        var table2 = new Table("T-001", 6, 2);

        // Assert
        Assert.Equal("T-001", table1.TableNumber);
        Assert.Equal("T-001", table2.TableNumber);
        Assert.NotEqual(table1.RestaurantId, table2.RestaurantId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Infrastructure.Data.Configurations;

public class OperatingHoursConfiguration : IEntityTypeConfiguration<OperatingHours>
{
    public void Configure(EntityTypeBuilder<OperatingHours> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.RestaurantId)
            .IsRequired();

        builder.Property(e => e.DayOfWeek)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.OpenTime)
            .IsRequired();

        builder.Property(e => e.CloseTime)
            .IsRequired();

        builder.Property(e => e.IsOpen)
            .IsRequired();

        builder.Property(e => e.IsOvernight)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne<Restaurant>()
            .WithMany()
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.RestaurantId, e.DayOfWeek })
            .IsUnique();
    }
}

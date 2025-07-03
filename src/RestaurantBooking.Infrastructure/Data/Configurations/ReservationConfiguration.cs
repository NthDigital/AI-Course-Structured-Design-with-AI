using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantBooking.Core.Entities;

namespace RestaurantBooking.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.CustomerId)
            .IsRequired();

        builder.Property(e => e.RestaurantId)
            .IsRequired();

        builder.Property(e => e.TableId)
            .IsRequired();

        builder.Property(e => e.ReservationDateTime)
            .IsRequired();

        builder.Property(e => e.EndDateTime)
            .IsRequired();

        builder.Property(e => e.PartySize)
            .IsRequired();

        builder.Property(e => e.SpecialRequests)
            .HasMaxLength(1000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Restaurant>()
            .WithMany()
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Table>()
            .WithMany()
            .HasForeignKey(e => e.TableId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

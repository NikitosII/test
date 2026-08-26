using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Number)
            .IsRequired()
            .HasMaxLength(OrderNumber.MaxLength);

        builder.HasIndex(order => order.Number).IsUnique();

        builder.Property(order => order.SenderCity)
            .IsRequired()
            .HasMaxLength(Order.CityMaxLength);

        builder.Property(order => order.SenderAddress)
            .IsRequired()
            .HasMaxLength(Order.AddressMaxLength);

        builder.Property(order => order.ReceiverCity)
            .IsRequired()
            .HasMaxLength(Order.CityMaxLength);

        builder.Property(order => order.ReceiverAddress)
            .IsRequired()
            .HasMaxLength(Order.AddressMaxLength);

        builder.Property(order => order.Weight)
            .HasPrecision(10, Order.WeightScale);

        builder.Property(order => order.PickupDate)
            .HasColumnType("date");

        builder.Property(order => order.CreatedAt)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(order => order.CreatedAt)
            .HasDatabaseName("IX_Orders_CreatedAt");
    }
}

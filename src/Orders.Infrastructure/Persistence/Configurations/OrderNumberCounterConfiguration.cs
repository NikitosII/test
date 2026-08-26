using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Infrastructure.Persistence.Entities;

namespace Orders.Infrastructure.Persistence.Configurations;

public sealed class OrderNumberCounterConfiguration : IEntityTypeConfiguration<OrderNumberCounter>
{
    public void Configure(EntityTypeBuilder<OrderNumberCounter> builder)
    {
        builder.ToTable("OrderNumberCounters");

        builder.HasKey(counter => counter.Date);

        builder.Property(counter => counter.Date)
            .HasColumnType("date");

        builder.Property(counter => counter.LastSequence)
            .IsRequired();
    }
}

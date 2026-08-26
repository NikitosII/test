namespace Orders.Infrastructure.Persistence.Entities;

public sealed class OrderNumberCounter
{
    public DateOnly Date { get; set; }
    public int LastSequence { get; set; }
}

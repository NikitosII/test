namespace Orders.Application.Exceptions;

public sealed class OrderNotFoundException(Guid id) : Exception($"Заказ {id} не найден.")
{
    public Guid OrderId { get; } = id;
}

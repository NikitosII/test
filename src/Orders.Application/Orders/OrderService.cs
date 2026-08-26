using Orders.Application.Abstractions;
using Orders.Application.Dto;
using Orders.Application.Exceptions;
using Orders.Application.Mapping;

namespace Orders.Application.Orders;

public sealed class OrderService(IOrderRepository repository, IOrderNumberGenerator numberGenerator, TimeProvider timeProvider)
    : IOrderService
{
    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var createdAt = timeProvider.GetUtcNow();
        var number = await numberGenerator.NextAsync(DateOnly.FromDateTime(createdAt.UtcDateTime), cancellationToken);
        var order = request.ToDomain(number, createdAt);

        await repository.AddAsync(order, cancellationToken);
        return order.ToResponse();
    }

    public async Task<IReadOnlyList<OrderListItemResponse>> ListAsync(CancellationToken cancellationToken)
    {
        var orders = await repository.ListAsync(cancellationToken);
        return [.. orders.Select(OrderMappings.ToListItem)];
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new OrderNotFoundException(id);

        return order.ToResponse();
    }
}

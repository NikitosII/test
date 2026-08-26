using Orders.Application.Dto;
using Orders.Application.Exceptions;

namespace Orders.Application.Abstractions;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderListItemResponse>> ListAsync(CancellationToken cancellationToken);
    Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}

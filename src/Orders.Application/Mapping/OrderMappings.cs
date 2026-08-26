using Orders.Application.Dto;
using Orders.Domain.Entities;

namespace Orders.Application.Mapping;

public static class OrderMappings
{
    public static Order ToDomain(this CreateOrderRequest request, string number, DateTimeOffset createdAt) =>
        Order.Create(
            number,
            request.SenderCity!,
            request.SenderAddress!,
            request.ReceiverCity!,
            request.ReceiverAddress!,
            request.Weight!.Value,
            request.PickupDate!.Value,
            createdAt);

    public static OrderResponse ToResponse(this Order order) =>
        new(
            order.Id,
            order.Number,
            order.SenderCity,
            order.SenderAddress,
            order.ReceiverCity,
            order.ReceiverAddress,
            order.Weight,
            order.PickupDate,
            order.CreatedAt);

    public static OrderListItemResponse ToListItem(this Order order) =>
        new(
            order.Id,
            order.Number,
            order.SenderCity,
            order.SenderAddress,
            order.ReceiverCity,
            order.ReceiverAddress,
            order.Weight,
            order.PickupDate,
            order.CreatedAt);
}

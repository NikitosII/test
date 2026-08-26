namespace Orders.Application.Dto;

public sealed record CreateOrderRequest(
    string? SenderCity,
    string? SenderAddress,
    string? ReceiverCity,
    string? ReceiverAddress,
    decimal? Weight,
    DateOnly? PickupDate);

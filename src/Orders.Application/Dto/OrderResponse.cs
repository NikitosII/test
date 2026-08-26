namespace Orders.Application.Dto;

public sealed record OrderResponse(
    Guid Id,
    string Number,
    string SenderCity,
    string SenderAddress,
    string ReceiverCity,
    string ReceiverAddress,
    decimal Weight,
    DateOnly PickupDate,
    DateTimeOffset CreatedAt);

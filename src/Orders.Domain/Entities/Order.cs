using Orders.Domain.Exceptions;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Entities;

/// <summary>
/// Заказ на доставку.
/// </summary>
public sealed class Order
{
    public const int CityMinLength = 2;
    public const int CityMaxLength = 100;
    public const int AddressMinLength = 2;
    public const int AddressMaxLength = 200;

    public const decimal MinWeight = 0m;
    public const decimal MaxWeight = 20_000m;
    public const int WeightScale = 3;

    private Order()
    {
        Number = null!;
        SenderCity = null!;
        SenderAddress = null!;
        ReceiverCity = null!;
        ReceiverAddress = null!;
    }

    private Order(
        Guid id,
        string number,
        string senderCity,
        string senderAddress,
        string receiverCity,
        string receiverAddress,
        decimal weight,
        DateOnly pickupDate,
        DateTimeOffset createdAt)
    {
        Id = id;
        Number = number;
        SenderCity = senderCity;
        SenderAddress = senderAddress;
        ReceiverCity = receiverCity;
        ReceiverAddress = receiverAddress;
        Weight = weight;
        PickupDate = pickupDate;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; }

    public string SenderCity { get; private set; }

    public string SenderAddress { get; private set; }

    public string ReceiverCity { get; private set; }

    public string ReceiverAddress { get; private set; }

    public decimal Weight { get; private set; }

    public DateOnly PickupDate { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Создаёт валидный заказ: строки нормализуются (trim), числовые и датовые инварианты проверяются.
    /// </summary>
    public static Order Create(
        string number,
        string senderCity,
        string senderAddress,
        string receiverCity,
        string receiverAddress,
        decimal weight,
        DateOnly pickupDate,
        DateTimeOffset createdAt)
    {
        if (!OrderNumber.IsValid(number))
        {
            throw new DomainException($"Номер заказа '{number}' не соответствует формату ORD-yyyyMMdd-NNNN.");
        }

        var today = DateOnly.FromDateTime(createdAt.UtcDateTime);

        if (pickupDate < today)
        {
            throw new DomainException("Дата забора груза не может быть раньше сегодняшнего дня.");
        }

        return new Order(
            Guid.CreateVersion7(),
            number,
            NormalizeCity(senderCity, "Город отправителя"),
            NormalizeAddress(senderAddress, "Адрес отправителя"),
            NormalizeCity(receiverCity, "Город получателя"),
            NormalizeAddress(receiverAddress, "Адрес получателя"),
            NormalizeWeight(weight),
            pickupDate,
            createdAt);
    }

    private static string NormalizeCity(string value, string fieldName) =>
        NormalizeText(value, fieldName, CityMinLength, CityMaxLength);

    private static string NormalizeAddress(string value, string fieldName) =>
        NormalizeText(value, fieldName, AddressMinLength, AddressMaxLength);

    private static string NormalizeText(string value, string fieldName, int minLength, int maxLength)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (trimmed.Length < minLength || trimmed.Length > maxLength)
        {
            throw new DomainException($"{fieldName}: длина должна быть от {minLength} до {maxLength} символов.");
        }

        return trimmed;
    }

    private static decimal NormalizeWeight(decimal value)
    {
        if (value <= MinWeight || value > MaxWeight)
        {
            throw new DomainException($"Вес груза должен быть больше 0 и не больше {MaxWeight:0} кг.");
        }

        var rounded = decimal.Round(value, WeightScale, MidpointRounding.ToZero);

        if (rounded != value)
        {
            throw new DomainException($"Вес груза может содержать не более {WeightScale} знаков после запятой.");
        }

        return rounded;
    }
}

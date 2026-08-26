using FluentValidation;
using Orders.Application.Dto;
using Orders.Domain.Entities;

namespace Orders.Application.Validation;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    private readonly TimeProvider _timeProvider;

    public CreateOrderRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        _timeProvider = timeProvider;

        City(x => x.SenderCity, "Город отправителя");
        Address(x => x.SenderAddress, "Адрес отправителя");
        City(x => x.ReceiverCity, "Город получателя");
        Address(x => x.ReceiverAddress, "Адрес получателя");

        RuleFor(x => x.Weight)
            .NotNull().WithMessage("Вес груза обязателен.")
            .GreaterThan(Order.MinWeight).WithMessage("Вес груза должен быть больше 0.")
            .LessThanOrEqualTo(Order.MaxWeight).WithMessage($"Вес груза не может превышать {Order.MaxWeight:0} кг.")
            .Must(HasAllowedScale).WithMessage($"Вес груза может содержать не более {Order.WeightScale} знаков после запятой.");

        RuleFor(x => x.PickupDate)
            .NotNull().WithMessage("Дата забора груза обязательна.")
            .Must(NotInThePast).WithMessage("Дата забора груза не может быть раньше сегодняшнего дня.");
    }

    private void City(
        System.Linq.Expressions.Expression<Func<CreateOrderRequest, string?>> selector,
        string fieldName) =>
        Text(selector, fieldName, Order.CityMinLength, Order.CityMaxLength);

    private void Address(
        System.Linq.Expressions.Expression<Func<CreateOrderRequest, string?>> selector,
        string fieldName) =>
        Text(selector, fieldName, Order.AddressMinLength, Order.AddressMaxLength);

    private void Text(
        System.Linq.Expressions.Expression<Func<CreateOrderRequest, string?>> selector,
        string fieldName,
        int minLength,
        int maxLength) =>
        RuleFor(selector)
            .NotEmpty().WithMessage($"{fieldName}: поле обязательно для заполнения.")
            .Must(value => value is null || value.Trim().Length >= minLength)
                .WithMessage($"{fieldName}: минимум {minLength} символа.")
            .Must(value => value is null || value.Trim().Length <= maxLength)
                .WithMessage($"{fieldName}: максимум {maxLength} символов.");

    private static bool HasAllowedScale(decimal? value) =>
        value is null || decimal.Round(value.Value, Order.WeightScale, MidpointRounding.ToZero) == value.Value;

    private bool NotInThePast(DateOnly? value) =>
        value is null || value.Value >= DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
}

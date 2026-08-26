using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orders.Application.Abstractions;
using Orders.Application.Orders;
using Orders.Application.Validation;

namespace Orders.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IOrderService, OrderService>();
        services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>(ServiceLifetime.Scoped);

        return services;
    }
}

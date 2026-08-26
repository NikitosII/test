namespace Orders.Application.Abstractions;

public interface IOrderNumberGenerator
{
    Task<string> NextAsync(DateOnly date, CancellationToken cancellationToken);
}

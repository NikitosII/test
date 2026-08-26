namespace Orders.Domain.Exceptions;

/// <summary>
/// Нарушение доменного инварианта. 
/// </summary>
public sealed class DomainException(string message) : Exception(message);

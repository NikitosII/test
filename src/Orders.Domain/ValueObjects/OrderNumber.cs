using System.Globalization;
using System.Text.RegularExpressions;

namespace Orders.Domain.ValueObjects;

/// <summary>
/// Формат номера заказа <c>ORD-yyyyMMdd-NNNN</c>: дата создания и порядковый номер в рамках дня.
/// </summary>
public static partial class OrderNumber
{
    public const int MaxLength = 32;

    private const int SequenceDigits = 4;

    public static string Format(DateOnly date, int sequence)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sequence, 1);

        var suffix = sequence.ToString($"D{SequenceDigits}", CultureInfo.InvariantCulture);

        return string.Create(CultureInfo.InvariantCulture, $"ORD-{date:yyyyMMdd}-{suffix}");
    }

    public static bool IsValid(string value) => Pattern().IsMatch(value);

    [GeneratedRegex(@"^ORD-\d{8}-\d{4,}$")]
    private static partial Regex Pattern();
}

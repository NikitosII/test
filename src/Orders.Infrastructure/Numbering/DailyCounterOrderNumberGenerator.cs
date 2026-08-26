using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Orders.Application.Abstractions;
using Orders.Domain.ValueObjects;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Numbering;

/// <summary>
/// Выдаёт порядковый номер дня одним атомарным UPSERT'ом с <c>RETURNING</c>.
/// </summary>
public sealed class DailyCounterOrderNumberGenerator(AppDbContext context) : IOrderNumberGenerator
{
    private const string NextSequenceSql = """
        INSERT INTO "OrderNumberCounters" ("Date", "LastSequence")
        VALUES (@date, 1)
        ON CONFLICT ("Date")
        DO UPDATE SET "LastSequence" = "OrderNumberCounters"."LastSequence" + 1
        RETURNING "LastSequence";
        """;

    public async Task<string> NextAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var sequence = await NextSequenceAsync(date, cancellationToken);

        return OrderNumber.Format(date, sequence);
    }

    private async Task<int> NextSequenceAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var database = context.Database;

        await database.OpenConnectionAsync(cancellationToken);

        try
        {
            await using var command = database.GetDbConnection().CreateCommand();

            command.CommandText = NextSequenceSql;
            command.Transaction = database.CurrentTransaction?.GetDbTransaction();
            command.Parameters.Add(CreateDateParameter(command, date));

            var result = await command.ExecuteScalarAsync(cancellationToken);

            return result is int sequence
                ? sequence
                : throw new InvalidOperationException("Счётчик номеров заказа не вернул значение.");
        }
        finally
        {
            await database.CloseConnectionAsync();
        }
    }

    private static DbParameter CreateDateParameter(DbCommand command, DateOnly date)
    {
        var parameter = command.CreateParameter();

        parameter.ParameterName = "date";
        parameter.Value = date;

        return parameter;
    }
}

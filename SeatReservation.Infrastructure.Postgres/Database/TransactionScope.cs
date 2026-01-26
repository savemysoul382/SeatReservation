// SeatReservation.Infrastructure.Postgres

using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.DataBase;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Database;

public class TransactionScope : ITransactionScope
{
    private readonly IDbTransaction _transaction;

    public TransactionScope(IDbTransaction transaction, ILogger<TransactionScope> logger)
    {
        _transaction = transaction;
    }

    public UnitResult<Error> Commit()
    {
        try
        {
            _transaction.Commit();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            return Error.Failure("database", "Failed to commit transaction: " + ex.Message);
        }
    }

    public UnitResult<Error> Rollback()
    {
        try
        {
            _transaction.Rollback();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            return Error.Failure("database", "Failed to rollback transaction: " + ex.Message);
        }
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}
// SeatReservation.Infrastructure.Postgres

using System.Data;
using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Application.DataBase;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken ct = default, IsolationLevel? level = null);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken ct);
}
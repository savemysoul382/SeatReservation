// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Application.DataBase;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken ct);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken ct);
}
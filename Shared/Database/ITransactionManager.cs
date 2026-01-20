// DevQuestions.Application

using System.Data;

namespace Shared.Database;

/// <summary>
/// Unit of work.
/// </summary>
public interface ITransactionManager
{
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
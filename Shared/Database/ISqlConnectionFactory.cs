// DevQuestions.Infrastructure.Postgres

using System.Data;

namespace Shared.Database;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}
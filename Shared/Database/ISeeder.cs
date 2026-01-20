// DevQuestions.Infrastructure.Postgres

namespace Shared.Database;

public interface ISeeder
{
    Task SeedAsync();
}
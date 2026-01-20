// DevQuestions.Application

using System.Text.Json;

namespace Shared.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(Error[] errors)
        : base(JsonSerializer.Serialize(value: errors))
    {
    }
}
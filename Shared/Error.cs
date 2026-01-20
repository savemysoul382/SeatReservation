using System.Text.Json.Serialization;

namespace Shared
{
    public record Error
    {
        public static Error None { get; } = new Error(code: string.Empty, message: string.Empty, type: ErrorType.NONE, null);

        public string Code { get; }
        public string Message { get; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ErrorType Type { get; }

        public string? InvalidField { get; }

        [JsonConstructor]
        private Error(string code, string message, ErrorType type, string? invalidField = null)
        {
            Code = code;
            Message = message;
            Type = type;
            InvalidField = invalidField;
        }

        public static Error NotFound(string message, Guid? id = null)
        {
            return new Error("record.not.found", message: message, type: ErrorType.NOT_FOUND);
        }

        public static Error Validation(string? code, string message, string? invalidField = null)
        {
            return new Error(code ?? "value.is.invalid", message: message, type: ErrorType.VALIDATION, invalidField: invalidField);
        }

        public static Error Conflict(string? code, string message)
        {
            return new Error(code: code ?? "value.is.conflict", message: message, type: ErrorType.CONFLICT);
        }

        public static Error Failure(string? code, string message)
        {
            return new Error(code: code ?? "failure", message: message, type: ErrorType.FAILURE);
        }

        public Failure ToFailure()
        {
            return new Failure(this);
        }
    }
}
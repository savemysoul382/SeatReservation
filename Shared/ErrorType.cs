// Shared

namespace Shared;

public enum ErrorType
{
    /// <summary>
    /// Неизвестная ошибка
    /// </summary>
    NONE,

    /// <summary>
    /// Ошбика валидации
    /// </summary>
    VALIDATION,

    /// <summary>
    /// Ошибка ничего не найдено
    /// </summary>
    NOT_FOUND,

    /// <summary>
    /// Ошибка сервера
    /// </summary>
    FAILURE,

    /// <summary>
    /// Ошибка конфликт
    /// </summary>
    CONFLICT,
}
namespace Infrastructure.AppComponents.AppExceptions;

/// <summary>
/// Базовый класс исключений в Модели пользователя (покупателя).
/// </summary>
public class AdminModelException : AppException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AdminModelException(string? message = null, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
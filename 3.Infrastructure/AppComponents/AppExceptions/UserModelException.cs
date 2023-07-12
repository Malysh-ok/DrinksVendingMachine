namespace Infrastructure.AppComponents.AppExceptions;

/// <summary>
/// Базовый класс исключений в Модели пользователя (покупателя).
/// </summary>
public class UserModelException : AppException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public UserModelException(string? message = null, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
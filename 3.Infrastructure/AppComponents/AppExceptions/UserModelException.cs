namespace Infrastructure.AppComponents.AppExceptions;

/// <summary>
/// Базовый класс исключений в Модели покупателя.
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
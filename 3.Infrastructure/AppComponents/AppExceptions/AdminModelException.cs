namespace Infrastructure.AppComponents.AppExceptions;

/// <summary>
/// Базовый класс исключений в Модели покупателя.
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
namespace Infrastructure.AppComponents.AppExceptions.UserModelExceptions;

/// <summary>
/// Исключение при обновлении данных Модели.
/// </summary>
public class UserModelDataUpdateException : UserModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public UserModelDataUpdateException(string? message = "Ошибка обновления данных Модели.",
        Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
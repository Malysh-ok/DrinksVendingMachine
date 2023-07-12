namespace Infrastructure.AppComponents.AppExceptions.AdminModelExceptions;

/// <summary>
/// Исключение при обновлении данных Модели.
/// </summary>
public class AdminModelDataUpdateException : AdminModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AdminModelDataUpdateException(string? message = "Ошибка обновления данных Модели",
        Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
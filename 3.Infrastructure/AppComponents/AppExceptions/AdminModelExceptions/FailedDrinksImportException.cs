namespace Infrastructure.AppComponents.AppExceptions.AdminModelExceptions;

/// <summary>
/// Исключение при неудачном импорта напитков.
/// </summary>
public class FailedDrinksImportException : AdminModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public FailedDrinksImportException(string? message = "Неудачный импорт напитков.",
        Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
namespace Infrastructure.AppComponents.AppExceptions.UserModelExceptions;

/// <summary>
/// Исключение при нехватке денег.
/// </summary>
public class NotEnoughMoneyException : UserModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public NotEnoughMoneyException(string? message = "Не хватает денег для покупки напитка.",
        Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
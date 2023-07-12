namespace Infrastructure.AppComponents.AppExceptions.UserModelExceptions;

/// <summary>
/// Исключение, если в автомате закончились деньги.
/// </summary>
public class NoMoneyLeftException : UserModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public NoMoneyLeftException(string? message = "В автомате закончились деньги.",
        Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
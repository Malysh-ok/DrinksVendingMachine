namespace Infrastructure.AppComponents.AppExceptions.AdminModelExceptions;

/// <summary>
/// Исключение, если напиток с таким названием уже существует.
/// </summary>
public class DrinkAlreadyExistsException : AdminModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public DrinkAlreadyExistsException(string? message = "Напиток с таким названием уже существует.",
        Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
namespace Infrastructure.AppComponents.AppExceptions.AdminModelExceptions;

/// <summary>
/// Исключение, если напитка с таким названием не существует.
/// </summary>
public class DrinkNotExistsException : AdminModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public DrinkNotExistsException(string? message = "Напитка с таким названием не существует.",
        Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
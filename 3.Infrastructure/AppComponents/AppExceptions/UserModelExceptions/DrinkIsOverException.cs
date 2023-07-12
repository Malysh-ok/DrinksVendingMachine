namespace Infrastructure.AppComponents.AppExceptions.UserModelExceptions;

/// <summary>
/// Исключение, если в автомате закончился напиток.
/// </summary>
public class DrinkIsOverException : UserModelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public DrinkIsOverException(string? subMessage = null,
        Exception? innerException = null) 
        : base($"Напиток \"{subMessage ?? string.Empty}\" закончился.", innerException)
    {
    }
}
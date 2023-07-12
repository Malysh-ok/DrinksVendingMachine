namespace Infrastructure.AppComponents.AppExceptions;

public class AppException : Exception
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AppException() 
    {
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AppException(string? message) 
        : base(message)
    {
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AppException(string? message, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
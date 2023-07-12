namespace Domain.Models;

public class ErrorModel
{
    /// <summary>
    /// Текст ошибки.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Трассировка стека.
    /// </summary>
    public string? StackTrace { get; set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="message">Текст ошибки.</param>
    /// <param name="stackTrace">Трассировка стека.</param>
    public ErrorModel(string? message = null, string? stackTrace = null)
    {
        Message = message;
        StackTrace = stackTrace;
    }
}
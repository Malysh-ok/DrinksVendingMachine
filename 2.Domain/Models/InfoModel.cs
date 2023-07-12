namespace Domain.Models;

/// <summary>
/// Модель для частичного Представление _Info.
/// </summary>
public class InfoModel
{
    /// <summary>
    /// Признак того, что тип сообщения - ошибка.
    /// </summary>
    public bool IsErrorMsg = false;

    /// <summary>
    /// Сообщение Представлению.
    /// </summary>
    public string Msg = string.Empty;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public InfoModel()
    {
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="msg">Признак того, что тип сообщения - ошибка.</param>
    /// <param name="isErrorMsg">Сообщение Представлению.</param>
    public InfoModel(string msg, bool isErrorMsg = false)
    {
        Msg = msg;
        IsErrorMsg = isErrorMsg;
    }
}
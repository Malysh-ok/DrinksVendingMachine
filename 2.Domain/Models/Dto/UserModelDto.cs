namespace Domain.Models.Dto;

/// <summary>
/// Класс, предназначенный для передачи данных из главной Модели пользователя Контроллеру UserController.
/// </summary>
public class UserModelDto
{
    /// <summary>
    /// Сдача.
    /// </summary>
    public string Change { get; }
    
    /// <summary>
    /// Название напитка.
    /// </summary>
    public string DrinkName  { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="change">Сдача.</param>
    /// <param name="drinkName">Название напитка.</param>
    public UserModelDto(string change, string drinkName)
    {
        Change = change;
        DrinkName = drinkName;
    }
}
using App.Infrastructure.Authorization.Entities;

namespace App.Main.Controllers.Dto;

/// <summary>
/// Класс, предназначенный для передачи данных между Контроллером <see cref="LoginController"/>
/// и Представлением.
/// </summary>
public class LoginControllerDto
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    public Personality? User { get; set; }

    /// <summary>
    /// Признак того, что JWT-токен передается посредством строки запроса URL.
    /// </summary>
    public bool IsJwtInAddressBarParams { get; set; }

    /// <summary>
    /// Конструктор для MVC.
    /// </summary>
    public LoginControllerDto()
    {
        User = new Personality();
        IsJwtInAddressBarParams = false;
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <param name="isJwtInAddressBarParams">Признак того,
    /// что JWT-токен передается посредством строки запроса URL.</param>
    public LoginControllerDto(Personality? user, bool isJwtInAddressBarParams)
    {
        User = user;
        IsJwtInAddressBarParams = isJwtInAddressBarParams;
    }

    /// <summary>
    /// Извлечение из текущего экземпляра класса сущности <see cref="User"/>
    /// и логического значения <see cref="IsJwtInAddressBarParams"/>
    /// (в виде кортежа).
    /// </summary>
    public (Personality? user, bool isJwtInQuery) ExtractData()
    {
        return (user: User, isJwtInQuery: IsJwtInAddressBarParams);
    }
}
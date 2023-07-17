using System.ComponentModel.DataAnnotations;
using App.Authorization.Entities;

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
    /// Признак того, что JWS-токен передается через параметры в адресной строке.
    /// </summary>
    public bool IsJwsInAddressBarParams { get; set; }

    /// <summary>
    /// Конструктор для MVC.
    /// </summary>
    public LoginControllerDto()
    {
        User = new Personality();
        IsJwsInAddressBarParams = false;
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isJwsInAddressBarParams"></param>
    public LoginControllerDto(Personality? user, bool isJwsInAddressBarParams)
    {
        User = user;
        IsJwsInAddressBarParams = isJwsInAddressBarParams;
    }

    /// <summary>
    /// Извлечение из текущего экземпляра класса сущности <see cref="User"/>
    /// и логического значения <see cref="LoginControllerDto.IsJwsInAddressBarParams"/>
    /// (в виде кортежа).
    /// </summary>
    public (Personality? user, bool isJwsInQuery) ExtractData()
    {
        return (user: User, isJwsInQuery: IsJwsInAddressBarParams);
    }
}
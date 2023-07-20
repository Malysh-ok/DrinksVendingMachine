using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using App.Infrastructure.Authorization.Entities;
using Microsoft.AspNetCore.Http;

namespace App.Infrastructure.Authorization.Models;

/// <summary>
/// Модель для работы с аутентификацией.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Время жизни JWT-токена в минутах.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")] 
    private const int JWT_LIFETIME = 5;

    /// <summary>
    /// JWT-токен.
    /// </summary>
    public JwtSecurityToken? Jwt { get; private set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="expires">Время окончания жизни токена.</param>
    public LoginModel()
    {
        Jwt = null;
    }

    /// <summary>
    /// Получить привилегированного пользователя без пароля.
    /// </summary>
    public Personality? GetSuperuserWithoutPassword() =>
        LoginManager.GetFirstSuperuserWithoutPassword();

    /// <summary>
    /// Признак того, что пользователь авторизован.
    /// </summary>
    /// <param name="person">Пользователь.</param>
    public bool IsAuthorization(Personality? person)
    {
        var realPerson = LoginManager.GetSuperuser(person?.Name);
        return realPerson is not null && realPerson.Password!.Equals(person?.Password);
    }
    
    #region [----- Работа с признаком того, что аутенификация по параметрам в адресной строке -----]

    /// <summary>
    /// Признак того, что JWT-токен передается через параметры запроса (query).
    /// </summary>
    private bool? _jwtInQueryFlag;
    
    /// <inheritdoc cref="LoginManager.GetJwtInQueryFlag"/>
    public bool GetJwtInQueryFlag(HttpContext context) => 
        _jwtInQueryFlag ?? LoginManager.GetJwtInQueryFlag(context);
    
    /// <inheritdoc cref="LoginManager.SetJwtInQueryFlag"/>
    public void SetJwtInQueryFlag(bool value, HttpContext context)
    {
        _jwtInQueryFlag = value;
        LoginManager.SetJwtInQueryFlag(value, context);
    }

    #endregion
    
    #region [----- Работа с JWT-токеном -----]

    /// <inheritdoc cref="LoginManager.CreateJwt(DateTime)"/>
    private JwtSecurityToken CreateJwt(DateTime? expires = null)
    {
        var jwt = LoginManager.CreateJwt(expires ?? DateTime.UtcNow.AddMinutes(JWT_LIFETIME));
        return jwt;
    }

    /// <summary>
    /// Сделать невалидным Jwt-токен.
    /// </summary>
    /// <remarks>
    /// Фактически он остается валидным, просто удаляется кука, его содержащая.
    /// </remarks>
    public void CancelJwt(HttpContext context)
    {
        Jwt = null;
        LoginManager.RemoveJwtStr(context);
    }
    
    /// <summary>
    /// Получить сериализованный JWT-токен.
    /// </summary>
    public string? GetJwtStr() => Jwt.SerializeJwt();

    /// <summary>
    /// Признак того, что JWT-токен валиден.
    /// </summary>
    /// <remarks>
    /// Если признак того, что JWT-токен передается через параметры запроса (query) УСТАНОВЛЕН,
    /// проверяем <paramref name="jwtStr"/>, иначе - токен, записанный в заголовке HTTP-запроса.
    /// </remarks>
    public bool IsValidJwtStr(HttpContext context, string jwtStr)
    {
        return LoginManager.IsValidJwtStr(GetJwtInQueryFlag(context)
            ? jwtStr
            : LoginManager.GetJwtFromHeader(context)
        );
    }
    
    /// <summary>
    /// Загрузить JWT-токен из хранилища (кук).
    /// </summary>
    public string? LoadJwtStr(HttpContext context)
    {
        return LoginManager.GetJwtStr(context);
    }

    /// <summary>
    /// Обновить и сохранить в хранилище (куках) JWT-токен.
    /// </summary>
    public void UpdateAndSaveJwt(HttpContext context)
    {
        // Обновляем токен
        Jwt = CreateJwt();
        
        // Сохраняем токен в куках
        Jwt.SaveJwt(context,
            TimeSpan.FromSeconds(Jwt.ValidToLocal().Subtract(DateTime.Now).TotalSeconds));
    }
    
    #endregion
}
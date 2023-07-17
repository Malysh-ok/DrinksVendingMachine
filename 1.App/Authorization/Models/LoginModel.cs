using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using App.Authorization.Entities;
using Microsoft.AspNetCore.Http;

namespace App.Authorization.Models;

/// <summary>
/// Модель для работы с аутентификацией.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Время жизни JWT-токена в минутах.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")] 
    private const int JWT_LIFETIME = 10;

    /// <summary>
    /// JWT-токен.
    /// </summary>
    public JwtSecurityToken Jwt { get; private set; }
    
    /// <summary>
    /// Сериализованный JWT-токен.
    /// </summary>
    public string JwtStr => Jwt.SerializeJwt();

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="expires"></param>
    public LoginModel(DateTime? expires = null)
    {
        Jwt = UpdateJws(expires);
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

    /// <inheritdoc cref="LoginManager.GetJwsInQueryFlag"/>
    public bool GetJwsInQueryFlag(HttpContext context) => 
        LoginManager.GetJwsInQueryFlag(context);
    
    /// <inheritdoc cref="LoginManager.SetJwsInQueryFlag"/>
    public void SetJwsInQueryFlag(bool value, HttpContext context) => 
        LoginManager.SetJwsInQueryFlag(value, context);
   
    #endregion
    
    #region [----- Работа с JWT-токеном -----]

    /// <summary>
    /// Обновить JWS-токен.
    /// </summary>
    /// <param name="expires">Время окончания жизни токена.</param>
    private JwtSecurityToken UpdateJws(DateTime? expires = null)
    {
        Jwt = LoginManager.CreateJwt(expires ?? DateTime.Now.AddMinutes(JWT_LIFETIME));
        return Jwt;
    }

    /// <summary>
    /// Сделать невалидным Jws-токен.
    /// </summary>
    /// <remarks>
    /// Фактически он остается валидным, просто удаляется кука, его содержащая.
    /// </remarks>
    public void CancelJws(HttpContext context)
    {
        LoginManager.RemoveJwtStr(context);
    }

    /// <summary>
    /// Признак того, что JWT-токен валиден.
    /// </summary>
    /// <remarks>
    /// Если признак того, что JWS-токен передается через параметры в адресной строке УСТАНОВЛЕН,
    /// проверяем не <paramref name="jwtStr"/>, а токен, записанный в заголовке HTTP-запроса.
    /// </remarks>
    public bool IsValidJwtStr(HttpContext context, string jwtStr)
    {
        return LoginManager.IsValidJwtStr(GetJwsInQueryFlag(context)
            ? jwtStr
            : LoginManager.GetJwtFromHeader(context)
        );
    }
    
    /// <summary>
    /// Обновить и сохранить в хранилище (куках) JWS-токен.
    /// </summary>
    public void UpdateAndSaveJws(HttpContext context)
    {
        // Обновляем токен
        UpdateJws();
        
        // Сохраняем токен в куках
        LoginManager.SetJwtStr(JwtStr, context,
            TimeSpan.FromSeconds(Jwt.Expires()
                .Subtract(DateTime.Now).TotalSeconds));
    }
    
    #endregion
}
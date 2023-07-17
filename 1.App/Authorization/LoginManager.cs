using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using App.Authorization.Entities;
using App.Authorization.Models;
using Domain.Entities;
using Infrastructure.BaseExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace App.Authorization;

/// <summary>
/// Статический класс для работы с аутентификацией.
/// </summary>
public static class LoginManager
{
    /// <summary>
    /// Название куки, куда мы сохраняем JWT-токен.
    /// </summary>
    /// <remarks>
    /// Для наглядности. Лучше называть абракадаброй.
    /// </remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public const string JWT_COOKIE_NAME = "JWT";
    
    /// <summary>
    /// Название куки, куда мы сохраняем признак того,
    /// что JWS-токен передается через параметры в адресной строке.
    /// </summary>
    public const string IS_JWT_IN_QUERY_COOKIE_NAME = "IsJwsInQuery";

    /// <summary>
    /// Список привилегированных пользователей.
    /// </summary>
    /// <remarks>
    /// В нашем случае - один.
    /// </remarks>
    private static readonly List<Personality> Superusers = 
        new() { new Personality("Admin", "12345") };

    /// <summary>
    /// "Свойства" привилегированных пользователей.
    /// </summary>
    private static readonly List<Claim> Claims =
        Superusers.Select(p => new Claim(ClaimTypes.Name, p.Name!)).ToList();

    /// <summary>
    /// Получить привилегированного пользователя по имени.
    /// </summary>
    public static Personality? GetSuperuser(string? name) =>
        Superusers.FirstOrDefault(p => p.Name == name);

    /// <summary>
    /// Получить первого найденного привилегированного пользователя без пароля.
    /// </summary>
    public static Personality? GetFirstSuperuserWithoutPassword() =>
        Superusers
            .Select(p => new Personality(p.Name!, string.Empty))
            .FirstOrDefault();

    /// <summary>
    /// Получить параметры валидации JWT-токена.
    /// </summary>
    public static TokenValidationParameters GetTokenValidationParameters()
    {
        var tvp = new TokenValidationParameters
        {
            ValidateIssuer = true,                  // проверка издателя
            ValidIssuer = JwtOptions.ISSUER,
            ValidateAudience = true,                // проверка потребителя
            ValidAudience = JwtOptions.AUDIENCE,
            ValidateLifetime = true,                // проверка времени жизни
            IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,        // проверка ключа
        };

        return tvp;
    }

    #region [----- Работа с признаком того, что аутенификация по параметрам в адресной строке -----]

    /// <summary>
    /// Получить из кук признак того,
    /// что JWS-токен передается через параметры в адресной строке.
    /// </summary>
    /// <param name="context">Http-контекст.</param>
    public static bool GetJwsInQueryFlag(HttpContext context)
    {
        var flagStr = context.Request.Cookies[IS_JWT_IN_QUERY_COOKIE_NAME];
        return flagStr.ParseToBool(true);
    }

    /// <summary>
    /// Установить в куках (или сбросить) признак того,
    /// что JWS-токен передается через параметры в адресной строке.
    /// </summary>
    /// <param name="value">Значение признака.</param>
    /// <param name="context">Http-контекст.</param>
    public static void SetJwsInQueryFlag(bool value, HttpContext context)
    {
        context.Response.Cookies.Append(IS_JWT_IN_QUERY_COOKIE_NAME, value.ToString(),
            new CookieOptions()
            {
                HttpOnly = true,
                Secure = true
            }
        );
    }

    #endregion

    #region [----- Работа с JWT-токеном -----]

    /// <summary>
    /// Создать JWT-токен.
    /// </summary>
    /// <param name="expires">Время окончание жизни токена.</param>
    public static JwtSecurityToken CreateJwt(DateTime expires)
    {
        var jwt = new JwtSecurityToken(
            issuer: JwtOptions.ISSUER,
            audience: JwtOptions.AUDIENCE,
            claims: Claims,
            expires: expires,
            signingCredentials: new SigningCredentials(JwtOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        
        return jwt;
    }
    
    /// <summary>
    /// Сериализовать JWT-токен.
    /// </summary>
    public static string SerializeJwt(this JwtSecurityToken jwt) =>
        new JwtSecurityTokenHandler().WriteToken(jwt);

    /// <summary>
    /// Локальное время окончания жизни JWT-токена.
    /// </summary>
    public static DateTime Expires(this JwtSecurityToken jwt) => 
        jwt.ValidTo.ToLocalTime();
    
    /// <summary>
    /// Записать JWT-токен в заголовок HTTP-запроса.
    /// </summary>
    public static void AddJwtToHeader(HttpContext context, string? jwtStr)
    {
        if (!string.IsNullOrEmpty(jwtStr))
            context.Request.Headers.Add("Authorization", "Bearer " + jwtStr);
    }
    
    // /// <summary>
    // /// Записать JWT-токен в параметры HTTP-запроса.
    // /// </summary>
    // public static void AddJwtToQuery(HttpContext context, string? jwtStr)
    // {
    //     if (jwtStr.IsNullOrEmpty())
    //         return;
    //     
    //     var query = 
    //         HttpUtility.ParseQueryString(context.Request.QueryString.Value ?? string.Empty);
    //     query["access_token"] = jwtStr;
    //
    //     context.Request.QueryString = new QueryString("?" + query.ToString());
    // }
    
    /// <summary>
    /// Получить JWT-токен из заголовка HTTP-запроса.
    /// </summary>
    public static string? GetJwtFromHeader(HttpContext context)
    {
        context.Request.Headers.TryGetValue("Authorization", out var headerBody);
        var jwtStr = headerBody.FirstOrDefault()?.GetEnd("Bearer ");
        return jwtStr;
    }
    
    /// <summary>
    /// Признак того, что JWT-токен валиден.
    /// </summary>
    public static bool IsValidJwtStr(string? jwtStr)
    {
        if (jwtStr is null)
            return false;
        
        var validator = new JwtSecurityTokenHandler();
        var tvp = GetTokenValidationParameters();
        try
        {
            var principal = validator.ValidateToken(jwtStr, tvp, out var validatedToken);
            var personName = principal.Identity?.Name;
            
            return Superusers.Select(p => p.Name).Contains(personName);
        }
        catch
        {
            // ignored
        }

        return false;
    }

    /// <summary>
    /// Получить JWT-токен из куков.
    /// </summary>
    /// <param name="context">Http-контекст.</param>
    public static string? GetJwtStr(HttpContext context) => 
        context.Request.Cookies[JWT_COOKIE_NAME];

    /// <summary>
    /// Сохранить JWT-токен в куки.
    /// </summary>
    /// <param name="jwtStr">JWS-токен в виде строки.</param>
    /// <param name="context">Http-контекст.</param>
    /// <param name="maxAge">Срок жизни токена (точнее - куки, куда токен сохраняется.</param>
    public static void SetJwtStr(string jwtStr, HttpContext context, TimeSpan? maxAge = null)
    {
        if (maxAge is null)
        {
            var validator = new JwtSecurityTokenHandler();
            var jwt = validator.ReadJwtToken(jwtStr);
            var expires = jwt.Expires();
            maxAge = TimeSpan.FromSeconds(expires.Subtract(DateTime.Now).TotalSeconds);
        }

        context.Response.Cookies.Append(JWT_COOKIE_NAME, jwtStr,
            new CookieOptions()
            {
                MaxAge = maxAge,
                HttpOnly = true,
                Secure = true
            }
        );
    }

    /// <summary>
    /// Удалить JWT-токен из кук.
    /// </summary>
    /// <param name="context">Http-контекст.</param>
    public static void RemoveJwtStr(HttpContext context)
    {
        context.Response.Cookies.Delete(JWT_COOKIE_NAME);
    }

    #endregion
}
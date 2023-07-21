using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using App.Infrastructure.Authorization.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.BaseExtensions;

namespace App.Infrastructure.Authorization;

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
    /// что JWT-токен передается через параметры в адресной строке.
    /// </summary>
    public const string IS_JWT_IN_QUERY_COOKIE_NAME = "IsJwtInQuery";

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
        tvp.ClockSkew = new TimeSpan(0);

        return tvp;
    }

    #region [----- Работа с признаком того, что аутенификация по параметрам в адресной строке -----]

    /// <summary>
    /// Получить из кук признак того,
    /// что JWT-токен передается через параметры запроса (query).
    /// </summary>
    /// <param name="context">Http-контекст.</param>
    public static bool GetJwtInQueryFlag(HttpContext context)
    {
        var flagStr = context.Request.Cookies[IS_JWT_IN_QUERY_COOKIE_NAME];
        return flagStr.ParseToBool(true);
    }

    /// <summary>
    /// Установить в куках (или сбросить) признак того,
    /// что JWT-токен передается через параметры запроса (query).
    /// </summary>
    /// <param name="value">Значение признака.</param>
    /// <param name="context">Http-контекст.</param>
    public static void SetJwtInQueryFlag(bool value, HttpContext context)
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
    /// Записать JWT-токен в заголовок HTTP-запроса.
    /// </summary>
    /// <param name="context">Http-контекст.</param>
    /// <param name="jwtStr">Записываемый JWT-токен.</param>
    /// <param name="isRewriteHeader">Признак того, что если заголовок существует,
    /// то его необходимо перезаписать.</param>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void AddJwtToHeader(HttpContext context, string? jwtStr, bool isRewriteHeader = false)
    {
        const string HEADER_NAME = "Authorization";

        if (// Если токен пуст,
            jwtStr.IsNull() ||
            // или заголовок присутствует, но флаг сброшен
            (context.Request.Headers[HEADER_NAME].Count > 0 && !isRewriteHeader))
        {
            return;
        }
        
        context.Request.Headers[HEADER_NAME] = $"Bearer {jwtStr}";
    }
    
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
    /// Получить JWT-токен из заголовка HTTP-запроса.
    /// </summary>
    public static string? GetJwtFromHeader(HttpContext context)
    {
        context.Request.Headers.TryGetValue("Authorization", out var headerBody);
        var jwtStr = headerBody.FirstOrDefault()?.GetEnd("Bearer ");
        return jwtStr;
    }
    
    /// <summary>
    /// Получить JWT-токен из куков.
    /// </summary>
    /// <param name="context">Http-контекст.</param>
    public static string? GetJwtStr(HttpContext context) => 
        context.Request.Cookies[JWT_COOKIE_NAME];

    /// <summary>
    /// Признак того, что JWT-токен валиден.
    /// </summary>
    public static bool IsValidJwtStr(string? jwtStr)
    {
        if (jwtStr.IsNullOrEmpty())
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
    /// Удалить JWT-токен из кук.
    /// </summary>
    /// <param name="context">Http-контекст.</param>
    public static void RemoveJwtStr(HttpContext context) => 
        context.Response.Cookies.Delete(JWT_COOKIE_NAME);

    /// <summary>
    /// Сериализовать JWT-токен.
    /// </summary>
    public static string? SerializeJwt(this JwtSecurityToken? jwt) =>
        jwt is null
            ? null
            : new JwtSecurityTokenHandler().WriteToken(jwt);

    /// <summary>
    /// Сохранить JWT-токен в куки.
    /// </summary>
    /// <param name="jwt">JWT-токен.</param>
    /// <param name="context">Http-контекст.</param>
    /// <param name="maxAge">Срок жизни токена (точнее - куки, куда токен сохраняется.</param>
    public static void SaveJwt(this JwtSecurityToken jwt, HttpContext context, TimeSpan maxAge)
    {
        var jwtStr = jwt.SerializeJwt();

        if (jwtStr != null)
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
    /// Локальное время начала жизни JWT-токена.
    /// </summary>
    public static DateTime ValidFromLocal(this JwtSecurityToken? jwt) =>
        jwt?.ValidFrom.ToLocalTime() ?? DateTime.MinValue;

    /// <summary>
    /// Локальное время окончания жизни JWT-токена.
    /// </summary>
    public static DateTime ValidToLocal(this JwtSecurityToken? jwt) =>
        jwt?.ValidTo.ToLocalTime() ?? DateTime.MinValue;

    #endregion
}
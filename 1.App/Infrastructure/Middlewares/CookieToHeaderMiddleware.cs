using App.Infrastructure.Authorization;

namespace App.Infrastructure.Middlewares;

/// <summary>
/// Промежуточный слой, который записывает JWT-токен из куков в заголовок HTTP-запроса.
/// </summary>
public class CookieToHeaderMiddleware
{
    /// <summary>
    /// Ссылка на делегат следующего запроса в конвейере.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Признак того, что если заголовок существует, то его необходимо перезаписать.
    /// </summary>
    private readonly bool _isRewriteHeader;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="next">Ссылка на делегат следующего запроса в конвейере.</param>
    /// <param name="isRewriteHeader">Признак того, что если заголовок существует,
    /// то его необходимо перезаписать.</param>
    public CookieToHeaderMiddleware(RequestDelegate next, bool isRewriteHeader = false)
    {
        _next = next;
        _isRewriteHeader = isRewriteHeader;
    }

    /// <summary>
    /// Метод, вызываемый при обработки запроса.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Получаем JWT-токен
        var jwtStr = LoginManager.GetJwtStr(context);
        
        // Пишем токен в заголовок.
        LoginManager.AddJwtToHeader(context, jwtStr);

        await _next.Invoke(context);
    }
}
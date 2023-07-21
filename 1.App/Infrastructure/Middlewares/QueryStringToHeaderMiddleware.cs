
using App.Infrastructure.Authorization;

namespace App.Infrastructure.Middlewares;

/// <summary>
/// Промежуточный слой, который записывает JWT-токен из параметров строки запроса URL в заголовок HTTP-запроса.
/// </summary>
public class QueryStringToHeaderMiddleware
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
    public QueryStringToHeaderMiddleware(RequestDelegate next, bool isRewriteHeader = false)
    {
        _next = next;
        _isRewriteHeader = isRewriteHeader;
    }

    /// <summary>
    /// Метод, вызываемый при обработки запроса.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Получаем нужный нам параметр из Query
        context.Request.Query.TryGetValue("access_token", out var queryStringValues);

        if (queryStringValues.Count != 1)
        {
            // Если ошибка - в заголовок не пишем
            await _next.Invoke(context);
            return;
        }

        // Получаем JWT-токен
        var jwtStr = queryStringValues.Single();
        
        // Пишем токен в заголовок.
        LoginManager.AddJwtToHeader(context, jwtStr, isRewriteHeader: _isRewriteHeader);
        
        await _next.Invoke(context);
    }
}
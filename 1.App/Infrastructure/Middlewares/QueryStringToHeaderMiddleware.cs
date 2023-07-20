
using App.Infrastructure.Authorization;

namespace App.Infrastructure.Middlewares;

/// <summary>
/// Промежуточный слой, который записывает JWT-токен из Query-параметров в заголовок HTTP-запроса.
/// </summary>
public class QueryStringToHeaderMiddleware
{
    /// <summary>
    /// Ссылка на делегат следующего запроса в конвейере.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public QueryStringToHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Метод, вызываемый при обработки запроса.
    /// </summary>
    public async Task InvokeAsync(HttpContext context )
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
        LoginManager.AddJwtToHeader(context, jwtStr, isRewriteHeader: false);
        
        await _next.Invoke(context);
    }
}
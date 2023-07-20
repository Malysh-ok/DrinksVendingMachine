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
    /// Список путей; если текущий путь HTTP-контекста
    /// совпадает с любым элементом этого списка, то JWT-токен в заголовок не записывается.
    /// </summary>
    private readonly IList<PathString> _skipPaths;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="next">Ссылка на делегат следующего запроса в конвейере.</param>
    /// <param name="skipPaths">Список путей; если текущий путь HTTP-контекста
    /// совпадает с любым элементом этого списка, то JWT-токен в заголовок не записывается.
    /// </param>
    public CookieToHeaderMiddleware(
        RequestDelegate next, IList<PathString> skipPaths)
    {
        _next = next;
        _skipPaths = skipPaths;
    }

    /// <summary>
    /// Метод, вызываемый при обработки запроса.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        if (_skipPaths.Contains(context.Request.Path))
        {
            await _next.Invoke(context);
            return;
        }
        
        // Получаем JWT-токен
        var jwtStr = LoginManager.GetJwtStr(context);
        
        // Пишем токен в заголовок.
        LoginManager.AddJwtToHeader(context, jwtStr);

        await _next.Invoke(context);
    }
}
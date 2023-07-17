using Domain.Models;
using Infrastructure.BaseExtensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace App.Main.Controllers;

/// <summary>
/// Контроллер для работы с ошибками.
/// </summary>

public class ErrorController : Controller
{
    /// <summary>
    /// Обработчик ошибки.
    /// </summary>
    /// <param name="message">Текст ошибки.</param>
    /// <param name="stackTrace">Трассировка стека.</param>
    [Route("/error")]
    public IActionResult HandleError(string? message, string? stackTrace)
    {
        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;
        return View("Error", new ErrorModel(
            exceptionHandlerFeature?.Error.Flatten() ?? message,
            exceptionHandlerFeature?.Error.StackTrace ?? stackTrace
        ));
    }
}
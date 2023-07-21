using System.Security.Claims;
using System.Security.Policy;
using App.Infrastructure.Authorization.Models;
using App.Main.Controllers.Dto;
using Infrastructure.BaseExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace App.Main.Controllers;

/// <summary>
/// Контроллер для работы с аутентификацией.
/// </summary>
public class LoginController : Controller
{
    /// <summary>
    /// Главная модель.
    /// </summary>
    private readonly LoginModel _mainModel;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="mainModel">Главная модель.</param>
    public LoginController(LoginModel mainModel)
    {
        _mainModel = mainModel;
    }
    
    /// <summary>
    /// Показываем главное Представление.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    [Route("Login")]
    public IActionResult Index([FromQuery(Name = "access_token")] string jwtStr)
    {
        if (_mainModel.IsValidJwtStr(HttpContext, jwtStr))
        {
            // Если токен валиден - на страницу администратора
            return RedirectToAction("Index", "Admin",
                _mainModel.GetJwtInQueryFlag(HttpContext)
                    ? new {access_token = jwtStr}
                    : null
            );
        }
        
        return View("Index",
            new LoginControllerDto(_mainModel.GetSuperuserWithoutPassword(),
                _mainModel.GetJwtInQueryFlag(HttpContext))
        );
    }

    /// <summary>
    /// Получаем данные из главного Представления.
    /// </summary>
    [HttpPost]
    [Route("Login")]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoginControllerDto loginControllerDto)
    {
        // Извлекаем данные из loginControllerDto в person и isJwtInAddressBarParams
        var (user, isJwtInQuery) = loginControllerDto.ExtractData();

        if (ModelState.IsValid)
        {
            // Устанавливаем признак того, что JWT-токен передается через параметры запроса (query)
            _mainModel.SetJwtInQueryFlag(isJwtInQuery, HttpContext);
            
            if (!_mainModel.IsAuthorization(user))
            {
                // Устанавливаем ошибку для автоматического отображения
                ModelState.AddModelError("", "Неверное имя пользователя и/или пароль.");
            }
            else
            {
                // Пользователь прошел авторизацию
                
                // Сохраняем токен
                _mainModel.UpdateAndSaveJwt(HttpContext);
                
                // Перенаправляем на прошлую страницу
                var referer = TempData["Referer"]?.ToString() 
                              ?? Url.Action("Index","Buyer");
                if (referer.IsContains("Admin"))
                    return RedirectToAction("Index", "Admin",
                        _mainModel.GetJwtInQueryFlag(HttpContext)
                            ? new {access_token = _mainModel.GetJwtStr()}
                            : null
                    );
                return RedirectToAction("Index", "Buyer");
            }
        }
        
        return View("Index");
    }

    /// <summary>
    /// Разлогиниться.
    /// </summary>
    [Route("Logout")]
    public IActionResult Logout()
    {
        // Делаем невалидным токен
        _mainModel.CancelJwt(HttpContext);

        // Сохраняем страницу с которой уходим, если она не Login
        var referer = Request.Headers["Referer"].ToString();
        if (!referer.IsContains("Login"))
            TempData["Referer"] = referer;
        
        return RedirectToAction("Index");
    }

    [Route("AjaxRedirect")]
    public IActionResult AjaxRedirect()
    {
        var url = Url.Action("Index");
        return Json(new { RedirectUrl = url });
    }
}
using System.Security.Claims;
using App.Authorization;
using App.Authorization.Models;
using App.Main.Controllers.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [Route("Admin/Login")]
    public IActionResult Index([FromQuery(Name = "access_token")] string jwtStr)
    {
        if (_mainModel.IsValidJwtStr(HttpContext, jwtStr))
        {
            // Если токен валиден - на страницу администратора
            return RedirectToAction("Index", "Admin",
                _mainModel.GetJwsInQueryFlag(HttpContext)
                    ? new {access_token = jwtStr}
                    : null
            );
        }
        
        return View("Index",
            new LoginControllerDto(_mainModel.GetSuperuserWithoutPassword(),
                _mainModel.GetJwsInQueryFlag(HttpContext))
        );
    }

    /// <summary>
    /// Получаем данные из главного Представления.
    /// </summary>
    [HttpPost]
    [Route("Admin/Login")]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoginControllerDto loginControllerDto)
    {
        // Извлекаем данные из loginControllerDto в person и isJwsInAddressBarParams
        var (user, isJwsInQuery) = loginControllerDto.ExtractData();

        if (ModelState.IsValid)
        {
            // Устанавливаем признак того, что JWS-токен передается через параметры в адресной строке
            _mainModel.SetJwsInQueryFlag(isJwsInQuery, HttpContext);
            
            if (!_mainModel.IsAuthorization(user))
            {
                // Устанавливаем ошибку для автоматического отображения
                ModelState.AddModelError("", "Неверное имя пользователя и/или пароль.");
            }
            else
            {
                // Пользователь прошел авторизацию
                
                // Сохраняем токен
                _mainModel.UpdateAndSaveJws(HttpContext);
                
                return RedirectToAction("Index", "Admin",
                    isJwsInQuery
                        ? new {access_token = _mainModel.JwtStr}
                        : null
                );
            }
        }
        
        return View("Index");
    }

    /// <summary>
    /// Разлогиниться.
    /// </summary>
    [Route("Admin/Logout")]
    public IActionResult Logout()
    {
        // Делаем невалидным токен
        _mainModel.CancelJws(HttpContext);
        
        return RedirectToAction("Index");
    }
}
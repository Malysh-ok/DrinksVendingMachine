using App.Authorization.Models;
using App.Main.Controllers.Dto;
using Domain.Entities;
using Domain.Models;
using Infrastructure.AppComponents.AppExceptions.AdminModelExceptions;
using Infrastructure.AspComponents.Extensions;
using Infrastructure.BaseExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace App.Main.Controllers;

/// <summary>
/// Контроллер для работы с администратором.
/// </summary>
public class AdminController : Controller
{
    /// <summary>
    /// Главная модель.
    /// </summary>
    private readonly AdminModel _mainModel;

    /// <summary>
    /// Модель авторизации.
    /// </summary>
    private readonly LoginModel _loginModel;

    /// <summary>
    /// Механизм рендеринга Представления.
    /// </summary>
    private readonly ICompositeViewEngine _viewEngine;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="mainModel">Главная модель.</param>
    /// <param name="loginModel">Модель авторизации.</param>
    /// <param name="viewEngine">Механизм рендеринга Представления.</param>
    public AdminController(AdminModel mainModel, LoginModel loginModel, ICompositeViewEngine viewEngine)
    {
        _mainModel = mainModel;
        _loginModel = loginModel;
        _viewEngine = viewEngine;
    }

    /// <summary>
    /// Показываем главное Представление.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult Index()
    {
        // Если JWS-токен передается через параметры в адресной строке -
        // пробрасываем токен модели
        ViewData["JwtStr"] = _loginModel.GetJwsInQueryFlag(HttpContext)
            ? _loginModel.JwtStr
            : null;
        
        return View(_mainModel);
    }
    
    /// <summary>
    /// Обработка нажатия кнопки "Применить" (для Напитка).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApplyDrink([FromBody] [Bind("Id, Count, Price")] Drink drink)
    {
        // В контроллере - только валидация
        string? infoHtml;

        if (!ModelState.IsValid)
        {
            // Ошибка валидации модели
            return BadRequest(this.GetModelStateErrors());
        }

        if (drink.Price < 1 || drink.Count < 1)
        {
            // Не положительная цена/количество
            infoHtml = this.ConvertViewToString(
                PartialView("_Info", 
                    new InfoModel("Цена и (или) количество напитков должны " +
                                  "иметь положительные значения!", true)), _viewEngine
            );
            
            return Json(new AdminControllerToViewDto(
                infoHtml: infoHtml
            ));
        }
        
        // ---------------------------------
        // Обновляем данные в главной Модели
        var result = _mainModel.UpdateDrink(drink);

        if (!result)
        {
            // Фатальная ошибка
            return StatusCode(StatusCodes.Status500InternalServerError, 
                result.Excptn.Flatten());
        }

        // Все ок!
        var drinksHtml = this.ConvertViewToString(
            PartialView("_Drinks", _mainModel.Drinks), _viewEngine
        );
        infoHtml = this.ConvertViewToString(
            PartialView("_Info", 
                new InfoModel("Напиток изменен.")), _viewEngine
        );
        return Json(new AdminControllerToViewDto(
            drinksHtml: drinksHtml,
            infoHtml: infoHtml
        ));
    }

    /// <summary>
    /// Обработка нажатия кнопки "Добавить" (для Напитка).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddDrink([FromBody] [Bind("Name, Count, Price")] Drink drink)
    {
        // В контроллере - только валидация
        string? infoHtml;

        if (!ModelState.IsValid)
        {
            // Ошибка валидации модели
            return BadRequest(this.GetModelStateErrors());
        }

        if (drink.Price < 1 || drink.Count < 1)
        {
            // Не положительная цена/количество
            infoHtml = this.ConvertViewToString(
                PartialView("_Info", 
                    new InfoModel("Цена и (или) количество напитков " +
                                  "должны иметь положительные значения!", true)), _viewEngine
            );
            return Json(new AdminControllerToViewDto(
                infoHtml: infoHtml
            ));
        }
        
        // ---------------------------------
        // Обновляем данные в главной Модели
        var result = _mainModel.AddDrink(drink);
        
        if (result.Excptn is DrinkAlreadyExistsException)
        {
            // Напиток с таким названием уже существует
            infoHtml = this.ConvertViewToString(
                PartialView("_Info", 
                    new InfoModel(result.Excptn.Message, true)), _viewEngine
            );            
            
            return Json(new UserControllerToViewDto(
                infoHtml: infoHtml
            ));
        }

        if (!result)
        {
            // Фатальная ошибка
            return StatusCode(StatusCodes.Status500InternalServerError, 
                result.Excptn.Flatten());
        }

        // Все ок!
        infoHtml = this.ConvertViewToString(
            PartialView("_Info", 
                new InfoModel("Напиток добавлен.")), _viewEngine
        );
        var drinksHtml = this.ConvertViewToString(
            PartialView("_Drinks", _mainModel.Drinks), _viewEngine
        );
        return Json(new AdminControllerToViewDto(
            drinksHtml: drinksHtml,
            infoHtml: infoHtml,
            isClearDrinks: true
        ));    
    }
    
    /// <summary>
    /// Обработка нажатия кнопки "Удалить" (для Напитка).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveDrink([FromBody] [Bind("Name")] Drink drink)
    {
        // В контроллере - только валидация
        string? infoHtml;

        if (!ModelState.IsValid)
        {
            // Ошибка валидации модели
            return BadRequest(this.GetModelStateErrors());
        }
        
        // ---------------------------------
        // Обновляем данные в главной Модели
        var result = _mainModel.RemoveDrink(drink);
        
        if (result.Excptn is DrinkNotExistsException)
        {
            // Напитка с таким названием не существует
            infoHtml = this.ConvertViewToString(
                PartialView("_Info", 
                    new InfoModel(result.Excptn.Message, true)), _viewEngine
            );            
            
            return Json(new UserControllerToViewDto(
                infoHtml: infoHtml
            ));
        }

        if (!result)
        {
            // Фатальная ошибка
            return StatusCode(StatusCodes.Status500InternalServerError, 
                result.Excptn.Flatten());
        }

        // Все ок!
        infoHtml = this.ConvertViewToString(
            PartialView("_Info", 
                new InfoModel("Напиток удален.")), _viewEngine
        );
        var drinksHtml = this.ConvertViewToString(
            PartialView("_Drinks", _mainModel.Drinks), _viewEngine
        );
        return Json(new AdminControllerToViewDto(
            drinksHtml: drinksHtml,
            infoHtml: infoHtml,
            isClearDrinks: true
        ));    
    }

    /// <summary>
    /// Обработка нажатия кнопки "Применить" (для Монет).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApplyCoin([FromBody] [Bind("Id, Count, IsLocked")] Coin coin)
    {
        // В контроллере - только валидация
        string? infoHtml;

        if (!ModelState.IsValid)
        {
            // Ошибка валидации модели
            return BadRequest(this.GetModelStateErrors());
        }

        if (coin.Count < 1)
        {
            // Не положительная цена/количество
            infoHtml = this.ConvertViewToString(
                PartialView("_Info",
                    new InfoModel("Количество монет должно иметь положительно значение!", 
                        true)), _viewEngine
            );

            return Json(new AdminControllerToViewDto(
                infoHtml: infoHtml
            ));
        }
        
        // ---------------------------------
        // Обновляем данные в главной Модели
        var result = _mainModel.UpdateCoin(coin);

        if (!result)
        {
            // Фатальная ошибка
            return StatusCode(StatusCodes.Status500InternalServerError, 
                result.Excptn.Flatten());
        }

        // Все ок!
        infoHtml = this.ConvertViewToString(
            PartialView("_Info", 
                new InfoModel("Монета изменена.")), _viewEngine
        );
        var coinsHtml = this.ConvertViewToString(
            PartialView("_Coins", _mainModel.Coins), _viewEngine
        );
        return Json(new AdminControllerToViewDto(
            coinsHtml: coinsHtml,
            infoHtml: infoHtml
        ));
    }
    
    /// <summary>
    /// Обработка нажатия кнопки "Отмена".
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Escape()
    {
        var partialViewResult = PartialView("_Info", 
            new InfoModel("Начнем сначала...", false));
        return Json(new AdminControllerToViewDto(
            isClearDrinks: true,
            isClearAdditionDrink: true,
            isClearCoins: true,
            infoHtml:this.ConvertViewToString(partialViewResult, _viewEngine))
        );
    }
}
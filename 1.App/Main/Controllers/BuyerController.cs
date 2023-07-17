using System.Diagnostics;
using App.Authorization.Models;
using App.Main.Controllers.Dto;
using Domain.Entities;
using Domain.Models;
using Infrastructure.AppComponents.AppExceptions.UserModelExceptions;
using Infrastructure.AspComponents;
using Infrastructure.AspComponents.Extensions;
using Infrastructure.BaseExtensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace App.Main.Controllers;

/// <summary>
/// Контроллер для работы с покупателем.
/// </summary>
public class BuyerController : Controller
{
    /// <summary>
    /// Главная модель.
    /// </summary>
    private readonly BuyerModel _buyerModel;

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
    /// <param name="buyerModel">Главная модель.</param>
    /// <param name="loginModel">Модель авторизации.</param>
    /// <param name="viewEngine">Механизм рендеринга Представления.</param>
    public BuyerController(BuyerModel buyerModel, LoginModel loginModel, ICompositeViewEngine viewEngine)
    {
        _buyerModel = buyerModel;
        _loginModel = loginModel;
        _viewEngine = viewEngine;
    }
    
    /// <summary>
    /// Показываем главное Представление.
    /// </summary>
    public IActionResult Index()
    {
        // Если JWS-токен передается через параметры в адресной строке -
        // пробрасываем токен модели
        ViewData["JwtStr"] = _loginModel.GetJwsInQueryFlag(HttpContext)
            ? _loginModel.JwtStr
            : null;

        return View(_buyerModel);
    }
    
    // TODO: На будущее (переделать на асинхронные операции):
    // public async Task<IActionResult> Index()
    // {
    //     return View(await _buyerModel);
    // }
    
    /// <summary>
    /// Обработка нажатия кнопки "Отмена".
    /// </summary>
    /// <param name="purchaseDto">Объект, характеризующий покупку напитка.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Escape([FromBody] PurchaseDto purchaseDto)
    {
        var returnedCoins = _buyerModel.GetChange(purchaseDto.PurchaseParts);
        var infoHtml = returnedCoins.IsNullOrEmpty()
            ? string.Empty
            : this.ConvertViewToString(
            PartialView("_Info", 
                new InfoModel("Возвращаем деньги.")), _viewEngine
        );
        
        return Json(new UserControllerToViewDto(true,
            infoHtml: infoHtml,
            change: returnedCoins, 
            drink: string.Empty
        ));
    }

    /// <summary>
    /// Обработка нажатия кнопки "Купить".
    /// </summary>
    /// <param name="purchaseDto">Объект, характеризующий покупку напитка.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Buy([FromBody] PurchaseDto purchaseDto)
    {
        // В контроллере - только валидация

        if (!ModelState.IsValid)
        {
            // Ошибка валидации модели
            return BadRequest(this.GetModelStateErrors());
        }

        if (!purchaseDto.IsBuyerGetSubjects)
        {
            // Если покупатель не забрал сдачу и напиток
            var infoHtml = this.ConvertViewToString(
                PartialView("_Info", 
                    new InfoModel("Заберите сдачу и напиток!", true)), _viewEngine
            );
            return Json(new UserControllerToViewDto(
                infoHtml: infoHtml
            ));
        }

        if (purchaseDto.DrinkId == 0)
        {
            // Если не выбран напиток
            var infoHtml = this.ConvertViewToString(
                PartialView("_Info", 
                    new InfoModel("Не выбран напиток!", true)), _viewEngine
            );            
            return Json(new UserControllerToViewDto(
                infoHtml: infoHtml
            ));
        }
        
        if (purchaseDto.PurchaseParts.Select(pp => pp.CoinCount).Sum() == 0)
        {
            // Если деньги не вносили 
            var infoHtml = this.ConvertViewToString(
                PartialView("_Info", 
                    new InfoModel("Сначала внесите деньги!", true)), _viewEngine
            );            
            return Json(new UserControllerToViewDto(
                infoHtml: infoHtml
            ));
        }
            
        // ---------------------------------
        // Обновляем данные в главной Модели
        var result = _buyerModel.UpdateData(purchaseDto.PurchaseParts, purchaseDto.DrinkId);
        
        switch (result.Excptn)
        {
            case NotEnoughMoneyException:
            {
                // Не хватает денег
                var infoHtml = this.ConvertViewToString(
                    PartialView("_Info", 
                        new InfoModel(result.Excptn.Message, true)), _viewEngine
                );            
            
                return Json(new UserControllerToViewDto(
                    infoHtml: infoHtml
                ));
            }
            case DrinkIsOverException:
            {
                // Напиток закончился
                var returnedCoins = _buyerModel.GetChange(purchaseDto.PurchaseParts);
                var infoHtml = this.ConvertViewToString(
                    PartialView("_Info", 
                        new InfoModel($"{result.Excptn.Message} Заберите деньги.", 
                            true)), _viewEngine
                );            

                return Json(new UserControllerToViewDto(
                    isClearMainModelData: true,
                    infoHtml: infoHtml,
                    change: returnedCoins, 
                    drink: string.Empty
                ));
            }
            case NoMoneyLeftException:
            {
                // В автомате закончились деньги
                var returnedCoins = _buyerModel.GetChange(purchaseDto.PurchaseParts);
                var infoHtml = this.ConvertViewToString(
                    PartialView("_Info", 
                        new InfoModel("Нет возможности выдать сдачу. Заберите деньги.", 
                            true)), _viewEngine
                );            
            
                return Json(new UserControllerToViewDto(
                    isClearMainModelData: true,
                    infoHtml: infoHtml,
                    change: returnedCoins, 
                    drink: string.Empty
                ));
            }
        }

        if (!result)
        {
            // Фатальная ошибка
            return StatusCode(StatusCodes.Status500InternalServerError, 
                result.Excptn.Flatten());
        }

        // Все ок!
        return Json(new UserControllerToViewDto(
            isClearMainModelData: true,
            change: result.Value.Change,
            drink: result.Value.DrinkName));
    }

    /// <summary>
    /// Обработка нажатия кнопки "Забрать".
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UserReceiving()
    {
        // Очищаем сдачу и напиток
        return Json(new UserControllerToViewDto(
            infoHtml: string.Empty,
            change: string.Empty, 
            drink: string.Empty
        ));
    }

    /// <summary>
    /// Обработка нажатия ссылки "License".
    /// </summary>
    [Route("License")]
    public IActionResult License()
    {
        return View("License");
    }
}
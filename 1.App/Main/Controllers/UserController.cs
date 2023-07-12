using System.Diagnostics;
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

public class UserController : Controller
{
    /// <summary>
    /// Главная модель.
    /// </summary>
    private readonly UserModel _userModel;

    /// <summary>
    /// Механизм рендеринга Представления.
    /// </summary>
    private readonly ICompositeViewEngine _viewEngine;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userModel">Главная модель.</param>
    /// <param name="viewEngine">Механизм рендеринга Представления.</param>
    public UserController(UserModel userModel, ICompositeViewEngine viewEngine)
    {
        _userModel = userModel;
        _viewEngine = viewEngine;
    }
    
    // Показываем главное Представление
    public IActionResult Index()
    {
        return View(_userModel);
    }
    
    // TODO: На будущее (переделать на асинхронные операции):
    // public async Task<IActionResult> Index()
    // {
    //     return View(await _userModel);
    // }
    
    /// <summary>
    /// Обработка нажатия кнопки "Отмена"
    /// </summary>
    /// <param name="purchaseDto">Объект, характеризующий покупку напитка.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Escape([FromBody] PurchaseDto purchaseDto)
    {
        var returnedCoins = _userModel.GetChange(purchaseDto.PurchaseParts);
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
    /// Обработка нажатия кнопки "Купить"
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
        var result = _userModel.UpdateData(purchaseDto.PurchaseParts, purchaseDto.DrinkId);
        
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
                var returnedCoins = _userModel.GetChange(purchaseDto.PurchaseParts);
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
                var returnedCoins = _userModel.GetChange(purchaseDto.PurchaseParts);
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
    /// Обработка нажатия кнопки "Забрать"
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
    
    // [Route("/error")]
    // public IActionResult HandleError()
    // {
    //     var exceptionHandlerFeature =
    //         HttpContext.Features.Get<IExceptionHandlerFeature>()!;
    //     // return Content("Вот и жопа пришла!!!\r\n"
    //     //                + exceptionHandlerFeature.Error.Flatten() + "\r\n"
    //     //                + exceptionHandlerFeature.Error.StackTrace);
    //     return View("Error", new ErrorModel(
    //         exceptionHandlerFeature.Error.Flatten(),
    //         exceptionHandlerFeature.Error.StackTrace
    //     ));
    // }
}
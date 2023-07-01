using System.Diagnostics;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Main.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    private readonly MainModel _mainModel;

    public UserController(ILogger<UserController> logger, MainModel mainModel)
    {
        _logger = logger;
        _mainModel = mainModel;
    }

    public IActionResult Index()
    {
        return View(_mainModel);
    }

    public IActionResult Edit2([FromBody] DataFromView dataFromView)   // string[] lines
    {
        
        
        return PartialView("_PurchasePartial", _mainModel.EmptyPurchase);
    }

    [HttpPost]
    public IActionResult Edit([FromBody] DataFromView dataFromView)
    {
        if (ModelState.IsValid)
        {
            var result = _mainModel.UpdateData(dataFromView);
            if (!result)  
                BadRequest(result.Excptn.Message);        
        }
        return PartialView("_PurchasePartial", _mainModel.EmptyPurchase);
    }


    public IActionResult Admin()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // TODO: Разобраться с ошибками и перенести Модель в слой Domain
        return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
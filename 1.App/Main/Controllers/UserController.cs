using System.Diagnostics;
using System.Net;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Main.Controllers;

public class UserController : Controller
{
    // TODO: Переделать на асинхронные операции.
    
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
                return BadRequest(result.Excptn.Message);      
                
            return PartialView("_PurchasePartial", _mainModel.EmptyPurchase);
        }
        
        return BadRequest(new { turnOnClass = "is-invalid", turnOffClass = "invisible" });
    }

    public IActionResult Admin()
    {
        return View();
    }

    // TODO: Сделать валидацию и сообщения об ошибках
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
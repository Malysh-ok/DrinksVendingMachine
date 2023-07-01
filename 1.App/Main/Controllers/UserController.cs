using System.Diagnostics;
using App.Main.Models;
using Domain.DbContexts;
using Domain.Entities;
using Infrastructure.BaseExtensions.Collections;
using Microsoft.AspNetCore.Mvc;

namespace App.Main.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    private readonly AppDbContext _dbContext;

    public UserController(ILogger<UserController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return View(new MainModel(
            _dbContext.Drinks.ToList(),
            Purchase.GetEmptyPurchases()
        ));
    }

    public IActionResult Edit2([FromBody] IEnumerable<Purchase> purchases )   // string[] lines
    {
        
        return PartialView("_PurchasePartial", _dbContext.Purchases.ToList());
    }

    [HttpPost]
    public IActionResult Edit([FromBody] IList<Purchase> purchases)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Заполняем номер и дату
                var number = _dbContext.Purchases.Any()
                    ? _dbContext.Purchases
                        .OrderBy(p => p.Number)
                        .LastOrDefault()!.Number + 1
                    : 1;
                var dt = DateTime.Now;
                purchases.ForEach(p =>
                {
                    p.Number = number;
                    p.TimeStump = dt;
                });

                // Обновляем данные в БД
                _dbContext.UpdateRange(purchases);
                _dbContext.SaveChanges();
            }
            catch
            {
                return BadRequest("Ошибка обновления базы данных.");        
            }
        }

        return PartialView("_PurchasePartial", Purchase.GetEmptyPurchases());
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
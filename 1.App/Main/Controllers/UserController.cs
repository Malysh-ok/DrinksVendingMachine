using System.Diagnostics;
using App.Main.Models;
using Domain.DbContexts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;

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
        // return View();
        return View(new Agregate(
            _dbContext.Coins.ToList(), 
            _dbContext.Drinks.ToList(),
            // _dbContext.Purchases.ToList()
            Purchase.GetEmptyPurchases()
            ));
    }

    public IActionResult Edit2([FromBody] IEnumerable<Purchase> purchases )   // string[] lines
    {
        
        return PartialView("_PurchasePartial", _dbContext.Purchases.ToList());
    }

    [HttpPost]
    public IActionResult Edit([FromBody] IEnumerable<Purchase> purchases)
    {
        Console.WriteLine("+++ Request!!!");
        Console.WriteLine(Request);
        
        Agregate? aggregate = null;
        try
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.UpdateRange(purchases);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }
        finally
        {
            aggregate = new Agregate(
                _dbContext.Coins.ToList(), 
                _dbContext.Drinks.ToList(),
                Purchase.GetEmptyPurchases()
            );
        }
        // return View("Index", aggregate);
        return PartialView("_PurchasePartial", Purchase.GetEmptyPurchases());
    }


    public IActionResult Admin()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
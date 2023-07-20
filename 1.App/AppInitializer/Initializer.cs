using Domain.DbContexts;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.AppComponents.AppExceptions;
using Infrastructure.BaseComponents.Components;
using Infrastructure.Phrases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.AppInitializer;

/// <summary>
/// Инициализатор приложения.
/// </summary>
public static class Initializer
{
    /// <summary>
    /// Заполнить БД.
    /// </summary>
    /// <param name="dbContext">Контекст БД.</param>
    /// <returns>True или false (в случае неудачи), обернутое в <see cref="Result{T}"/>.</returns>
    public static Result<bool> FillDatabase(AppDbContext? dbContext)
    {
        if (dbContext is null)
            return Result<bool>.Fail(DbPhrases.DbContextError);

        // Заполняем монеты
        try
        {
            dbContext.Coins.AddRange(
                new Coin(CoinEnm.One, 1000),
                new Coin(CoinEnm.Two, 1000),
                new Coin(CoinEnm.Five, 1000, false),
                new Coin(CoinEnm.Ten, 1000)
            );
            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(ex);
        }
        
        // Заполняем напитки
        try
        {
            dbContext.Drinks.AddRange(
                new Drink("Сок апельсиновый", 20, 100),
                new Drink("Сок ананасовый", 40, 100),
                new Drink("Сок персиковый", 25, 100),
                new Drink("Сок вишневый", 25, 100),
                new Drink("Чай ромашковый", 20, 100),
                new Drink("Чай из Иван-чая", 30, 100)
            );
            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(ex);
        }
        
        return Result<bool>.Done(true);
    }

    /// <summary>
    /// Инициализировать БД.
    /// </summary>
    /// <remarks>
    /// Если БД уже существует - то ничего не делаем. 
    /// </remarks>
    /// <param name="dbContext">Контекст БД.</param>
    /// <returns>True или false (в случае неудачи), обернутое в <see cref="Result{T}"/>.</returns>
    public static Result<bool> InitDatabase(AppDbContext dbContext)
    {
        var result = Result<bool>.Done(true);

        // Признак существования БД
        var isExists = dbContext?.Database.CanConnect() ?? false;
        
        if (isExists) 
            return result;                          // выходим, если БД существует
        
        try
        {
            dbContext!.Database.EnsureCreated();
            result = FillDatabase(dbContext);               // заполняем БД
        }
        catch (Exception ex)
        {
            result = Result<bool>.Fail(ex);
        }

        return result;
    }

    /// <summary>
    /// Инициализация приложения.
    /// </summary>
    /// <param name="serviceProvider">Экземпляр <see cref="IServiceProvider"/> (сервисы).</param>
    /// <returns>True или false (в случае неудачи), обернутое в <see cref="Result{T}"/>.</returns>
    public static Result<bool> Init(IServiceProvider serviceProvider)
    {
        // var loginModel = serviceProvider.GetRequiredService<LoginModel>();
        // var isJwtInQueryFlag
        
        // Инициализация БД
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var result = InitDatabase(dbContext);
        return result 
            ? result 
            : Result<bool>.Fail(new AppException("Неудачное создание базы данных.", result.Excptn));

        // Инициализация ...
        // ...
    }
}
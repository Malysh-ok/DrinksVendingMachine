using Domain.DbContexts;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.BaseComponents.Components;
using Infrastructure.Phrases;
using Microsoft.EntityFrameworkCore;

namespace App.AppInitializer;

public static class DatabaseFiller
{
    public static Result<bool> Fill(AppDbContext? dbContext)
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
}
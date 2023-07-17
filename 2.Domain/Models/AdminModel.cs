using Domain.DbContexts;
using Domain.Entities;
using Infrastructure.AppComponents.AppExceptions.AdminModelExceptions;
using Infrastructure.BaseComponents.Components;

namespace Domain.Models;

/// <summary>
/// Главная модель администратора.
/// </summary>
/// <remarks>
/// Вся основная бизнес-логика сосредоточена здесь.
/// </remarks>
public class AdminModel
{
    /// <summary>
    /// Контекст БД.
    /// </summary>
    private readonly AppDbContext _dbContext;
    
    /// <summary>
    /// Монеты.
    /// </summary>
    public List<Coin> Coins => _dbContext.Coins.ToList();
    
    /// <summary>
    /// Напитки.
    /// </summary>
    public List<Drink> Drinks => _dbContext.Drinks.ToList();

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="dbContext">Контекст БД.</param>
    public AdminModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Обновляем напиток в модели.
    /// </summary>
    /// <param name="drinkTmp">Обновляемый напиток.</param>
    /// <returns>Либо true (при успешной операции), либо Exception,
    /// обернутое в <see cref="Result{T}"/>.</returns>
    public Result<bool> UpdateDrink(Drink drinkTmp)
    {
        try
        {
            // Получаем напиток по Id
            var drink = _dbContext.Drinks.Find(drinkTmp.Id);
            drink!.Count = drinkTmp.Count;
            drink!.Price = drinkTmp.Price;

            _dbContext.Update(drink);
            _dbContext.SaveChanges();
            return Result<bool>.Done(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(new AdminModelDataUpdateException(innerException: ex));        
        }
    }
    
    /// <summary>
    /// Добавляем напиток в модель.
    /// </summary>
    /// <param name="drink">Добавляемый напиток.</param>
    /// <returns>Либо true (при успешной операции), либо Exception,
    /// обернутое в <see cref="Result{T}"/>.</returns>
    public Result<bool> AddDrink(Drink drink)
    {
        try
        {
            var existingDrink = _dbContext.Drinks
                .FirstOrDefault(d => d.Name.Equals(drink.Name));
            if (existingDrink is not null)
                // Напиток с таким названием уже существует
                return Result<bool>.Fail(new DrinkAlreadyExistsException());
            
            _dbContext.Add(drink);
            _dbContext.SaveChanges();
            
            return Result<bool>.Done(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(new AdminModelDataUpdateException(innerException: ex));        
        }
    }
    
    /// <summary>
    /// Удаляем напиток из модели.
    /// </summary>
    /// <param name="drink">Удаляемый напиток.</param>
    /// <returns>Либо true (при успешной операции), либо Exception,
    /// обернутое в <see cref="Result{T}"/>.</returns>
    public Result<bool> RemoveDrink(Drink drink)
    {
        try
        {
            var existingDrink = _dbContext.Drinks
                .FirstOrDefault(d => d.Name.Equals(drink.Name));
            if (existingDrink is null)
                // Напитка с таким названием не существует
                return Result<bool>.Fail(new DrinkNotExistsException());
            
            _dbContext.Remove(existingDrink);
            _dbContext.SaveChanges();
            
            return Result<bool>.Done(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(new AdminModelDataUpdateException(innerException: ex));        
        }
    }
    
    /// <summary>
    /// Обновляем монету в модели.
    /// </summary>
    /// <param name="coinTmp">Обновляемая монета.</param>
    /// <returns>Либо true (при успешной операции), либо Exception,
    /// обернутое в <see cref="Result{T}"/>.</returns>
    public Result<bool> UpdateCoin(Coin coinTmp)
    {
        try
        {
            // Получаем монету по Id
            var coin = _dbContext.Coins.Find(coinTmp.Id);
            coin!.Count = coinTmp.Count;
            coin.IsLocked = coinTmp.IsLocked;

            _dbContext.Update(coin);
            _dbContext.SaveChanges();
            return Result<bool>.Done(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(new AdminModelDataUpdateException(innerException: ex));        
        }
    }
}
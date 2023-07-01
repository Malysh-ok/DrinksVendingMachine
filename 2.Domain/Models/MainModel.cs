using Domain.DbContexts;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.BaseComponents.Components;
using Infrastructure.BaseExtensions.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace Domain.Models;

public class MainModel
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="dbContext">Контекст БД.</param>
    public MainModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Напитки.
    /// </summary>
    public List<Drink> Drinks => _dbContext.Drinks.ToList();
    
    /// <summary>
    /// Части покупки.
    /// </summary>
    [BindNever]
    public List<PurchasePart> PurchaseParts => _dbContext.PurchaseParts.ToList();

    private List<PurchasePart>? _emptyPurchase;
    /// <inheritdoc cref="GetEmptyPurchase"/>
    public List<PurchasePart> EmptyPurchase => _emptyPurchase ??= GetEmptyPurchase();
    
    /// <summary>
    /// "Пустая" покупка.
    /// </summary>
    /// <remarks>
    /// Список всех возможных значений <see cref="PurchasePart"/>
    /// с пустыми данными.
    /// </remarks>
    private static List<PurchasePart> GetEmptyPurchase()
    {
        return Enum.GetValues(typeof(CoinEnm))
            .Cast<CoinEnm>()
            .Select(i => new PurchasePart(i))
            .ToList();
    }

    /// <summary>
    /// Обновляем данные в модели.
    /// </summary>
    /// <param name="dataFromView">Данные, переданные из представления.</param>
    /// <returns>Либо true (при успешной операции), либо Exception,
    /// обернутое в <see cref="Result{T}"/>.</returns>
    public Result<bool> UpdateData(DataFromView dataFromView)
    {
        try
        {
            // Заполняем номер и дату покупки, id напитка
            var number = _dbContext.PurchaseParts.Any()
                ? _dbContext.PurchaseParts
                    .OrderBy(p => p.PurchaseNumber)
                    .LastOrDefault()!.PurchaseNumber + 1
                : 1;
            var dt = DateTime.Now;
            dataFromView.PurchaseParts.ForEach(p =>
            {
                p.PurchaseNumber = number;
                p.TimeStump = dt;
                p.DrinkId = dataFromView.DrinkId;
            });

            // Обновляем данные в БД
            _dbContext.UpdateRange(dataFromView.PurchaseParts);
            _dbContext.SaveChanges();

            return Result<bool>.Done(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(new Exception("Ошибка обновления базы данных.", ex));        
        }    
    }
}
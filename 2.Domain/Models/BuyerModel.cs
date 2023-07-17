using System.Text;
using Domain.DbContexts;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Models.Dto;
using Infrastructure.AppComponents.AppExceptions.UserModelExceptions;
using Infrastructure.BaseComponents.Components;
using Infrastructure.BaseExtensions;
using Infrastructure.BaseExtensions.Collections;
using Infrastructure.BaseExtensions.ValueTypes;

namespace Domain.Models;

/// <summary>
/// Главная модель покупателя.
/// </summary>
/// <remarks>
/// Вся основная бизнес-логика сосредоточена здесь.
/// </remarks>
public class BuyerModel
{
    /// <summary>
    /// Контекст БД.
    /// </summary>
    private readonly AppDbContext _dbContext;

    private List<PurchasePart>? _emptyPurchase;
    /// <inheritdoc cref="GetEmptyPurchase"/>
    public List<PurchasePart> EmptyPurchase => _emptyPurchase ??= GetEmptyPurchase();

    /// <summary>
    /// Напитки.
    /// </summary>
    public List<Drink> Drinks => _dbContext.Drinks.ToList();
    
    /// <summary>
    /// Признак того, что сдача может выдаваться заблокированными монетами.
    /// </summary>
    public bool IsGivenOutLockedCoins { get; set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="dbContext">Контекст БД.</param>
    public BuyerModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        IsGivenOutLockedCoins = false;      // настройка по умолчанию -
                                            // сдача НЕ может выдаваться заблокированными монетами
    }

    /// <summary>
    /// "Пустая" покупка.
    /// </summary>
    /// <remarks>
    /// Список всех возможных значений <see cref="PurchasePart"/>
    /// с пустыми данными.
    /// </remarks>
    private List<PurchasePart> GetEmptyPurchase()
    {
        return Enum.GetValues(typeof(CoinEnm))
            .Cast<CoinEnm>()
            .Select(i =>
            {
                var coin = _dbContext.Coins
                    .OrderBy(c => c.Id)
                    .FirstOrDefault(c => c.Id == i);
                return new PurchasePart(i, coin);
            })
            .ToList();
    }

    /// <summary>
    /// Обрабатываем введенные покупателем монеты.
    /// </summary>
    /// <param name="change">Сдача.</param>
    /// <param name="purchaseParts">Части покупки.</param>
    /// <param name="coins">Список монет.</param>
    /// <returns>Сдача в виде строки, либо null - если неудача.</returns>
    private static string? HandleCoins(int change, IList<PurchasePart> purchaseParts, ref IList<Coin> coins)
    {
        // Клонируем список монет в coinsTmp,
        // к каждому количеству монет списка прибавляем монеты из покупки (purchaseParts)
        IList<Coin> coinsTmp = new List<Coin>();
        for (var i = 0; i < coins.Count; i++)
        {
            coinsTmp.Add(coins[i].Clone());
            var coinCount = purchaseParts
                .FirstOrDefault(pp => pp.CoinId == coinsTmp[i].Id)!.CoinCount;
            coinsTmp[i].Count += coinCount;
        }
        
        var isNoCoins = false;                      // признак того, что монеты закончились
        var sb = new StringBuilder();               // строка StringBuilder, в которой формируем текст сдачи
        
        // Перебираем монеты от большего номинала к меньшему
        foreach (var coin in coinsTmp)
        {
            if (coin.IsLocked)
                // Если монета заблокирована
                continue;
            
            isNoCoins = false;
            int nominal  = coin.Id.ToInt();         // номинал монеты
            int coinCount  = change / nominal;      // количество монет данного номинала для сдачи
            coin.Count -= coinCount;

            if (coin.Count < 0)
            {
                // Если монет с данным номиналом нет
                coinCount += coin.Count;
                coin.Count = 0;
                isNoCoins = true;
            }

            if (coinCount != 0)
            {
                // Если текущее количество монет не равно 0
                if (sb.Length != 0)
                    sb.Append(", ");
                sb.Append($"{nominal} => {coinCount}");
            }
            change -= coinCount * nominal;
        }

        if (isNoCoins)
            // Если монет с каким-то из номиналов не осталось
            return null;
        
        // Перезаписываем количество оставшихся монет в исходном списке
        for (var i = 0; i < coinsTmp.Count; i++)
        {
            coins[i].Count = coinsTmp[i].Count;
        }
        return sb.ToString();
    }
    
    
    /// <summary>
    /// Возвращаем деньги покупателю в виде текста.
    /// </summary>
    /// <param name="purchaseParts">Список частей покупки.</param>
    public string GetChange(IEnumerable<PurchasePart> purchaseParts)
    {
        var sb = new StringBuilder();
        
        purchaseParts.ForEach(pp =>
        {
            if (pp.CoinCount <= 0)
                return;
            
            if (sb.Length != 0)
                sb.Append(", ");
            sb.Append($"{pp.CoinId.ToInt()} => {pp.CoinCount}");
        });

        return sb.ToString();
    }
    
    /// <summary>
    /// Обновляем данные в модели.
    /// </summary>
    /// <param name="purchaseParts">Список частей покупки.</param>
    /// <param name="drinkId">Id выбранного напитка.</param>
    /// <returns>Либо объект для Контроллера (при успешной операции), либо Exception,
    /// обернутое в <see cref="Result{T}"/>.</returns>
    public Result<UserModelDto> UpdateData(IList<PurchasePart> purchaseParts, int drinkId)
    {
        try
        {
            var coinSum = 0;   // сумма монет
            
            // Заполняем номер и дату покупки, id напитка
            var number = _dbContext.PurchaseParts
                .OrderBy(p => p.PurchaseNumber)
                .LastOrDefault()?.PurchaseNumber ?? 0;
            var dt = DateTime.Now;
            purchaseParts.ForEach(p =>
            {
                p.PurchaseNumber = number + 1;
                p.TimeStump = dt;
                p.DrinkId = drinkId;
                coinSum += p.CoinCount * p.CoinId.ToInt();
            });

            // Получаем напиток по Id
            var drink = _dbContext.Drinks
                .OrderBy(e => e.Id)
                .FirstOrDefault(d => d.Id == drinkId);
            if (drink!.Count == 0)
                // Напиток закончился
                return Result<UserModelDto>.Fail(new DrinkIsOverException(drink.Name));

            drink!.Count--;     // уменьшаем количество ост. напитков
            
            if (coinSum < drink!.Price)
                // Не хватает денег
                return Result<UserModelDto>.Fail(new NotEnoughMoneyException());

            // Сдача
            IList<Coin> coins = _dbContext.Coins
                .OrderByDescending(c => c.Id)
                .ToList();                              // список монет
            int change = coinSum - drink!.Price;        // сдача числом
            var changeStr = HandleCoins(change, purchaseParts, ref coins);
            if (changeStr is null)
                // В автомате закончились деньги
                return Result<UserModelDto>.Fail(new NoMoneyLeftException());
            
            // Все ок - обновляем данные в БД
            _dbContext.UpdateRange(coins);
            _dbContext.Update(drink);
            _dbContext.UpdateRange(purchaseParts);
            _dbContext.SaveChanges();

            return Result<UserModelDto>.Done
            (
                // Передаем только название напитка
                new UserModelDto(changeStr.NullToEmpty(), drink.Name)
            );
        }
        catch (Exception ex)
        {
            return Result<UserModelDto>.Fail(new UserModelDataUpdateException(innerException: ex));        
        }    
    }
}
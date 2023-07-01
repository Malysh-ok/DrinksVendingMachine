using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Покупка.
/// </summary>
public class Purchase
{
    public Purchase()
    {
    }
    
    // TODO: Конструктор
    public Purchase(CoinEnm coinId, int coinCount = 0, DateTime? timeStump = null, int? drinkId = null)
    {
        CoinId = coinId;
        CoinCount = coinCount;
        DrinkId = drinkId;
        TimeStump = timeStump;
    }

    /// <summary>
    /// Монета (идентификатор).
    /// </summary>
    public CoinEnm CoinId { get; set; }

    /// <summary>
    /// Монета.
    /// </summary>
    public Coin? Coin { get; set; }
    
    /// <summary>
    /// Количество монет.
    /// </summary>
    public int CoinCount { get; set; }
    
    /// <summary>
    /// Дата/время опускания монеты.
    /// </summary>
    public DateTime? TimeStump { get; set; }

    /// <summary>
    /// Напиток (идентификатор).
    /// </summary>
    public int? DrinkId { get; set; }
    
    /// <summary>
    /// Напиток.
    /// </summary>
    public Drink? Drink { get; set; }

    /// <summary>
    /// Получаем список всех возможных значений <see cref="Purchase"/>
    /// с пустыми данными.
    /// </summary>
    public static List<Purchase> GetEmptyPurchases()
    {
        return Enum.GetValues(typeof(CoinEnm))
            .Cast<CoinEnm>()
            .Select(i => new Purchase(i))
            .ToList();
    }
}
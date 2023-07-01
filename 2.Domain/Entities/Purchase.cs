using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Покупка.
/// </summary>
public class Purchase
{
    /// <summary>
    /// Конструктор для MVC.
    /// </summary>
    public Purchase()
    {
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="coinId">Монета.</param>
    /// <param name="coinCount">Количество монет</param>
    /// <param name="drinkId">Напиток.</param>
    /// <param name="number">Номер покупки.</param>
    /// <param name="timeStump">Дата/время покупки.</param>
    public Purchase(CoinEnm coinId, int coinCount = 0, 
        int? drinkId = null, int number = 0, DateTime? timeStump = null)
    {
        CoinId = coinId;
        CoinCount = coinCount;
        DrinkId = drinkId;
        Number = number;
        TimeStump = timeStump;
    }
    
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Монета.
    /// </summary>
    public Coin? Coin { get; set; }
    
    /// <inheritdoc cref="Coin"/>
    public CoinEnm CoinId { get; set; }

    /// <summary>
    /// Количество монет.
    /// </summary>
    public int CoinCount { get; set; }
    
    /// <summary>
    /// Напиток.
    /// </summary>
    public Drink? Drink { get; set; }

    /// <inheritdoc cref="Drink"/>
    public int? DrinkId { get; set; }

    /// <summary>
    /// Номер покупки.
    /// </summary>
    public int Number { get; set; }
        
    /// <summary>
    /// Дата/время покупки.
    /// </summary>
    public DateTime? TimeStump { get; set; }

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
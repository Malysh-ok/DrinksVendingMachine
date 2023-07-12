using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Часть покупки, выраженная количеством одного номинала монеты.
/// </summary>
public class PurchasePart
{
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
    public int DrinkId { get; set; }

    /// <summary>
    /// Номер покупки.
    /// </summary>
    public int PurchaseNumber { get; set; }
        
    /// <summary>
    /// Дата/время покупки.
    /// </summary>
    public DateTime? TimeStump { get; set; }
    
    /// <summary>
    /// Конструктор для MVC.
    /// </summary>
    public PurchasePart()
    {
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="coinId">Номинал монеты.</param>
    /// <param name="coinCount">Количество монет</param>
    /// <param name="drinkId">Напиток.</param>
    /// <param name="purchaseNumber">Номер покупки.</param>
    /// <param name="timeStump">Дата/время покупки.</param>
    public PurchasePart(CoinEnm coinId, int coinCount = 0, 
        int drinkId = 0, int purchaseNumber = 0, DateTime? timeStump = null)
    {
        CoinId = coinId;
        CoinCount = coinCount;
        DrinkId = drinkId;
        PurchaseNumber = purchaseNumber;
        TimeStump = timeStump;
    }
    
    /// <inheritdoc cref="PurchasePart(CoinEnm, int, int, int, DateTime?)"/>
    /// <param name="coin">Монета.</param>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public PurchasePart(CoinEnm coinId, Coin? coin, int coinCount = 0,
        int drinkId = 0, int purchaseNumber = 0, DateTime? timeStump = null) 
        : this(coinId, coinCount, drinkId, purchaseNumber, timeStump)
    {
        Coin = coin;
    }
}
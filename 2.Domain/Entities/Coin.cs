using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Монета.
/// </summary>
public class Coin
{
    /// <summary>
    /// Идентификатор (номинал).
    /// </summary>
    public CoinEnm Id { get; set; }

    /// <summary>
    /// Количество оставшихся монет.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Признак того, что монета заблокирована.
    /// </summary>
    public bool IsLocked { get; set; }
    
    /// <summary>
    /// Список частей покупок.
    /// </summary>
    public ICollection<PurchasePart> PurchaseParts { get; set; } = new HashSet<PurchasePart>();
    
    /// <summary>
    /// Конструктор для MVC.
    /// </summary>
    public Coin()
    {
        Count = 0;
        IsLocked = false;
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="id">Номинал.</param>
    /// <param name="count">Количество оставшихся монет.</param>
    /// <param name="isLocked">Признак того, что монету можно использовать.</param>
    public Coin(CoinEnm id, int count, bool isLocked = false)
    {
        Id = id;
        Count = count;
        IsLocked = isLocked;
    }

    /// <summary>
    /// Клонирование монеты.
    /// </summary>
    public Coin Clone()
    {
        return new Coin(Id, Count, IsLocked);
    }
}
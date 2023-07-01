﻿using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Монета.
/// </summary>
public class Coin
{
    // public Coin()
    // {
    // }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="id">Номинал.</param>
    /// <param name="count">Количество оставшихся монет.</param>
    public Coin(CoinEnm id, int count)
    {
        Id = id;
        Count = count;
    }
    
    /// <summary>
    /// Идентификатор (номинал).
    /// </summary>
    public CoinEnm Id { get; set; }

    /// <summary>
    /// Количество оставшихся монет.
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// Список покупок.
    /// </summary>
    public ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
}
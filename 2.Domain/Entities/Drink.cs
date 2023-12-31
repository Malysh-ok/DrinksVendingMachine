﻿namespace Domain.Entities;

/// <summary>
/// Напиток.
/// </summary>
public class Drink
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Наименование.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Количество оставшихся порций напитка.
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// Цена.
    /// </summary>
    public int Price { get; set; }
    
    /// <summary>
    /// Список частей покупок.
    /// </summary>
    public ICollection<PurchasePart> PurchaseParts { get; set; } = new HashSet<PurchasePart>();
    
    /// <summary>
    /// Конструктор для MVC.
    /// </summary>
    public Drink()
    {
        Name = string.Empty;
        Count = 0;
        Price = 0;
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="name">Наименование.</param>
    /// <param name="price">Цена.</param>
    /// <param name="count">Оставшееся количество порций.</param>
    public Drink(string name, int price, int count)
    {
        Name = name;
        Count = count;
        Price = price;
    }
}
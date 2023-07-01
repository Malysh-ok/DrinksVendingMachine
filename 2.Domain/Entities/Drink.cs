namespace Domain.Entities;

/// <summary>
/// Напиток.
/// </summary>
public class Drink
{
    // public Drink()
    // {
    // }
    
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
    /// Список покупок.
    /// </summary>
    public ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
}
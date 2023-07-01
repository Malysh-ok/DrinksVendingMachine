namespace Domain.Entities;

public class MainModel
{
    /// <summary>
    /// Напитки.
    /// </summary>
    public List<Drink> Drinks { get; private set; }
    
    /// <summary>
    /// Покупки.
    /// </summary>
    public List<Purchase> Purchases { get; private set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="drinks">Напитки.</param>
    /// <param name="purchases">Покупки.</param>
    public MainModel(List<Drink> drinks, List<Purchase> purchases)
    {
        Drinks = drinks;
        Purchases = purchases;
    }
}
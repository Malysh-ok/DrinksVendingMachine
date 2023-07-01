namespace Domain.Entities;

public class Agregate
{

    public List<Coin> Coins { get; private set; }
    
    public List<Drink> Drinks { get; private set; }
    
    public List<Purchase> Purchases { get; private set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="coins"></param>
    /// <param name="drinks"></param>
    /// <param name="purchases"></param>
    public Agregate(List<Coin> coins, List<Drink> drinks, List<Purchase> purchases)
    {
        Coins = coins;
        Drinks = drinks;
        Purchases = purchases;
    }
}
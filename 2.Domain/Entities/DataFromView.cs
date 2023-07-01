namespace Domain.Entities;

public class DataFromView
{
    public DataFromView(int drinkId, IList<Purchase> purchases)
    {
        DrinkId = drinkId;
        Purchases = purchases;
    }

    public int DrinkId { get; set; }
    
    public IList<Purchase> Purchases { get; set; }
}
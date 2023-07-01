namespace Domain.Entities;

/// <summary>
/// Сущность, предназначенная для передачи данных из представления в контроллер
/// (для метода действия Edit)/>).
/// </summary>
public class DataFromView
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="drinkId">Id выбранного напитка.</param>
    /// <param name="purchaseParts"></param>
    public DataFromView(int drinkId, IList<PurchasePart> purchaseParts)
    {
        DrinkId = drinkId;
        PurchaseParts = purchaseParts;
    }

    /// <summary>
    /// Id выбранного напитка.
    /// </summary>
    public int DrinkId { get; set; }
    
    public IList<PurchasePart> PurchaseParts { get; set; }
}
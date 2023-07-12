#nullable disable
using Domain.Entities;

namespace App.Main.Controllers.Dto;

/// <summary>
/// Класс, характеризующий покупку напитка.
/// </summary>
/// <remarks>
/// Класс, предназначенный для передачи данных
/// из представления пользователя в контроллер <see cref="UserController"/> 
/// (для метода действия <see cref="UserController.Buy"/>).
/// </remarks>
public class PurchaseDto
{
    /// <summary>
    /// Id выбранного напитка.
    /// </summary>
    public int DrinkId { get; set; }
    
    /// <summary>
    /// Список частей покупки.
    /// </summary>
    public IList<PurchasePart> PurchaseParts { get; set; }
    
    /// <summary>
    /// Признак того, что покупатель забрал сдачу и напиток.
    /// </summary>
    public bool IsBuyerGetSubjects { get; set; }
}
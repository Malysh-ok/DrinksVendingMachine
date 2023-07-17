#nullable disable
using Microsoft.AspNetCore.Mvc;

namespace App.Main.Controllers.Dto;

/// <summary>
/// Класс, предназначенный для передачи данных от Контроллера <see cref="BuyerController"/>
/// в Представление.
/// </summary>
public class UserControllerToViewDto
{
    /// <summary>
    /// Признак очистки данных в Представлении, связанных главной Моделью.
    /// </summary>
    public bool IsClearMainModelData { get; }
    
    /// <summary>
    /// <see cref="PartialViewResult"/> частичного Представления _Info, преобразованный в string.
    /// </summary>
    public string InfoHtml { get; }

    /// <summary>
    /// Сдача покупателю.
    /// </summary>
    public string Change { get; }
    
    /// <summary>
    /// Напиток покупателю.
    /// </summary>
    public string Drink { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="isClearMainModelData">Признак очистки данных в Представлении,
    /// связанных главной Моделью.</param>
    /// <param name="infoHtml"><see cref="PartialViewResult"/> частичного Представления _Info,
    /// преобразованный в string.</param>
    /// <param name="change">Сдача покупателю.</param>
    /// <param name="drink">Напиток покупателю.</param>
    public UserControllerToViewDto(bool isClearMainModelData = false, 
        string infoHtml = null, string change = null, string drink = null)
    {
        IsClearMainModelData = isClearMainModelData;
        InfoHtml = infoHtml;
        Change = change;
        Drink = drink;
    }
}
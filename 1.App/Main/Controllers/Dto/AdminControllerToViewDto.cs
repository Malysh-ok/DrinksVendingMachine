#nullable disable
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace App.Main.Controllers.Dto;

/// <summary>
/// Класс, предназначенный для передачи данных от Контроллера <see cref="AdminController"/>
/// в Представление.
/// </summary>
public class AdminControllerToViewDto
{
    /// <summary>
    /// <see cref="PartialViewResult"/> частичного Представления _Drink, преобразованный в string.
    /// </summary>
    public string DrinksHtml { get; }

    /// <summary>
    /// Признак очистки данных в Представлении, связанных настройкой напитка.
    /// </summary>
    public bool IsClearDrinks { get; }

    /// <summary>
    /// Признак очистки данных в Представлении, связанных добавлением напитка.
    /// </summary>
    public bool IsClearAdditionDrink { get; }

    /// <summary>
    /// <see cref="PartialViewResult"/> частичного Представления _Coins, преобразованный в string.
    /// </summary>
    public string CoinsHtml { get; }

    /// <summary>
    /// Признак очистки данных в Представлении, связанных настройкой монет.
    /// </summary>
    public bool IsClearCoins { get; }

    /// <summary>
    /// <see cref="PartialViewResult"/> частичного Представления _Info, преобразованный в string.
    /// </summary>
    public string InfoHtml { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="drinksHtml"><see cref="PartialViewResult"/> частичного Представления _Drink,
    /// преобразованный в string.</param>
    /// <param name="isClearDrinks">Признак очистки данных в Представлении, связанных настройкой напитка.</param>
    /// <param name="isClearAdditionDrink">Признак очистки данных в Представлении, связанных добавлением напитка.</param>
    /// <param name="coinsHtml"><see cref="PartialViewResult"/> частичного Представления _Coins,
    /// преобразованный в string.</param>
    /// <param name="isClearCoins">Признак очистки данных в Представлении, связанных настройкой монет.</param>
    /// <param name="infoHtml"><see cref="PartialViewResult"/> частичного Представления _Info,
    /// преобразованный в string.</param>
    public AdminControllerToViewDto(
        string drinksHtml = null, 
        bool isClearDrinks = false,
        bool isClearAdditionDrink = false,
        string coinsHtml = null,
        bool isClearCoins = false,
        string infoHtml = null)
    {
        DrinksHtml = drinksHtml;
        IsClearDrinks = isClearDrinks;
        IsClearAdditionDrink = isClearAdditionDrink;
        CoinsHtml = coinsHtml;
        IsClearCoins = isClearCoins;
        InfoHtml = infoHtml;
    }
}
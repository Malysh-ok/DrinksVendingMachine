using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace App.Infrastructure.Authorization;

/// <summary>
/// Вспомогательный класс, для настроек генерации и валидации JWT-токена.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class JwtOptions
{
    /// <summary>
    /// Издатель токена.
    /// </summary>
    public const string ISSUER = "DrinksVendingMachineServer";
    
    /// <summary>
    /// Потребитель токена.
    /// </summary>
    public const string AUDIENCE = "DrinksVendingMachineClient";
    
    /// <summary>
    /// Ключ для шифрования.
    /// </summary>
    const string KEY = "DrinksVendingMachine-12.07.2023";
    
    /// <summary>
    /// Симметричный ключ для подписи JWT-токена.
    /// </summary>
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new(Encoding.UTF8.GetBytes(KEY));
}
using System.ComponentModel.DataAnnotations;

namespace App.Authorization.Entities;

/// <summary>
/// Личность (человек).
/// </summary>
public class Personality
{
    /// <summary>
    /// Имя.
    /// </summary>
    [Required(ErrorMessage = "Поле не должно быть пустым.")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Пароль.
    /// </summary>
    [Required(ErrorMessage = "Поле не должно быть пустым.")]
    public string? Password { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public Personality()
    {
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public Personality(string name, string password)
    {
        Name = name;
        Password = password;
    }
}
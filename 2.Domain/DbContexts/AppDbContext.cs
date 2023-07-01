#nullable disable
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Domain.DbContexts;

/// <summary>
/// Основной контекст приложения.
/// </summary>
[SuppressMessage("ReSharper", "NotNullOrRequiredMemberIsNotInitialized")]
public sealed partial class AppDbContext : DbContext
{
    /// <summary>
    /// Строка подключения к БД.
    /// </summary>
    private readonly string _connectionString;

    /// <summary>
    /// Конструктор, запрещающий создание экземпляра без параметров.
    /// </summary>
    public AppDbContext()
    {
    }
        
    /// <summary>
    /// Конструктор.
    /// </summary>
    internal AppDbContext(string connectionString)
        : base()
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        _connectionString = Database.GetConnectionString();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options, string connectionString)
        : base(options)
    {
        _connectionString = connectionString;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Создание модели монет. 
        CreateModel_Coins(modelBuilder);
        
        // Создание модели напитков. 
        CreateModel_Drinks(modelBuilder);
        
        // Создание модели покупок. 
        CreateModel_Purchases(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        optionsBuilder.UseSqlite(_connectionString);
    }
}
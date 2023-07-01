using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.BaseExtensions.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Domain.DbContexts;

public sealed partial class AppDbContext
{
    /// <summary>
    /// Монеты.
    /// </summary>
    public DbSet<Coin> Coins { get; set; }
        
    /// <summary>
    /// Напитки.
    /// </summary>
    public DbSet<Drink> Drinks { get; set; }

    /// <summary>
    /// Покупки.
    /// </summary>
    public DbSet<Purchase> Purchases { get; set; }

    
    /// <summary>
    /// Создание модели монет.
    /// </summary>
    private static void CreateModel_Coins(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coin>(entity =>
        {
            entity.ToTable($"Coins",
                t => t.HasComment("Монеты"));

            entity.Property(c => c.Id).ValueGeneratedNever()
                .HasConversion(
                    enm => enm.ToInt(),
                    i => i.ToEnumWithException<CoinEnm>()
                );
                
            entity.Property(c => c.Count).IsRequired();

            entity.HasKey(c => c.Id)
                .HasName("PK_Coins");
        });
    }
    
    /// <summary>
    /// Создание модели напитков.
    /// </summary>
    private static void CreateModel_Drinks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Drink>(entity =>
        {
            entity.ToTable($"Drinks",
                t => t.HasComment("Напитки"));

            entity.Property(d => d.Id).ValueGeneratedOnAdd();
                
            entity.Property(d => d.Name).IsRequired().HasMaxLength(300);
            
            entity.Property(d => d.Count).IsRequired();
            
            entity.Property(d => d.Price).IsRequired();

            entity.HasKey(d => d.Id)
                .HasName("PK_Drinks");
        });
    }
    
    /// <summary>
    /// Создание модели покупок.
    /// </summary>
    private static void CreateModel_Purchases(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable($"Purchases",
                t => t.HasComment("Покупки"));

            entity.Property(p => p.CoinId).ValueGeneratedNever()
                .HasConversion(
                    enm => enm.ToInt(),
                    i => i.ToEnumWithException<CoinEnm>()
                );
                
            entity.Property(p => p.CoinCount).IsRequired();
            
            entity.Property(p => p.TimeStump).IsRequired(false);

            entity.HasKey(p => p.CoinId)
                .HasName("PK_Purchases");
            
            // Вторичный ключ - Монеты
            entity.HasOne(p => p.Coin)
                .WithMany(c => c.Purchases)
                .HasForeignKey(p => p.CoinId)
                .HasConstraintName("FK_Purchases_CoinId");
            
            // Вторичный ключ - Напитки
            entity.HasOne(p => p.Drink)
                .WithMany(d => d.Purchases)
                .HasForeignKey(p => p.DrinkId)
                .HasConstraintName("FK_Purchases_DrinkId");
        });
    }
}
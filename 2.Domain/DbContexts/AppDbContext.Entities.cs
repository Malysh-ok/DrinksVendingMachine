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
    /// Части покупок.
    /// </summary>
    public DbSet<PurchasePart> PurchaseParts { get; set; }

    
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

            entity.Property(c => c.IsLocked).IsRequired();

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
    /// Создание модели частей покупок.
    /// </summary>
    private static void CreateModel_PurchaseParts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchasePart>(entity =>
        {
            entity.ToTable($"PurchaseParts",
                t => t.HasComment("Части покупок"));

            entity.Property(p => p.Id).ValueGeneratedOnAdd();

            entity.Property(p => p.CoinId).IsRequired()
                .HasConversion(
                    enm => enm.ToInt(),
                    i => i.ToEnumWithException<CoinEnm>()
                );
                
            entity.Property(p => p.CoinCount).IsRequired();
            
            entity.Property(p => p.DrinkId).IsRequired();
            
            entity.Property(p => p.PurchaseNumber).IsRequired();
            
            entity.Property(p => p.TimeStump).IsRequired();

            entity.HasKey(p => p.Id)
                .HasName("PK_PurchasePart");
            
            // Вторичный ключ - Монеты
            entity.HasOne(p => p.Coin)
                .WithMany(c => c.PurchaseParts)
                .HasForeignKey(p => p.CoinId)
                .HasConstraintName("FK_Purchases_CoinId");
            
            // Вторичный ключ - Напитки
            entity.HasOne(p => p.Drink)
                .WithMany(d => d.PurchaseParts)
                .HasForeignKey(p => p.DrinkId)
                .HasConstraintName("FK_Purchases_DrinkId");
        });
    }
}
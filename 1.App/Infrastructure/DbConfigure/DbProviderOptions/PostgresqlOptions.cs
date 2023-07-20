using System.Diagnostics.CodeAnalysis;
using App.Infrastructure.DbConfigure.Interfaces;
using App.Infrastructure.DbManagement;
using Infrastructure.BaseComponents.Components;
using Infrastructure.BaseExtensions;
using Infrastructure.Phrases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure.DbConfigure.DbProviderOptions;

/// <summary>
/// Опции (настройки) провайдера БД - PostgreSql.
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
public class PostgresqlOptions : IDbProviderOptions
{
    /// <inheritdoc />
    public DbProviderEnm DbProviderType => DbProviderEnm.PostgreSql;

    /// <inheritdoc />
    public bool IsEmbeddedDb => false;
    
    /// <inheritdoc />
    public string? DateTimeColumnType => "TimestampTz";
    
    /// <inheritdoc />
    public string? MigrationsAssemblyName { get; private init; }
    
    /// <summary>
    /// Конструктор, запрещающий создание экземпляра без параметров.
    /// </summary>
    private PostgresqlOptions()
    {
    }

    /// <inheritdoc />
    public static IDbProviderOptions Create(string? migrationsAssemblyName = null)
    {
        var sqliteOptions = new PostgresqlOptions
        {
            MigrationsAssemblyName = migrationsAssemblyName
        };
        return sqliteOptions;
    }
    
    /// <inheritdoc />
    public void ClearAutoincrementSequence(DbContext dbContext, 
        string autoincrementColumnName, params string[] tableNames)
    {
        // TODO: В PostgreeSQL не тестировалось!
        foreach (var table in tableNames)
        {
            // Получаем название последовательности для столбца с автоинкрементом
            var sequenceName = dbContext.Database.SqlQueryRaw<string>
                ($"SELECT pg_get_serial_sequence('{table}', '{autoincrementColumnName}');").FirstOrDefault();

            // Обнуляем счетчик
            dbContext.Database.ExecuteSqlRaw
                ($"ALTER SEQUENCE {sequenceName} RESTART WITH 1;");
        }
    }

    /// <inheritdoc />
    public Result<string> FixConnectionString(string connectionString, string? rootPath)
    {
        // По сути - ничего не делаем: просто пробрасываем connectionString
        return connectionString.IsNull()
            ? Result<string>.Fail(new ArgumentException(DbPhrases.ConnectionStringError))
            : Result<string>.Done(connectionString);
    }

    /// <inheritdoc />
    public string GetConnectionString(ConfigurationManager configuration)
    {
        // TODO: Не тестировалось!
        return configuration.GetConnectionString("PostgreSqlConnection") ?? string.Empty;
    }

    /// <inheritdoc />
    public DbContextOptions<TDbContext> GetDbContextsOptions<TDbContext>(
        DbContextOptionsBuilder<TDbContext> optionsBuilder, string connectionString) where TDbContext : DbContext
    {
        if (MigrationsAssemblyName.IsNullOrEmpty())
            optionsBuilder.UseNpgsql(connectionString);
        else
            optionsBuilder.UseNpgsql(connectionString, 
                b => b.MigrationsAssembly(MigrationsAssemblyName));

#if DEBUG
        Console.WriteLine(@"MigrationsAssembly ===== " + MigrationsAssemblyName);
#endif
        
        return optionsBuilder.Options;
    }

    /// <inheritdoc />
    public void ModelBuilderInit(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityByDefaultColumns(); // стратегия генерации автоувеличения значения столбцов

        modelBuilder.HasPostgresExtension("adminpack")
            .HasAnnotation("Relational:Collation", "Russian_Russia.1251")    // правило сортировки
            .HasAnnotation("Relational:CType", "Russian_Russia.1251");       // набор символов
    }
}
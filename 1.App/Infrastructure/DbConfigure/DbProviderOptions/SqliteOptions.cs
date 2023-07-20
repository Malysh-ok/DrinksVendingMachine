using App.Infrastructure.DbConfigure.Interfaces;
using App.Infrastructure.DbManagement;
using Infrastructure.BaseComponents.Components;
using Infrastructure.BaseExtensions;
using Infrastructure.Phrases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure.DbConfigure.DbProviderOptions;

/// <summary>
/// Опции (настройки) провайдера БД - SQLite.
/// </summary>
public class SqliteOptions : IDbProviderOptions
{
    /// <inheritdoc />
    public DbProviderEnm DbProviderType => DbProviderEnm.Sqlite;

    /// <inheritdoc />
    public bool IsEmbeddedDb => true;

    /// <inheritdoc />
    public string? DateTimeColumnType => null;

    /// <inheritdoc />
    public string? MigrationsAssemblyName { get; private init; }

    /// <summary>
    /// Конструктор, запрещающий создание экземпляра без параметров.
    /// </summary>
    private SqliteOptions()
    {
    }

    /// <inheritdoc />
    public static IDbProviderOptions Create(string? migrationsAssemblyName = null)
    {
        var sqliteOptions = new SqliteOptions
        {
            MigrationsAssemblyName = migrationsAssemblyName
        };
        return sqliteOptions;
    }
    
    /// <inheritdoc />
    public void ClearAutoincrementSequence(DbContext dbContext, 
        string autoincrementColumnName, params string[] tableNames)
    {
        // Формируем из последовательности названий строку для SQL-выражения
        var str = $"'{string.Join("', '", tableNames)}'";
            
        // Удаляем названия таблиц из специальной скрытой таблицы sqlite_sequence,
        // предназначенной для автоинкремента столбцов
        dbContext.Database.ExecuteSqlRaw
            ($"DELETE FROM `sqlite_sequence` WHERE `name` IN ({str});");
    }

    /// <inheritdoc />
    public Result<string> FixConnectionString(string connectionString, string? rootPath)
    {
        return connectionString.IsNull()
            ? Result<string>.Fail(new ArgumentException(DbPhrases.ConnectionStringError))
            : Result<string>.Done(string.Format(connectionString, rootPath ?? string.Empty));
    }

    /// <inheritdoc />
    public string GetConnectionString(ConfigurationManager configuration)
    {
        return configuration.GetConnectionString("SqliteConnection") ?? string.Empty;
    }
    
    /// <inheritdoc />
    public DbContextOptions<TDbContext> GetDbContextsOptions<TDbContext>(
        DbContextOptionsBuilder<TDbContext> optionsBuilder, string connectionString) where TDbContext : DbContext
    {
        if (MigrationsAssemblyName.IsNullOrEmpty())
            optionsBuilder.UseSqlite(connectionString);
        else
            optionsBuilder.UseSqlite(connectionString, 
                b => b.MigrationsAssembly(MigrationsAssemblyName));

#if DEBUG
        Console.WriteLine(@"MigrationsAssembly ===== " + MigrationsAssemblyName);
#endif
        
        return optionsBuilder.Options;
    }

    /// <inheritdoc />
    public void ModelBuilderInit(ModelBuilder modelBuilder)
    {
        // Ничего не делаем
    }

}
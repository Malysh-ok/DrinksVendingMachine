using System.Diagnostics.CodeAnalysis;
using App.Infrastructure.DbConfigure.DbProviderOptions;
using App.Infrastructure.DbConfigure.Interfaces;
using Infrastructure.BaseComponents.Components;
using Infrastructure.BaseExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure.DbConfigure
{
    /// <summary>
    /// Кофигуратор БД.
    /// </summary>
    /// <remarks>
    /// Различные настройки, связанные с подключением и работой БД.
    /// </remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    public class DbConfigurator
    {
        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public string ProcessedConnectionString { get; private set; }
        
        /// <summary>
        /// Опции текущего провайдера БД.
        /// </summary>
        public IDbProviderOptions ProviderOptions { get; private set; }

        /// <summary>
        /// Признак того, что строка подключения к БД зашифрована.
        /// </summary>
        public bool IsEncryptedConnectionString { get; set; }
        
        /// <summary>
        /// Наименование сборки с миграциями
        /// (по всей видимости, необходимо только фабрике контекста БД,
        /// которая используется в Entity Framework Core).
        /// </summary>
        public string? MigrationsAssemblyName
        {
            get => ProviderOptions.MigrationsAssemblyName;
            set => ProviderOptions = GetProviderOptions(value!);
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public DbConfigurator(ConfigurationManager configuration, string rootPath)
        {
            ProviderOptions = GetProviderOptions();
            IsEncryptedConnectionString = false;

            // Получаем строку подключения из конфигурации, обрабатываем и сохраняем
            var connectionStringPre = 
                ProviderOptions.GetConnectionString(configuration);
            ProcessedConnectionString = 
                GetProcessedConnectionString(connectionStringPre, rootPath);
        }
        
        /// <summary>
        /// Получить опции текущего провайдера БД (паттерн "Стратегия").
        /// </summary>
        /// <remarks>
        /// В нашем случае провайдер - SQLite.
        /// </remarks>
        private static IDbProviderOptions GetProviderOptions(string? migrationsAssemblyName = null) =>
            SqliteOptions.Create(migrationsAssemblyName);

        /// <summary>
        /// Получить обработанную строку подключения к БД.
        /// </summary>
        /// <param name="connectionStringPre">Строка подключения, полученная из конфигурации.</param>
        /// <param name="rootPath">Путь к корневой папке приложения.</param>
        private string GetProcessedConnectionString(string connectionStringPre, string? rootPath = null)
        {
            // Дешифруем строку подключения при необходимости
            string connStr;
            if (IsEncryptedConnectionString)
            {
                var secretKey = ArrayExtensions.RandomByteArray(1, Crypto.SecretKeySize);
                var iv = ArrayExtensions.RandomByteArray(2, Crypto.IvSize);
                connStr = Crypto.Decrypt(connectionStringPre, secretKey, iv);
            }
            else
                connStr = connectionStringPre;

            // Корректируем строку подключения 
            var resultConnStr = ProviderOptions.FixConnectionString(connStr, rootPath);
            return !resultConnStr 
                ? string.Empty          // при ошибке - возвращаем пустую строку (может лучше null ?)
                : resultConnStr.Value;
        }        
        
        /// <inheritdoc cref="IDbProviderOptions.GetDbContextsOptions{TDbContext}(DbContextOptionsBuilder{TDbContext}, string )"/>
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        public DbContextOptions<TDbContext> GetContextsOptions<TDbContext>() where TDbContext: DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
            
            return ProviderOptions.GetDbContextsOptions(optionsBuilder, ProcessedConnectionString);
        }
        
        /// <inheritdoc cref="IDbProviderOptions.ModelBuilderInit(ModelBuilder)"/>
        public void ModelBuilderInit(ModelBuilder modelBuilder)
        {
            ProviderOptions.ModelBuilderInit(modelBuilder);
        }
    }
}

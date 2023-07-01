using System.Diagnostics.CodeAnalysis;
using Infrastructure.AppComponents.ConfigManagement.Database;
using Infrastructure.BaseExtensions;
using Microsoft.EntityFrameworkCore.Design;

namespace Domain.DbContexts
{
    /// <summary>
    /// Фабрика создания контекста БД (используется при создании миграций).
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <inheritdoc />
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        public AppDbContext CreateDbContext(string[] args)
        {
            // Получаем Кофигуратор БД
            var dbConfigurator = new DbConfigurator();

            // Добавляем название проекта с миграциями, которое берем из командной строки
            dbConfigurator.MigrationsAssemblyName = args.FindArg("migrationsAssembly");

            return new AppDbContext(dbConfigurator.GetContextsOptions<AppDbContext>(), 
                dbConfigurator.GetProcessedConnectionString());
        }
    }
}
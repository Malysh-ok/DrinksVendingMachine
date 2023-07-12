using System.Web;
using App.AppInitializer;
using Domain.DbContexts;
using Domain.Models;
using Infrastructure.AppComponents.AppExceptions;
using Infrastructure.BaseComponents.Components;
using Infrastructure.BaseExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// builder.Services.AddMvc();

// Получаем строку подключения
var connectionString = string.Format(
    builder.Configuration.GetConnectionString("SqliteConnection") ?? string.Empty,
    builder.Environment.ContentRootPath);
    
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(connectionString)
);

// Внедряем зависимости на Контроллеры
builder.Services.AddTransient<UserModel>();
builder.Services.AddTransient<AdminModel>();

var app = builder.Build();
// app.Environment.EnvironmentName = "Production"; // меняем имя окружения

app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseExceptionHandler("/error");
}
app.UseStaticFiles();
app.UseRouting();
// app.UseAuthorization();

// Создание и заполнение БД при ее отсутствии
var result = Result<bool>.Done(true);
try
{
    using var serviceScope = ((IApplicationBuilder)app)
        .ApplicationServices
        .GetService<IServiceScopeFactory>()
        ?.CreateScope();
    var dbContext = serviceScope?.ServiceProvider.GetRequiredService<AppDbContext>();
    var dbFileName = dbContext?.Database.GetDbConnection().DataSource;
    if (!File.Exists(dbFileName))
    {
        try
        {
            dbContext!.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            result = Result<bool>.Fail(ex);
        }
        if (result)
            DatabaseFiller.Fill(dbContext);     // заполняем БД
    }

    // Сопоставляем маршрутов с контроллерами
    app.MapControllerRoute(
        name: "default",
        pattern: result
            ? "{controller=User}/{action=Index}/{id?}"
            : "{controller=Error}/{action=HandleError}"     // если ошибка
    );
    
    // Если была ошибка - генерируем исключение
    if (!result)
        throw new AppException("Неудачное создание базы данных.", result.Excptn);
}
catch (Exception ex)
{
    // Правильно перенаправляем на страницу ошибки (посредством контроллера ErrorController)
    var query = $"message={ex.Flatten()}" +
                $"&stackTrace={HttpUtility.UrlEncode(ex.StackTrace)}";
    app.UseStatusCodePagesWithReExecute("/error", $"?{query}");
}
finally
{
    app.Run();
}


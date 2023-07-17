using System.Net;
using System.Web;
using App.AppInitializer;
using App.Authorization;
using App.Authorization.Models;
using Domain.DbContexts;
using Domain.Models;
using Infrastructure.AppComponents.AppExceptions;
using Infrastructure.BaseComponents.Components;
using Infrastructure.BaseExtensions;
using Invio.Extensions.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddTransient<BuyerModel>();
builder.Services.AddTransient<AdminModel>();
builder.Services.AddTransient<LoginModel>();

// Подключаем аутентификацию с использованием JWT-токенов
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = LoginManager.GetTokenValidationParameters();
        options.AddQueryStringAuthentication();     // подключаем аутентификацию с исп. параметров адр. строки
                                                    // (nuget: Invio.Extensions.Authentication.JwtBearer)
        options.IncludeErrorDetails = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();
// app.Environment.EnvironmentName = "Production"; // меняем имя окружения

app.UseHttpsRedirection();

// Обработка ошибок HTTP и перехват ошибок 401 и 403
app.UseStatusCodePages(async context => 
{
    var response = context.HttpContext.Response;

    if (response.StatusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden)
    {
        // Если авторизация не прошла - перенаправляем на страницу входа в систему
        // context.HttpContext.Items.Add("Redirected", true);
        response.Redirect("/Admin/Login");
    }
});

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();                          // перенаправление http -> https 
    app.UseExceptionHandler("/error");      // в случае ошибки - перенаправление по адресу "/error"
}
app.UseStaticFiles();
app.UseRouting();

// Механизм добавления JWT-токена в заголовок HTTP-запроса 
app.Use(async (context, next) =>
{
    var jwtStr = 
        LoginManager.GetJwtStr(context);                            // получаем JWT-токен
    var controllerName = 
        context.GetRouteData().Values["controller"]?.ToString();    // название текущего контроллера   
    var actionName = 
        context.GetRouteData().Values["action"]?.ToString();        // название текущего действия

    if (controllerName == "Admin" && actionName == "Index" && context.Request.Method == "GET")
    {
        if (!LoginManager.GetJwsInQueryFlag(context))
            // Для Admin.Index и метода GET
            // добавляем в заголовок, только если признак признак того,
            // что JWS-токен передается через параметры в адресной строке СБРОШЕН
            LoginManager.AddJwtToHeader(context, jwtStr);
    }
    else
    {
        LoginManager.AddJwtToHeader(context, jwtStr);
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

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

    // Сопоставляем маршруты с контроллерами
    app.MapControllerRoute(
        name: "default",
        pattern: result
            ? "{controller=Buyer}/{action=Index}/{id?}"
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


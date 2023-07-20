using System.Net;
using System.Web;
using App.AppInitializer;
using App.Infrastructure.Authorization;
using App.Infrastructure.Authorization.Models;
using App.Infrastructure.DbConfigure;
using App.Infrastructure.Middlewares;
using Domain.DbContexts;
using Domain.Models;
using Infrastructure.BaseExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// builder.Services.AddMvc();

// Получаем Кофигуратор БД
var dbConfigurator = new DbConfigurator(
    builder.Configuration,
    builder.Environment.ContentRootPath
);
// builder.Services.AddSingleton(dbConfigurator);   // внедряем конфигуратор
    
// Внедряем контекст БД
builder.Services.AddDbContext<AppDbContext>(options  =>
    options.UseSqlite(dbConfigurator.ProcessedConnectionString)
);

// Внедряем зависимости на Контроллеры
builder.Services.AddTransient<BuyerModel>();
builder.Services.AddTransient<AdminModel>();
builder.Services.AddSingleton<LoginModel>();

// Подключаем аутентификацию с использованием JWT-токенов
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = LoginManager.GetTokenValidationParameters();
        
        // Можно подключить аутентификацию с исп. параметров адр. строки
        // (nuget: Invio.Extensions.Authentication.JwtBearer)
        // options.AddQueryStringAuthentication();  
        
        // options.IncludeErrorDetails = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();
// app.Environment.EnvironmentName = "Production"; // меняем имя окружения

app.UseHttpsRedirection();

// Обработка ошибок HTTP и перехват ошибки 401 - отсутствие авторизации
app.UseStatusCodePages(context => 
{
    var response = context.HttpContext.Response;

    if (response.StatusCode is 
        (int)HttpStatusCode.Unauthorized)
    {
        // Если авторизация не прошла - перенаправляем на страницу входа в систему
        response.Redirect(context.HttpContext.Request.Method.ToUpper() == "GET"
            ? "/Login"
            : "/AjaxRedirect"
        );
    }

    return Task.CompletedTask;
});

app.UseExceptionHandler("/Error");      // в случае исключения - перенаправление по адресу "/error"

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();                          // перенаправление http -> https 
}
app.UseStaticFiles();
app.UseRouting();

{
// Механизм добавления JWT-токена в заголовок HTTP-запроса

    // Список путей, при совпадении с которыми текущего пути,
    // JWT-токен в заголовок не записывается.
    // Данный список содержит единственный элемент "",
    // если JWT-токен передается через параметры запроса (query). 
    var skipPaths = new List<PathString>();
    app.Use(async (context, next) =>
    {
        if (LoginManager.GetJwtInQueryFlag(context))
            skipPaths = new List<PathString> { new("/Admin") };
        await next.Invoke();
    });
    
    // JWT-токен из куков
    app.UseMiddleware<CookieToHeaderMiddleware>(skipPaths);
    
    // JWT-токен из параметров запроса (при установленном признаке)
    app.UseWhen(LoginManager.GetJwtInQueryFlag,
        appBuilder => appBuilder.UseMiddleware<QueryStringToHeaderMiddleware>()
    );
}

app.UseAuthentication();
app.UseAuthorization();

// Инициализация и обработка ошибки, при ее наличии
try
{
    using var serviceScope = app.Services.CreateScope();

    // Инициализируем приложение
    var result = Initializer.Init(serviceScope.ServiceProvider);
    
    // Если была ошибка - генерируем исключение
    if (!result)
        throw result.Excptn;
    
    // Сопоставляем маршруты с контроллерами
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Buyer}/{action=Index}/{id?}"
    );
}
catch (Exception ex)
{
    // Сопоставляем маршруты с контроллерами (если ошибка)
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Error}/{action=HandleError}"
    );

    // Правильно перенаправляем на страницу ошибки (посредством контроллера ErrorController)
    var query = $"message={ex.Flatten()}" +
                $"&stackTrace={HttpUtility.UrlEncode(ex.StackTrace)}";
    app.UseStatusCodePagesWithReExecute("/Error", $"?{query}");
}
finally
{
    app.Run();
}


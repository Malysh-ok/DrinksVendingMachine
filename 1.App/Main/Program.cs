using App.AppInitializer;
using Domain.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

///////////////////
builder.Services.AddMvc();

// Получаем строку подключения
var connectionString = string.Format(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
    builder.Environment.ContentRootPath);
    
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(connectionString)
);

var app = builder.Build();

using (var serviceScope = ((IApplicationBuilder)app)
       .ApplicationServices
       .GetService<IServiceScopeFactory>()
       ?.CreateScope())
{
    var dbContext = serviceScope?.ServiceProvider.GetRequiredService<AppDbContext>();
    var dbFileName = dbContext?.Database.GetDbConnection().DataSource;
    if (!File.Exists(dbFileName))
    {
        dbContext!.Database.EnsureCreated();
        var result = DatabaseFiller.Fill(dbContext);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=New}/{action=Index}/{id?}");

// app.MapControllerRoute(
//     name: "Admin", 
//     pattern: "{controller=User}/{action=Admin}/{id?}");

app.Run();
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Services;

var builder = WebApplication.CreateBuilder(args);

// Поддержка MVC: контроллеры и представления.
builder.Services.AddControllersWithViews();

// Получение строки подключения.
var connectionString =
    builder.Configuration.GetConnectionString("VerleContext")
    ?? throw new InvalidOperationException(
        "Connection string 'VerleContext' not found.");

// Подключение PostgreSQL.
builder.Services.AddDbContext<VerleContext>(options =>
    options.UseNpgsql(connectionString));

//Пользовательские сервисы
builder.Services.AddScoped<SalesImportService>();
builder.Services.AddScoped<ProductionSpecificationImportService>();

var app = builder.Build();

// В режиме разработки создаём базу, если её нет,
// и применяем существующие миграции.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<VerleContext>();

    context.Database.Migrate();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
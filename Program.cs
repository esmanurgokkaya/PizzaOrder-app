using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using PizzaOrderApp.Data;
using PizzaOrderApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server ve Razor Pages servisleri
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=PizzaDb.db";
builder.Services.AddDbContext<PizzaStoreContext>(options =>
    options.UseSqlite(connectionString));

// Proje Servisleri
builder.Services.AddScoped<PizzaService>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Veritabanı Otomatik Oluşturma 
// Uygulama her açıldığında veritabanı yoksa oluşturur.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PizzaStoreContext>();
        var environment = services.GetRequiredService<IWebHostEnvironment>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Veritabanını oluştur
        context.Database.EnsureCreated();
        
        // Seed data ekle
        SeedData.InitializeAsync(context, environment, logger).Wait();
        
        logger.LogInformation("Veritabanı başarıyla oluşturuldu ve seed data yüklendi.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı oluşturulurken hata: {Error}", ex.Message);
        Console.WriteLine("Veritabanı oluşturulurken hata: " + ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Blazor Server Rotaları
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
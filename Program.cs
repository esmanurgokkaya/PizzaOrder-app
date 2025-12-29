using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using PizzaOrderApp.Data;
using PizzaOrderApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Blazor Server ve Razor Pages servislerini ekleyin
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 2. Veritabanı Bağlantısını Ekleyin (SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=PizzaDb.db";
builder.Services.AddDbContext<PizzaStoreContext>(options =>
    options.UseSqlite(connectionString));

// 3. Kendi Servislerinizi Ekleyin
builder.Services.AddScoped<PizzaService>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// 4. Veritabanı Otomatik Oluşturma (Seed Data)
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

// 5. Blazor Server Rotalarını Ayarlayın
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
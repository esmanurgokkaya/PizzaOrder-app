using Microsoft.EntityFrameworkCore;
using PizzaOrderApp.Data;
using PizzaOrderApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server Services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Entity Framework Core ve SQLite konfigürasyonu
builder.Services.AddDbContext<PizzaStoreContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
    
    // Development ortamında detaylı logging
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Uygulama servisleri - artık Entity Framework kullanıyor
builder.Services.AddScoped<PizzaService>(); 
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // HSTS sadece production'da
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Uygulama başlarken veritabanını oluştur ve seed data'yı yükle
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        // Seed data'yı yükle
        await SeedData.InitializeAsync(context, environment, logger);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı başlatılırken bir hata oluştu.");
        
        // Production ortamında uygulamanın durmasını istemiyorsan bu satırı kaldırabilirsin
        if (app.Environment.IsDevelopment())
        {
            throw; // Development'da hatayı fırlat ki debugging yapabilelim
        }
    }
}

app.Run();
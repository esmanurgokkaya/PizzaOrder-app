// Data/SeedData.cs

using Microsoft.EntityFrameworkCore;
using PizzaOrderApp.Models;
using System.Text.Json;

namespace PizzaOrderApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(PizzaStoreContext context, IWebHostEnvironment environment, ILogger logger)
        {
            try
            {
                // Veritabanının oluşturulduğundan emin ol
                await context.Database.EnsureCreatedAsync();
                
                // Migration'ların uygulandığından emin ol
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Pending migrations uygulanıyor: {Migrations}", 
                        string.Join(", ", pendingMigrations));
                    await context.Database.MigrateAsync();
                }

                // Eğer Pizzas tablosunda veri varsa seeding yapma
                if (await context.Pizzas.AnyAsync())
                {
                    logger.LogInformation("Pizzas tablosunda veri mevcut, seeding atlanıyor.");
                    return;
                }

                logger.LogInformation("Pizzas tablosu boş, seed data yükleniyor...");

                // JSON dosyasının yolunu belirle
                var jsonFilePath = Path.Combine(environment.WebRootPath, "data", "pizzas.json");
                
                if (!File.Exists(jsonFilePath))
                {
                    logger.LogWarning("pizzas.json dosyası bulunamadı: {FilePath}", jsonFilePath);
                    return;
                }

                // JSON dosyasını oku
                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                
                // JSON verilerini deserialize et - mevcut JSON formatına uygun model
                var pizzaDataList = JsonSerializer.Deserialize<List<PizzaJsonModel>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (pizzaDataList == null || !pizzaDataList.Any())
                {
                    logger.LogWarning("pizzas.json dosyası boş veya geçersiz format.");
                    return;
                }

                // Pizza'ları ve boyutlarını veritabanına ekle
                var pizzasToAdd = new List<Pizza>();
                var sizesToAdd = new List<PizzaSize>();

                foreach (var pizzaData in pizzaDataList)
                {
                    // Pizza entity'si oluştur
                    var pizza = new Pizza
                    {
                        Id = pizzaData.Id,
                        Name = pizzaData.Name,
                        BasePrice = pizzaData.BasePrice,
                        ImageUrl = pizzaData.ImageUrl,
                        Ingredients = pizzaData.Ingredients ?? new List<string>()
                    };

                    pizzasToAdd.Add(pizza);

                    // Pizza boyutlarını ekle
                    if (pizzaData.Sizes != null)
                    {
                        foreach (var sizeData in pizzaData.Sizes)
                        {
                            var pizzaSize = new PizzaSize
                            {
                                Id = sizeData.Id,
                                Name = sizeData.Name,
                                Multiplier = sizeData.Multiplier,
                                PizzaId = pizza.Id
                            };
                            sizesToAdd.Add(pizzaSize);
                        }
                    }
                }

                // Bulk insert operations
                await context.Pizzas.AddRangeAsync(pizzasToAdd);
                await context.SaveChangesAsync();

                await context.PizzaSizes.AddRangeAsync(sizesToAdd);
                await context.SaveChangesAsync();

                logger.LogInformation("Seed data başarıyla yüklendi. {PizzaCount} pizza ve {SizeCount} boyut eklendi.",
                    pizzasToAdd.Count, sizesToAdd.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Seed data yüklenirken hata oluştu.");
                throw; // Re-throw to handle in Program.cs if needed
            }
        }
    }

    // JSON dosyasından veri okumak için yardımcı model sınıfları
    public class PizzaJsonModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? Ingredients { get; set; }
        public List<PizzaSizeJsonModel>? Sizes { get; set; }
    }

    public class PizzaSizeJsonModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Multiplier { get; set; }
    }
}
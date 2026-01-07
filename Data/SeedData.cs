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
                foreach (var pizzaData in pizzaDataList)
                {
                    var pizza = new Pizza
                    {
                        Id = pizzaData.Id,
                        Name = pizzaData.Name,
                        BasePrice = pizzaData.BasePrice,
                        ImageUrl = pizzaData.ImageUrl,
                        Ingredients = pizzaData.Ingredients ?? new List<string>()
                    };

                    context.Pizzas.Add(pizza);

                    // Pizza boyutlarını ekle
                    if (pizzaData.Sizes != null)
                    {
                        foreach (var sizeData in pizzaData.Sizes)
                        {
                            var pizzaSize = new PizzaSize
                            {
                                Id = $"{pizza.Id}_{sizeData.Name.ToLowerInvariant()}",
                                Name = sizeData.Name,
                                Multiplier = sizeData.Multiplier,
                                PizzaId = pizza.Id
                            };
                            context.PizzaSizes.Add(pizzaSize);
                        }
                    }
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Seed data başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Seed data yüklenirken hata oluştu.");
                throw;
            }
        }
    }

    // JSON model sınıfları
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
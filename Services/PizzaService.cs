// Services/PizzaService.cs

using System.Net.Http;
using System.Net.Http.Json; // ReadFromJsonAsync için gerekli
using PizzaOrderApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaOrderApp.Services
{
    public class PizzaService
    {
        // HttpClient, Blazor'da API istekleri veya statik dosya okumak için kullanılır.
        private readonly HttpClient _httpClient;
        
        // Statik olarak okunan pizza listesini bellekte tutmak için.
        private List<Pizza>? _pizzas;

        // 1. HttpClient'ı Dependency Injection (DI) aracılığıyla enjekte etme.
        public PizzaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 2. pizzas.json dosyasını asenkron okuyan metot.
        public async Task<List<Pizza>> GetPizzasAsync()
        {
            // Eğer veriler daha önce yüklendiyse, tekrar okumaya gerek yok (performans optimizasyonu).
            if (_pizzas != null)
            {
                return _pizzas;
            }

            try
            {
                // 3. wwwroot/data/pizzas.json yolundan veriyi okuma.
                // HttpClient'ın BaseAddress'i zaten wwwroot dizinine ayarlanmıştır.
                _pizzas = await _httpClient.GetFromJsonAsync<List<Pizza>>("data/pizzas.json");
            }
            catch (HttpRequestException ex)
            {
                // Hata yakalama: Dosya bulunamazsa veya JSON formatı hatalıysa
                Console.WriteLine($"Pizza verileri yüklenirken bir hata oluştu: {ex.Message}");
                _pizzas = new List<Pizza>(); // Boş liste döndür
            }

            return _pizzas ?? new List<Pizza>();
        }

        // Tek bir pizzayı ID'ye göre bulmak için yardımcı metot
        public Pizza? GetPizzaById(string id)
        {
            // Not: Bu metot çağrılmadan önce GetPizzasAsync'in çağrılmış olması beklenir.
            return _pizzas?.FirstOrDefault(p => p.Id == id);
        }
    }
}
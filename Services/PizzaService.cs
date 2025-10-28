using System.Net.Http;
using System.Net.Http.Json;
using PizzaOrderApp.Models; // Ad alanını kontrol edin
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // FirstOrDefault için eklendi
using System; // Console.WriteLine için eklendi

namespace PizzaOrderApp.Services // Ad alanını kontrol edin
{
    public class PizzaService
    {
        private readonly HttpClient _httpClient;
        private List<Pizza>? _pizzas;

        public PizzaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Pizza>> GetPizzasAsync()
        {
            if (_pizzas != null)
            {
                return _pizzas;
            }

            try
            {
                _pizzas = await _httpClient.GetFromJsonAsync<List<Pizza>>("data/pizzas.json");
            }
            catch (Exception ex) // Daha genel hata yakalama
            {
                Console.WriteLine($"Pizza verileri yüklenemedi: {ex.Message}");
                _pizzas = new List<Pizza>();
            }

            return _pizzas ?? new List<Pizza>();
        }

        public async Task<Pizza?> GetPizzaByIdAsync(string id) // Asenkron yapıldı
        {
             // Eğer _pizzas null ise önce yükle
            if (_pizzas == null)
            {
                await GetPizzasAsync();
            }
            return _pizzas?.FirstOrDefault(p => p.Id == id);
        }
    }
}
using System.Net.Http;
using System.Net.Http.Json;
using PizzaOrderApp.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System; 

namespace PizzaOrderApp.Services 
{
    // Pizza verilerini yükleyen servis: `data/pizzas.json`'den menü verilerini getirir ve önbellekler.
    public class PizzaService
    {
        private readonly HttpClient _httpClient;
        private List<Pizza>? _pizzas;

        public PizzaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

    // Menüdeki tüm pizzaları döner; iç önbellek varsa doğrudan onu kullanır.
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
            catch (Exception ex) 
            {
                Console.WriteLine($"Pizza verileri yüklenemedi: {ex.Message}");
                _pizzas = new List<Pizza>();
            }

            return _pizzas ?? new List<Pizza>();
        }

    // Verilen `id` ile önbellekteki pizzayı döndürür, önbellek boşsa önce tüm pizzaları yükler.
    public async Task<Pizza?> GetPizzaByIdAsync(string id) 
        {
            if (_pizzas == null)
            {
                await GetPizzasAsync();
            }
            return _pizzas?.FirstOrDefault(p => p.Id == id);
        }
    }
}
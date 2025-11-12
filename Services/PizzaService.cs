using System.Net.Http;
using System.Net.Http.Json;
using PizzaOrderApp.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System; 

namespace PizzaOrderApp.Services 
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
            catch (Exception ex) 
            {
                Console.WriteLine($"Pizza verileri yüklenemedi: {ex.Message}");
                _pizzas = new List<Pizza>();
            }

            return _pizzas ?? new List<Pizza>();
        }

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
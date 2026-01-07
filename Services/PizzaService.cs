using Microsoft.EntityFrameworkCore;
using PizzaOrderApp.Data;
using PizzaOrderApp.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System; 

namespace PizzaOrderApp.Services 
{
    // Pizza verilerini veritabanından yükleyen servis: Entity Framework ile veri erişimi.
    public class PizzaService
    {
        private readonly PizzaStoreContext _context;

        public PizzaService(PizzaStoreContext context)
        {
            _context = context;
        }

        // Veritabanındaki tüm pizzaları boyutları ile birlikte döner.
        public async Task<List<Pizza>> GetPizzasAsync()
        {
            try
            {
                return await _context.Pizzas
                    .Include(p => p.Sizes)
                    .ToListAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Pizza verileri yüklenemedi: {ex.Message}");
                return new List<Pizza>();
            }
        }

        // Verilen ID ile veritabanından pizza döndürür, boyutları ile birlikte.
        public async Task<Pizza?> GetPizzaByIdAsync(string id) 
        {
            try
            {
                return await _context.Pizzas
                    .Include(p => p.Sizes)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pizza bulunamadı (ID: {id}): {ex.Message}");
                return null;
            }
        }

        // Yeni pizza ekler
        public async Task<bool> AddPizzaAsync(Pizza pizza)
        {
            try
            {
                _context.Pizzas.Add(pizza);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pizza eklenemedi: {ex.Message}");
                return false;
            }
        }

        // Pizza günceller 
        public async Task<bool> UpdatePizzaAsync(Pizza pizza)
        {
            try
            {
                _context.Pizzas.Update(pizza);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pizza güncellenemedi: {ex.Message}");
                return false;
            }
        }

        // Pizza siler 
        public async Task<bool> DeletePizzaAsync(string id)
        {
            try
            {
                var pizza = await GetPizzaByIdAsync(id);
                if (pizza != null)
                {
                    _context.Pizzas.Remove(pizza);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pizza silinemedi: {ex.Message}");
                return false;
            }
        }

        // Pizza sayısını döner
        public async Task<int> GetPizzaCountAsync()
        {
            try
            {
                return await _context.Pizzas.CountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pizza sayısı alınamadı: {ex.Message}");
                return 0;
            }
        }
    }
}
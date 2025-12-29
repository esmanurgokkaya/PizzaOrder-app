using PizzaOrderApp.Data;
using PizzaOrderApp.Models; 
using System;
using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq; 


namespace PizzaOrderApp.Services
{
    // Sipariş durumunu yöneten servis: seçim, fiyat hesaplama, yerel depolama ve veritabanı işlemleri.
    public class OrderService
    {
        // IJSRuntime ile localStorage erişimi sağlar (JS interop için constructor'ta alınır).
        private const decimal ExtraToppingPrice = 10.00m;
        public Order CurrentOrder { get; private set; } = new Order();
        public event Action? OnOrderStateChanged;
        private readonly IJSRuntime? _jsRuntime;
        private readonly PizzaStoreContext _context;

        public OrderService(PizzaStoreContext context, IJSRuntime? jsRuntime = null)
        {
            _context = context;
            _jsRuntime = jsRuntime;
        }

        // Sipariş değiştiğinde fiyatı yeniden hesaplar ve abonelere bildirir.
        public void UpdateOrder()
        {
            Console.WriteLine("UpdateOrder çağrıldı");
            CalculatePrice();
            NotifyStateChanged();
        }

        // Toplam tutarı hesaplar: baz fiyat * boyut çarpanı + ekstra malzeme ücreti.
        // Seçili pizza veya boyut yoksa toplam 0 olur.
        private void CalculatePrice()
        {
            if (CurrentOrder.SelectedPizza == null || CurrentOrder.SelectedSize == null)
            {
                CurrentOrder.TotalPrice = 0;
                return;
            }
            decimal basePrice = CurrentOrder.SelectedPizza.BasePrice;
            double multiplier = CurrentOrder.SelectedSize.Multiplier;
            int extraToppingsCount = CurrentOrder.SelectedToppings.Count;
            decimal toppingsCost = extraToppingsCount * ExtraToppingPrice;
            CurrentOrder.TotalPrice = (basePrice * (decimal)multiplier) + toppingsCost;
            
            // Debug log
            Console.WriteLine($"Fiyat hesaplaması: BasePrice={basePrice}, Multiplier={multiplier}, ToppingsCount={extraToppingsCount}, ToppingsCost={toppingsCost}, Total={CurrentOrder.TotalPrice}");
        }

        // `OnOrderStateChanged` olayını tetikler (UI ve diğer aboneler için güncelleme bildirimi).
        private void NotifyStateChanged()
        {
            OnOrderStateChanged?.Invoke();
        }

    // Mevcut siparişi JSON olarak localStorage'a kaydeder (JS interop kullanır).
    public async Task SaveOrderToLocalStorageAsync()
        {
            if (_jsRuntime == null)
            {
                Console.WriteLine("JSRuntime mevcut değil, localStorage işlemi atlanıyor.");
                return;
            }
            
            try
            {
                 var orderJson = JsonSerializer.Serialize(CurrentOrder);
                 await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "lastCompletedOrder", orderJson);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"LocalStorage'a kaydetme hatası: {ex.Message}");
            }
        }
    // LocalStorage'dan 'lastCompletedOrder' anahtarını okuyup `CurrentOrder`'ı yükler.
    // Başarısız olursa boş bir sipariş ile devam eder.
    public async Task LoadLastOrderFromLocalStorageAsync()
        {
            if (_jsRuntime == null)
            {
                Console.WriteLine("JSRuntime mevcut değil, localStorage işlemi atlanıyor.");
                return;
            }
            
            try
            {
                var orderJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "lastCompletedOrder");
                if (!string.IsNullOrEmpty(orderJson))
                {
                    CurrentOrder = JsonSerializer.Deserialize<Order>(orderJson) ?? new Order();
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"LocalStorage'dan yükleme hatası: {ex.Message}");
            }
        }

        // Yeni sipariş oluştur ve veritabanına kaydet
        public async Task<bool> PlaceOrderAsync(Order order)
        {
            try
            {
                Console.WriteLine("Sipariş veritabanına kaydediliyor...");
                
                // CustomerInfo'yu kontrol et - mevcut bir müşteri var mı?
                CustomerInfo? existingCustomer = null;
                if (!string.IsNullOrEmpty(order.CustomerInfo.Email))
                {
                    existingCustomer = await _context.CustomerInfos
                        .FirstOrDefaultAsync(c => c.Email == order.CustomerInfo.Email);
                }

                if (existingCustomer != null)
                {
                    // Mevcut müşteri varsa onu kullan
                    order.CustomerInfo = existingCustomer;
                    order.CustomerInfoId = existingCustomer.Id;
                    Console.WriteLine($"Mevcut müşteri kullanılıyor: {existingCustomer.Email}");
                }
                else
                {
                    // Yeni müşteri oluştur
                    if (order.CustomerInfo.Id == 0)
                    {
                        _context.CustomerInfos.Add(order.CustomerInfo);
                        await _context.SaveChangesAsync();
                        order.CustomerInfoId = order.CustomerInfo.Id;
                        Console.WriteLine($"Yeni müşteri oluşturuldu: {order.CustomerInfo.Email}");
                    }
                }

                // Order'ı ekle
                order.OrderDate = DateTime.Now;
                
                // OrderNumber set edilmemişse otomatik generate et
                if (string.IsNullOrEmpty(order.OrderNumber))
                {
                    order.OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper();
                }
                
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                Console.WriteLine("Sipariş başarıyla kaydedildi!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sipariş kaydedilemedi: {ex.Message}");
                return false;
            }
        }

        // Mevcut CurrentOrder'ı veritabanına kaydet
        public async Task<bool> PlaceCurrentOrderAsync()
        {
            if (CurrentOrder.SelectedPizza == null || CurrentOrder.SelectedSize == null)
            {
                Console.WriteLine("Eksik sipariş bilgileri.");
                return false;
            }

            // Yeni bir Order nesnesi oluştur (CurrentOrder'ı klonla)
            var newOrder = new Order
            {
                // Id = 0 (yeni kayıt için default)
                OrderNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(), // Benzersiz sipariş numarası
                SelectedPizzaId = CurrentOrder.SelectedPizza.Id,
                SelectedSizeId = CurrentOrder.SelectedSize.Id,
                SelectedToppings = CurrentOrder.SelectedToppings.ToList(), // JSON property otomatik set edilir
                TotalPrice = CurrentOrder.TotalPrice,
                CustomerInfo = new CustomerInfo
                {
                    Name = CurrentOrder.CustomerInfo.Name,
                    Email = CurrentOrder.CustomerInfo.Email,
                    Address = CurrentOrder.CustomerInfo.Address
                }
            };

            return await PlaceOrderAsync(newOrder);
        }

        // Müşteri siparişlerini getir
        public async Task<List<Order>> GetCustomerOrdersAsync(string email)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.CustomerInfo)
                    .Include(o => o.SelectedPizza)
                    .Include(o => o.SelectedSize)
                    .Where(o => o.CustomerInfo.Email == email)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Müşteri siparişleri getirilemedi: {ex.Message}");
                return new List<Order>();
            }
        }

        // Sipariş detayını getir
        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.CustomerInfo)
                    .Include(o => o.SelectedPizza)
                    .Include(o => o.SelectedSize)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sipariş bulunamadı: {ex.Message}");
                return null;
            }
        }

        // Tüm siparişleri getir (admin fonksiyonu)
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.CustomerInfo)
                    .Include(o => o.SelectedPizza)
                    .Include(o => o.SelectedSize)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Siparişler getirilemedi: {ex.Message}");
                return new List<Order>();
            }
        }

        // Sipariş sayısını getir
        public async Task<int> GetOrderCountAsync()
        {
            try
            {
                return await _context.Orders.CountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sipariş sayısı alınamadı: {ex.Message}");
                return 0;
            }
        }

        // Günlük satış toplamını getir
        public async Task<decimal> GetDailySalesAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);

                return await _context.Orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                    .SumAsync(o => o.TotalPrice);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Günlük satış hesaplanamadı: {ex.Message}");
                return 0;
            }
        }
    }
}
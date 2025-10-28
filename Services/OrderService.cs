using PizzaOrderApp.Models; // Ad alanını kontrol edin
using System;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic; // List için eklendi
using System.Linq; // Any için eklendi


namespace PizzaOrderApp.Services // Ad alanını kontrol edin
{
    public class OrderService
    {
        private const decimal ExtraToppingPrice = 10.00m;
        public Order CurrentOrder { get; private set; } = new Order();
        public event Action? OnOrderStateChanged;
        private readonly IJSRuntime _jsRuntime;

        public OrderService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public void UpdateOrder()
        {
            CalculatePrice();
            NotifyStateChanged();
        }

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
        }

        private void NotifyStateChanged()
        {
            OnOrderStateChanged?.Invoke();
        }

        public async Task SaveOrderToLocalStorageAsync()
        {
            try
            {
                 var orderJson = JsonSerializer.Serialize(CurrentOrder);
                 await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "lastCompletedOrder", orderJson);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"LocalStorage'a kaydetme hatası: {ex.Message}");
                 // İsteğe bağlı: Kullanıcıya bir bildirim gösterilebilir.
            }
        }
         // İsteğe bağlı: Son siparişi yüklemek için
        public async Task LoadLastOrderFromLocalStorageAsync()
        {
            try
            {
                var orderJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "lastCompletedOrder");
                if (!string.IsNullOrEmpty(orderJson))
                {
                    // Yüklenen siparişi CurrentOrder'a atayabilirsiniz veya başka bir değişkende tutabilirsiniz.
                    // var lastOrder = JsonSerializer.Deserialize<Order>(orderJson);
                    // CurrentOrder = lastOrder ?? new Order();
                    // NotifyStateChanged(); // Yükleme sonrası arayüzü güncellemek için
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"LocalStorage'dan yükleme hatası: {ex.Message}");
            }
        }
    }
}
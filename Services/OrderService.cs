using PizzaOrderApp.Models; 
using System;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq; 


namespace PizzaOrderApp.Services
{
    // Sipariş durumunu yöneten servis: seçim, fiyat hesaplama ve yerel depolama işlemleri.
    public class OrderService
    {
        // IJSRuntime ile localStorage erişimi sağlar (JS interop için constructor'ta alınır).
        private const decimal ExtraToppingPrice = 10.00m;
        public Order CurrentOrder { get; private set; } = new Order();
        public event Action? OnOrderStateChanged;
        private readonly IJSRuntime _jsRuntime;

        public OrderService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // Sipariş değiştiğinde fiyatı yeniden hesaplar ve abonelere bildirir.
        public void UpdateOrder()
        {
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
        }

        // `OnOrderStateChanged` olayını tetikler (UI ve diğer aboneler için güncelleme bildirimi).
        private void NotifyStateChanged()
        {
            OnOrderStateChanged?.Invoke();
        }

    // Mevcut siparişi JSON olarak localStorage'a kaydeder (JS interop kullanır).
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
            }
        }
    // LocalStorage'dan 'lastCompletedOrder' anahtarını okuyup `CurrentOrder`'ı yükler.
    // Başarısız olursa boş bir sipariş ile devam eder.
    public async Task LoadLastOrderFromLocalStorageAsync()
        {
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
    }
}
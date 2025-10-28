// Services/OrderService.cs

using PizzaOrderApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json; // JSON serileştirme için gerekli
using Microsoft.JSInterop;

namespace PizzaOrderApp.Services
{
    public class OrderService
    {
        // Pizza ve ekstra malzemelerin fiyatlarını tutan sabit (10 TL)
        private const decimal ExtraToppingPrice = 10.00m; 

        // 1. Durumu Tutan Merkez Alan (State)
        public Order CurrentOrder { get; private set; } = new Order();
        private readonly IJSRuntime _jsRuntime;

        public OrderService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // 2. Olay Tanımı: Durum değiştiğinde bu olay tetiklenir (OrderSummary bileşeni için kritik).
        public event Action? OnOrderStateChanged;

        // 3. Durum Güncelleyici Metot: Seçimler değiştiğinde çağrılır.
        public void UpdateOrder()
        {
            // Fiyatı yeniden hesapla (Issue P-202)
            CalculatePrice(); 
            
            // Durumun değiştiğini dinleyen tüm bileşenlere bildir (Issue P-201)
            NotifyStateChanged();
        }
        
        // Fiyatı Anlık Hesaplama Metodu (Issue P-202)
        private void CalculatePrice()
    {
        // Temel kontrol: Pizza ve Boyut seçilmediyse fiyat 0'dır.
        if (CurrentOrder.SelectedPizza == null || CurrentOrder.SelectedSize == null)
        {
            CurrentOrder.TotalPrice = 0;
            return;
        }

        // 1. Baz Fiyatı Al
        decimal basePrice = CurrentOrder.SelectedPizza.BasePrice;

        // 2. Boyut Çarpanını Al
        double multiplier = CurrentOrder.SelectedSize.Multiplier;
        
        // 3. Ek Malzeme Maliyetini Hesapla (Her biri 10 TL)
        // Bu nedenle sadece CurrentOrder.SelectedToppings listesindeki eleman sayısı ek maliyete yol açar.
        int extraToppingsCount = CurrentOrder.SelectedToppings.Count;
        decimal toppingsCost = extraToppingsCount * ExtraToppingPrice;

        // 4. Formülü Uygula
        // Toplam Fiyat = (Baz Fiyat * Boyut Çarpanı) + Ekstra Malzeme Maliyeti
        CurrentOrder.TotalPrice = (basePrice * (decimal)multiplier) + toppingsCost;
    }

        // Bileşenlerin arayüzü güncellemesi için olay tetikleyici
        private void NotifyStateChanged()
        {
            OnOrderStateChanged?.Invoke(); 
        }

       public async Task SaveOrderToLocalStorageAsync()
        {
            // Sipariş nesnesini JSON string'e dönüştür
            var orderJson = JsonSerializer.Serialize(CurrentOrder);

            // localStorage.setItem("key", "value") JavaScript metodunu çağır
            await _jsRuntime.InvokeVoidAsync(
                "localStorage.setItem", 
                "lastCompletedOrder", // Kayıt Anahtarı
                orderJson
            );
        }

        // Uygulama yüklendiğinde son siparişi localStorage'dan yükler. (Ekstra Geliştirme)
        public async Task LoadLastOrderFromLocalStorageAsync()
        {
            // localStorage.getItem("key") JavaScript metodunu çağır
            var orderJson = await _jsRuntime.InvokeAsync<string>(
                "localStorage.getItem", 
                "lastCompletedOrder"
            );

            if (!string.IsNullOrEmpty(orderJson))
            {
                // JSON string'i Order nesnesine geri dönüştür
                var lastOrder = JsonSerializer.Deserialize<Order>(orderJson);

                // Bu, isteğe bağlıdır: Uygulama yeniden yüklendiğinde son siparişi CurrentOrder'a atayabilirsiniz.
                // CurrentOrder = lastOrder ?? new Order(); 
            }
        }
    }
}

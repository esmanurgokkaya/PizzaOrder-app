using PizzaOrderApp.Models; 
using System;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq; 


namespace PizzaOrderApp.Services
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
            }
        }
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
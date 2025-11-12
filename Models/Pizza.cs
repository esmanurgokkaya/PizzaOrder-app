// Models/Pizza.cs

using System.Collections.Generic;

namespace PizzaOrderApp.Models
{
    // Menüdeki bir pizzayı tanımlar: ad, temel fiyat, malzemeler ve desteklenen boyutlar.
    public class Pizza
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Ingredients { get; set; } = new();
        public List<PizzaSize> Sizes { get; set; } = new();
    }
}
// Models/Order.cs

using System.Collections.Generic;

namespace PizzaOrderApp.Models
{
    public class Order
    {
        public Pizza? SelectedPizza { get; set; }
        public PizzaSize? SelectedSize { get; set; }
        public List<string> SelectedToppings { get; set; } = new();
        public decimal TotalPrice { get; set; } = 0;
        public CustomerInfo CustomerInfo { get; set; } = new();
        public string OrderNumber { get; set; } = string.Empty;
    }
}
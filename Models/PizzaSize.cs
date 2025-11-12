// Models/PizzaSize.cs

namespace PizzaOrderApp.Models
{
    // Pizza boyutu ve fiyat çarpanını tutar (ör. Small, Medium, Large).
    public class PizzaSize
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Multiplier { get; set; }
    }
}
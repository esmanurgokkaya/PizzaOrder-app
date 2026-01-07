using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PizzaOrderApp.Models
{
    // Menüdeki bir pizzayı tanımlar
    public class Pizza
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public decimal BasePrice { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        // JSON olarak saklanacak malzemeler listesi
        public string IngredientsJson { get; set; } = "[]";
        
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> Ingredients
        {
            get => string.IsNullOrEmpty(IngredientsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(IngredientsJson) ?? new List<string>();
            set => IngredientsJson = JsonSerializer.Serialize(value);
        }
        
        public virtual ICollection<PizzaSize> Sizes { get; set; } = new List<PizzaSize>();
        
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
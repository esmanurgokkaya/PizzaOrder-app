// Models/Order.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PizzaOrderApp.Models
{
    // Bir siparişin seçili pizza, boyut, ekstra malzemeler ve toplam tutarını tutar.
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        // Foreign Keys
        public string? SelectedPizzaId { get; set; }
        public string? SelectedSizeId { get; set; }
        
        // JSON olarak saklanacak ekstra malzemeler listesi
        public string SelectedToppingsJson { get; set; } = "[]";
        
        [Required]
        public decimal TotalPrice { get; set; } = 0;
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("SelectedPizzaId")]
        public virtual Pizza? SelectedPizza { get; set; }
        
        [ForeignKey("SelectedSizeId")]
        public virtual PizzaSize? SelectedSize { get; set; }
        
        // Müşteri bilgileri - ayrı tablo olarak
        public int CustomerInfoId { get; set; }
        
        [ForeignKey("CustomerInfoId")]
        public virtual CustomerInfo CustomerInfo { get; set; } = new();
        
        // NotMapped property - veritabanında saklanmaz
        [NotMapped]
        public List<string> SelectedToppings
        {
            get => string.IsNullOrEmpty(SelectedToppingsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(SelectedToppingsJson) ?? new List<string>();
            set => SelectedToppingsJson = JsonSerializer.Serialize(value);
        }
    }
}
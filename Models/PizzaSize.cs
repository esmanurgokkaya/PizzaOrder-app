// Models/PizzaSize.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaOrderApp.Models
{
    // Pizza boyutu ve fiyat çarpanını tutar (ör. Small, Medium, Large).
    public class PizzaSize
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public double Multiplier { get; set; }
        
        // Foreign Key - Pizza ile ilişki
        [Required]
        public string PizzaId { get; set; } = string.Empty;
        
        // Navigation property
        [ForeignKey("PizzaId")]
        public virtual Pizza? Pizza { get; set; }
        
        // Navigation property - Order ile ilişki
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
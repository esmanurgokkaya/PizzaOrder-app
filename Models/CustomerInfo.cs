// Models/CustomerInfo.cs

using System.ComponentModel.DataAnnotations;

namespace PizzaOrderApp.Models
{
    public class CustomerInfo
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ad en az 2, en fazla 50 karakter olmalıdır.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres alanı zorunludur.")]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "Adres en az 10, en fazla 250 karakter olmalıdır.")]
        public string Address { get; set; } = string.Empty;
    }
}
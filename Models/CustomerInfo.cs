// Models/CustomerInfo.cs

using System.ComponentModel.DataAnnotations;

namespace PizzaOrderApp.Models
{
    public class CustomerInfo
    {
        // Ad: Zorunlu alan. [Required], [StringLength] gibi öznitelikler Blazor form doğrulamada kullanılır[cite: 1337, 2303].
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ad en az 2, en fazla 50 karakter olmalıdır.")]
        public string Name { get; set; } = string.Empty;

        // E-posta: Zorunlu ve geçerli format kontrolü. [cite_start][EmailAddress] özniteliği kullanılır[cite: 1341, 2304].
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        // Adres: Zorunlu ve uzunluk kısıtlamaları. [cite_start][StringLength] özniteliği kullanılır[cite: 1342].
        [Required(ErrorMessage = "Adres alanı zorunludur.")]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "Adres en az 10, en fazla 250 karakter olmalıdır.")]
        public string Address { get; set; } = string.Empty;
    }
}
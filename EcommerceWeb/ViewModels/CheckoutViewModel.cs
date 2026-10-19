using DatabaseLayer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWeb.ViewModels
{
    public class CheckoutViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Street Address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Phone, StringLength(11, MinimumLength = 11)]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a payment method.")]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
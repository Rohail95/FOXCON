using System.ComponentModel.DataAnnotations;

namespace EcommerceWeb.ViewModels
{
    public class AccountUpdateViewModel
    {
        // This is needed to identify the customer, but not displayed.
        public int CustomerId { get; set; }

        [Required, StringLength(50)]
        public required string FirstName { get; set; }

        [Required, StringLength(50)]
        public required  string LastName { get; set; }

        [Phone] // Ensures it's a valid phone format
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 digits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only digits.")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
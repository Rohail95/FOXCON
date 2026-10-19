using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWeb.ViewModels
{
    public class UserProfileViewModel
    {
        public string? ExistingImageUrl { get; set; }
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }

        

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
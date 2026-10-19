using System.ComponentModel.DataAnnotations;

namespace EcommerceWeb.ViewModels
{
    public class AdminChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class UpdateEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Current Email")]
        public string? Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "New Email")]
        public string? NewEmail { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool IsEmailConfirmed { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class UpdatePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string? OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email or Username")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}


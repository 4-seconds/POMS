using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class SupplierRegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Street")]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "State")]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        [Display(Name = "Business Phone Number")]
        public string BusinessPhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Contact Person Name")]
        public string ContactPersonName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        [Display(Name = "Contact Person Phone Number")]
        public string ContactPersonPhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "TIN Number")]
        public string TinNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Trade License File")]
        public IFormFile TradeLicenseFile { get; set; } = default!;

        [Required]
        [Display(Name = "TIN Number File")]
        public IFormFile TinNumberFile { get; set; } = default!;

        [Required]
        [Display(Name = "Payment Method 1")]
        public string PaymentMethod1 { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Payment Provider 1")]
        public int PaymentMethod1BankId { get; set; }

        [Display(Name = "Payment Method 2")]
        public string? PaymentMethod2 { get; set; }

        [Display(Name = "Payment Provider 2")]
        public int? PaymentMethod2BankId { get; set; }
    }
}


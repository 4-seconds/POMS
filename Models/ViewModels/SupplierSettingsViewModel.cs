using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class SupplierSettingsViewModel
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "TIN Number")]
        public string TinNumber { get; set; } = string.Empty;

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
        [StringLength(200)]
        [Display(Name = "Payment Method 1")]
        public string PaymentMethod1 { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Payment Method 1 Bank ID")]
        public int PaymentMethod1BankId { get; set; }

        [Display(Name = "Payment Method 2")]
        [StringLength(200)]
        public string? PaymentMethod2 { get; set; } = string.Empty;

        [Display(Name = "Payment Method 2 Bank ID")]
        public int? PaymentMethod2BankId { get; set; }

        public SelectList? Banks { get; set; }
    }
}


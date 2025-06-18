using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class UpdatePhoneNumberViewModel
    {
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}


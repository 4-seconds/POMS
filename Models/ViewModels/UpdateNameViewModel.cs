using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class UpdateNameViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
    }
}


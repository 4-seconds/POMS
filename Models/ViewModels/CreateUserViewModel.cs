using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Branch")]
        public Guid? BranchId { get; set; }
    }
} 

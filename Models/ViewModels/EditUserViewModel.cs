using System;
using System.ComponentModel.DataAnnotations;
using PurchaseOrderManagementSystem.Models;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Branch")]
        public Guid? BranchId { get; set; }

        [Required]
        [Display(Name = "Account Status")]
        public AccountStatus AccountStatus { get; set; }

        [Display(Name = "Password Reset Required")]
        public bool PasswordResetRequired { get; set; }
    }
}


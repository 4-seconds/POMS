using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PurchaseOrderManagementSystem.Models
{
    public class Role : IdentityRole
    {
        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Role name can only contain alphabetic characters.")]
        public override string Name { get; set; }
    }
}
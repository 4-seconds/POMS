using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Purchase_Order_Management_System.Models;
using static Enum;

namespace PurchaseOrderManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required] 
        [MaxLength(100)] 
        public string FirstName { get; set; } 

        [Required] 
        [MaxLength(100)] 
        public string LastName { get; set; } 

        public Gender? Gender { get; set; }
        
        public AccountStatus AccountStatus {  get; set; }
        public string SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [Required]
        public string Address { get; set; } 
    }
}

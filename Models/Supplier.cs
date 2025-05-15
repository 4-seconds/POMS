using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using static Enum;
using PurchaseOrderManagementSystem.Models;

namespace Purchase_Order_Management_System.Models
{
    public class Supplier : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SupplierID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(150)]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string TinNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public SupplierStatus Status { get; set; }

        // Navigation properties
        public virtual ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();
        public virtual ICollection<SupplierBranch> Branches { get; set; } = new HashSet<SupplierBranch>();
    }
}

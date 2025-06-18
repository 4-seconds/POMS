using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using PurchaseOrderManagementSystem.Models;

namespace PurchaseOrderManagementSystem.Models
{
    public class Supplier : BaseModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(150)]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string TinNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public SupplierStatus Status { get; set; }

        [StringLength(255)]
        public string? TradeLicenseFilePath { get; set; }

        [StringLength(255)]
        public string? TinNumberFilePath { get; set; }

        [Required]
        [StringLength(200)]
        public string PaymentMethod1 { get; set; } = string.Empty;

        public int PaymentMethod1BankId { get; set; }

        [Required]
        [StringLength(200)]
        public string PaymentMethod2 { get; set; } = string.Empty;

        public int? PaymentMethod2BankId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Navigation properties
        public virtual ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();
        public virtual ICollection<SupplierBranch> Branches { get; set; } = new HashSet<SupplierBranch>();
    }
}

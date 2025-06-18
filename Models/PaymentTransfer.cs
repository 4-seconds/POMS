using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseOrderManagementSystem.Models
{
    public class PaymentTransfer : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string PurchaseOrderId { get; set; }

        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        [Required]
        public string InitiatedById { get; set; }

        [ForeignKey("InitiatedById")]
        public virtual ApplicationUser InitiatedBy { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; }

        [StringLength(255)]
        public string Reference { get; set; }

        [Required]
        public int BankCode { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string AccountName { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // e.g., "Pending", "Approved", "Failed", "Completed"

        [StringLength(255)]
        public string TransactionId { get; set; } // Chapa's transaction ID

        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public new DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}


using PurchaseOrderManagementSystem.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Enum;

namespace PurchaseOrderManagementSystem.Models
{
    public class Tender : BaseModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string PurchaseRequestId { get; set; }

        [ForeignKey("PurchaseRequestId")]
        public virtual PurchaseRequest PurchaseRequest { get; set; }

        [Required]
        public DateTime TenderEndDate { get; set; }

        [Required]
        public DateTime DeliveryDeadline { get; set; }

        [Required]
        public TenderStatus Status { get; set; } // Assuming you'll define a TenderStatus enum

        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public new DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}


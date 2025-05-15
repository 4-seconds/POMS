using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Purchase_Order_Management_System.Models;
using PurchaseOrderManagementSystem.Models;
using static Enum;

namespace PurchaseOrderManagementSystem.Models
{
    public class GoodsReceived : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string PurchaseRequestId { get; set; }

        [ForeignKey("PurchaseRequestId")]
        public virtual PurchaseRequestOrder PurchaseRequest { get; set; }

        [Required]
        public string ReceivedById { get; set; }

        [ForeignKey("ReceivedById")]
        public virtual ApplicationUser ReceivedBy { get; set; }

        [Required]
        public DateTime ReceivedDate { get; set; }

        [Required]
        public string BidId { get; set; }

        [ForeignKey("BidId")]
        public virtual Bid Bid { get; set; }

        [Required]
        public UOM UOM { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        [Required]
        public string Status { get; set; } = "Received";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}

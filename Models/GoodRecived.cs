using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PurchaseOrderManagementSystem.Models;

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
        public virtual PurchaseRequest PurchaseRequest { get; set; }

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
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }


        [Required]
        public string Status { get; set; } = "Received";

        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public new DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

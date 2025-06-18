using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseOrderManagementSystem.Models
{
    public class Bid : BaseModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string SupplierId { get; set; }

        [Required]
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [Required]
        public string AuctionId { get; set; }

        [Required]
        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime? DeliveredDate { get; set; }

        [Required]
        public BidStatus Status { get; set; }

        [Required]
        public string PurchaseRequestId { get; set; }

        [Required]
        [ForeignKey("PurchaseRequestId")]
        public virtual PurchaseRequest PurchaseRequest { get; set; }

        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public new DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for GoodsReceived relationship
        public virtual ICollection<GoodsReceived> GoodsReceived { get; set; } = new HashSet<GoodsReceived>();
    }
}

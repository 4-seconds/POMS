using PurchaseOrderManagementSystem.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Enum;

namespace PurchaseOrderManagementSystem.Models
{
    public class Bid : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BidId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [Required]
        public string TenderId { get; set; }
        [ForeignKey("TenderId")]
        public virtual Tender Tender { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BidAmount { get; set; }

        [Required]
        public string Currency { get; set; } = "Birr";

        [Required]
        public string PaymentTerms { get; set; } = "Cash on Delivery";

        public string DeliveryLocation { get; set; }

        public string Remarks { get; set; }

        public BidStatus Status { get; set; }

        [Required]
        public string PurchaseRequestId { get; set; }

        [ForeignKey("PurchaseRequestId")]
        public virtual PurchaseRequest PurchaseRequest { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime? DeliveredDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for GoodsReceived relationship
        public virtual ICollection<GoodsReceived> GoodsReceived { get; set; } = new HashSet<GoodsReceived>();
    }
}

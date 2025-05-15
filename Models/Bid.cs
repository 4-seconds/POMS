using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Purchase_Order_Management_System.Models;
using PurchaseOrderManagementSystem.Models;

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
        public string PurchaseRequestId { get; set; }

        [ForeignKey("PurchaseRequestId")]
        public virtual PurchaseRequestOrder PurchaseRequest { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime? DeliveredDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        // Navigation property for GoodsReceived relationship
        public virtual ICollection<GoodsReceived> GoodsReceived { get; set; } = new HashSet<GoodsReceived>();
    }
}

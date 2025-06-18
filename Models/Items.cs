using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseOrderManagementSystem.Models
{
    public class Item : BaseModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Unit")]
        public string Unit { get; set; }

        [Required]
        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public new DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public virtual ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
        public virtual ICollection<GoodsReceived> GoodsReceived { get; set; } = new List<GoodsReceived>();
    }
}

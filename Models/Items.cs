using Purchase_Order_Management_System.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Enum;

namespace PurchaseOrderManagementSystem.Models
{
    public class Item : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ItemId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Current Stock")]
        public int CurrentStock { get; set; }

        [Required]
        [Display(Name = "Unit of Measure")]
        public UOM UOM { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<PurchaseRequestOrder> PurchaseRequests { get; set; }
        public virtual ICollection<GoodsReceived> GoodsReceived { get; set; }
    }
}

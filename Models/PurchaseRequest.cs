using PurchaseOrderManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Enum;

namespace PurchaseOrderManagementSystem.Models
{
    public class PurchaseRequest : BaseModel
    {
        [Key]
        public string purchaseRequestId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser User { get; set; }



        public string ExistingItemId { get; set; }

        [ForeignKey("existingItemId")]
        public virtual Item existingItem { get; set; }

        [Required]
        public string Item { get; set; }

        [Required]
        public string ItemDescription { get; set; }

        [Required]
        public UOM Unit { get; set; }

        [Required]
        public int quantity { get; set; }

        [Required]
        public string remark { get; set; }

        [Required]
        public string reviwed_by { get; set; }

        [ForeignKey("reviwed_by")]
        public ApplicationUser reviewd_by { get; set; }

        [Required]
        public string BudgetComment { get; set; }

        [Required]
        public PurchaseRequestStatus Status { get; set; }



    }
}

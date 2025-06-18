using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseOrderManagementSystem.Models
{
    public class PurchaseRequest : BaseModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("existingItem")]
        public string ExistingItemId { get; set; }

        [Required]
        public virtual Item existingItem { get; set; }

        [Required]
        public int quantity { get; set; }

        public string? ReviewedComment { get; set; }

        public string? BudgetComment { get; set; }

        [Required]
        public PurchaseRequestStatus Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Properties for the view
        public string ItemName => existingItem?.ItemName ?? "N/A";
        public int Quantity => quantity;

        /// <summary>
        /// Gets or sets the ID of the branch associated with the purchase request.
        /// This is a foreign key to the Branch model.
        /// </summary>
        [ForeignKey("Branch")]
        public Guid? BranchId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the Branch associated with the purchase request.
        /// </summary>
        public Branch? Branch { get; set; }
    }
}

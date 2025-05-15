using PurchaseOrderManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace Purchase_Order_Management_System.Models
{
    public class Category : BaseModel
    {
        [Key]
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        // Navigation property
        public ICollection<Item> Items { get; set; }
    }
}

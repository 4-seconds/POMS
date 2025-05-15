using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Purchase_Order_Management_System.Models;

namespace Purchase_Order_Management_System.Models
{
    public class SupplierBranch : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BranchId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string BranchName { get; set; }

        [Required]
        public string SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }
    }
}

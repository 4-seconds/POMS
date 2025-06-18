using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseOrderManagementSystem.Models
{
    public class Branch : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string BranchName { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(20)]
        public string ContactNumber { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
    }
}


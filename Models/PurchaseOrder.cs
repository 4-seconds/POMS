using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PurchaseOrderManagementSystem.Models;
namespace Purchase_Order_Management_System.Models
{
    public class PurchaseOrder : BaseModel
    {
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        public string RequestId { get; set; }
        [ForeignKey ("RequestId")]
        public PurchaseOrder purchaseOrder { get; set; }

        public string BidId { get; set; }

        [ForeignKey ("BidId")]
        public Bid Bid { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        public string OrderedBy { get; set; }

        [ForeignKey ("OrderedBy")]

        public  ApplicationUser ApplicationUser { get; set; }
        public string RequestedBy { get; set; }

        [ForeignKey ("RequestedBy")]
        public ApplicationUser Requested_by { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }

        public int Quantity { get; set; }

        public float UnitPrice { get; set; }

        public float TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }
}

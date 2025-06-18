using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PurchaseOrderManagementSystem.Models
{
    public class Auction : BaseModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string PurchaseRequestId { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime DeliveryDeadline { get; set; }

        [Required]
        public AuctionStatus Status { get; set; }

        [ForeignKey("PurchaseRequestId")]
        public virtual PurchaseRequest PurchaseRequest { get; set; } = null!;

        public virtual ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();
    }
}


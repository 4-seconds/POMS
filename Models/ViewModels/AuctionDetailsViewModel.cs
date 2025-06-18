using PurchaseOrderManagementSystem.Models;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class AuctionDetailsViewModel
    {
        public Auction Auction { get; set; } = null!;
        public Bid? SupplierBid { get; set; }
    }
} 
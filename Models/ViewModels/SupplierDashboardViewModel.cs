using PurchaseOrderManagementSystem.Models;

namespace PurchaseOrderManagementSystem.Models.ViewModels
{
    public class SupplierDashboardViewModel
    {
        public Supplier Supplier { get; set; }
        public List<Auction> ActiveAuctions { get; set; }
        public List<Bid> SupplierBids { get; set; }
    }
} 
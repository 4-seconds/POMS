using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PurchaseOrderManagementSystem.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupplierController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // Check supplier status
            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            // Get active auctions
            var activeAuctions = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .Include(a => a.PurchaseRequest.existingItem)
                .Where(a => a.Status == AuctionStatus.Open && a.EndDate > DateTime.UtcNow)
                .ToListAsync();


            // Get supplier's bids
            var supplierBids = await _context.Bids
                .Include(b => b.Auction)
                .Include(b => b.Auction.PurchaseRequest)
                .Include(b => b.Auction.PurchaseRequest.existingItem)
                .Where(b => b.SupplierId == supplier.Id)
                .ToListAsync();

            var viewModel = new SupplierDashboardViewModel
            {
                Supplier = supplier,
                ActiveAuctions = activeAuctions,
                SupplierBids = supplierBids
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ActiveAuctions()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // Check supplier status
            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            var auctions = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .Where(a => a.Status == AuctionStatus.Open && a.EndDate > DateTime.UtcNow)
                .ToListAsync();
            return View(auctions);
        }

        public async Task<IActionResult> MyBids()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // Check supplier status
            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            var bids = await _context.Bids
                .Include(b => b.Auction)
                .Include(b => b.Auction.PurchaseRequest)
                .Where(b => b.SupplierId == supplier.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bids);
        }

        public async Task<IActionResult> MyPurchaseOrders()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // Check supplier status
            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            var purchaseOrders = await _context.PurchaseOrders
                .Include(po => po.Bid)
                .ThenInclude(b => b.Auction)
                .ThenInclude(a => a.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .Where(po => po.Bid.SupplierId == supplier.Id)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();

            return View(purchaseOrders);
        }

        public async Task<IActionResult> ViewAuction(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // Check supplier status
            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            var auction = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .Include(a => a.PurchaseRequest.existingItem)
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            // Get supplier's bid for this auction if any
            var supplierBid = auction.Bids.FirstOrDefault(b => b.SupplierId == supplier.Id);

            var viewModel = new AuctionDetailsViewModel
            {
                Auction = auction,
                SupplierBid = supplierBid
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(string id, decimal price)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // Check supplier status
            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            var auction = await _context.Auctions
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            // Check if auction is still active
            if (auction.Status != AuctionStatus.Open || auction.EndDate <= DateTime.UtcNow)
            {
                TempData["Error"] = "This auction is no longer active.";
                return RedirectToAction(nameof(ViewAuction), new { id });
            }

            // Check if a bid already exists for this supplier and auction
            var existingBid = await _context.Bids
                .FirstOrDefaultAsync(b => b.AuctionId == id && b.SupplierId == supplier.Id);

            if (existingBid != null)
            {
                // Update existing bid
                existingBid.Price = price;
                existingBid.UpdatedAt = DateTime.UtcNow;
                _context.Bids.Update(existingBid);
            }
            else
            {
                // Create a new bid
                var bid = new Bid
                {
                    Id = Guid.NewGuid().ToString(),
                    AuctionId = auction.Id,
                    SupplierId = supplier.Id,
                    Price = price,
                    Status = BidStatus.Open,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PurchaseRequestId = auction.PurchaseRequestId
                };
                _context.Bids.Add(bid);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your bid has been placed successfully.";
            return RedirectToAction(nameof(ViewAuction), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePurchaseOrderStatus(string id, string status)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // Check supplier status
            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Bid)
                .FirstOrDefaultAsync(po => po.OrderId == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            // Verify that the current supplier owns this purchase order
            if (purchaseOrder.Bid.SupplierId != supplier.Id)
            {
                return Unauthorized();
            }

            purchaseOrder.Status = status;
            if (status == "Ready for Pickup")
            {
                purchaseOrder.Bid.DeliveredDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyPurchaseOrders));
        }

        public async Task<IActionResult> Transactions()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            if (supplier.Status != SupplierStatus.Active)
            {
                return RedirectToAction("PendingVerification", "Account");
            }

            var transactions = await _context.PaymentTransfers
                .Include(pt => pt.PurchaseOrder)
                    .ThenInclude(po => po.Bid)
                        .ThenInclude(b => b.Supplier)
                .Include(pt => pt.InitiatedBy)
                .Where(pt => pt.PurchaseOrder.Bid.SupplierId == supplier.Id)
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();

            return View(transactions);
        }
    }
}

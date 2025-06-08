using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Enum;

namespace PurchaseOrderManagementSystem.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var auctions = await _context.Tenders
                .Include(a => a.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .Where(a => a.Status == TenderStatus.Open && a.TenderEndDate > DateTime.UtcNow)
                .ToListAsync();
            return View(auctions);
        }

        public async Task<IActionResult> ViewAuction(string id)
        {
            var auction = await _context.Tenders
                .Include(a => a.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            // Also fetch existing bids by the current supplier for this auction
            var currentSupplierId = User.Identity.Name; // This is the username, not the ApplicationUser ID
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == currentSupplierId);

            Bid? existingBid = null;
            if (supplier != null)
            {
                existingBid = await _context.Bids
                    .Where(b => b.TenderId == id && b.SupplierId == supplier.SupplierID)
                    .FirstOrDefaultAsync();
            }

            return View((auction, existingBid));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(string id, decimal price)
        {
            var auction = await _context.Tenders.FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            var currentUserId = User.Identity.Name;
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == currentUserId);

            if (supplier == null)
            {
                // Handle case where supplier is not found (e.g., redirect to login or error)
                return Unauthorized();
            }

            // Check if a bid already exists for this supplier and auction
            var existingBid = await _context.Bids
                .FirstOrDefaultAsync(b => b.TenderId == id && b.SupplierId == supplier.SupplierID);

            if (existingBid != null)
            {
                // Update existing bid
                existingBid.UnitPrice = price;
                existingBid.UpdatedAt = DateTime.UtcNow;
                _context.Bids.Update(existingBid);
            }
            else
            {
                // Create a new bid
                var bid = new Bid
                {
                    BidId = Guid.NewGuid().ToString(),
                    TenderId = auction.Id,
                    SupplierId = supplier.SupplierID,
                    UnitPrice = price,
                    Status = BidStatus.Open,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Bids.Add(bid);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewAuction), new { id = auction.Id });
        }

        public async Task<IActionResult> MyBids()
        {
            var bids = await _context.Bids
                .Include(b => b.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .Where(b => b.SupplierId == User.Identity.Name)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bids);
        }
    }
}

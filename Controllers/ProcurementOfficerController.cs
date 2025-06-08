using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using System;
using System.Threading.Tasks;
using static Enum;

namespace PurchaseOrderManagementSystem.Controllers
{
    public class ProcurementOfficerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProcurementOfficerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .Where(pr => pr.Status == PurchaseRequestStatus.Approved)
                .ToListAsync();
            return View(requests);
        }

        public async Task<IActionResult> CreateAuction(string id)
        {
            var request = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .FirstOrDefaultAsync(pr => pr.purchaseRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAuction(string id, DateTime auctionEndDate, DateTime deliveryDeadline)
        {
            var request = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .FirstOrDefaultAsync(pr => pr.purchaseRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Create a new auction
            var auction = new Tender
            {
                Id = Guid.NewGuid().ToString(),
                PurchaseRequestId = id,
                TenderEndDate = auctionEndDate,
                DeliveryDeadline = deliveryDeadline,
                Status = TenderStatus.Open, // Corrected to TenderStatus
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tenders.Add(auction); // Corrected to Auctions DbSet
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

            var bids = await _context.Bids
                .Include(b => b.Supplier)
                .Where(b => b.TenderId == id)
                .ToListAsync();

            // Pass both the auction and its bids to the view
            return View((auction, bids));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseAuction(string id)
        {
            var auction = await _context.Tenders
                .Include(a => a.PurchaseRequest)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            auction.Status = TenderStatus.Closed;
            auction.UpdatedAt = DateTime.UtcNow;

            // Find the winning bid (lowest price) for this auction
            var winningBid = await _context.Bids
                .Include(b => b.Supplier)
                .Where(b => b.TenderId == id)
                .OrderBy(b => b.UnitPrice)
                .FirstOrDefaultAsync();

            if (winningBid != null)
            {
                winningBid.Status = BidStatus.Awarded;

                // Create a PurchaseOrder from the winning bid
                var purchaseOrder = new PurchaseOrder
                {
                    OrderId = Guid.NewGuid().ToString(),
                    RequestId = auction.PurchaseRequestId,
                    BidId = winningBid.BidId,
                    OrderDate = DateTime.UtcNow,
                    Unit = auction.PurchaseRequest.existingItem.UOM.ToString(),
                    Quantity = auction.PurchaseRequest.quantity,
                    UnitPrice = (float)winningBid.UnitPrice,
                    TotalPrice = (float)(winningBid.UnitPrice * auction.PurchaseRequest.quantity),
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending" // Initial status for PurchaseOrder
                };

                _context.PurchaseOrders.Add(purchaseOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

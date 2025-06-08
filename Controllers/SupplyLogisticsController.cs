using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using static Enum;

namespace PurchaseOrderManagementSystem.Controllers
{
    public class SupplyLogisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplyLogisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Items Management
        public async Task<IActionResult> Index()
        {
            var recentItems = await _context.Items
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentRequests = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .OrderByDescending(pr => pr.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(Tuple.Create(recentItems.AsEnumerable(), recentRequests.AsEnumerable()));
        }

        // Items Management
        public async Task<IActionResult> Items()
        {
            var items = await _context.Items
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
            return View(items);
        }

        public IActionResult CreateItem()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(Item item)
        {
            if (ModelState.IsValid)
            {
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;
                _context.Add(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Item created successfully.";
                return RedirectToAction(nameof(Items));
            }
            return View(item);
        }

        // Purchase Requests
        public async Task<IActionResult> Requests()
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
            return View(requests);
        }

        public async Task<IActionResult> CreateRequest()
        {
            var items = await _context.Items
                .OrderBy(i => i.ItemName)
                .ToListAsync();
            ViewBag.Items = new SelectList(items, "Id", "ItemName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(PurchaseRequest request)
        {
            request.purchaseRequestId = Guid.NewGuid().ToString();
            request.Status = PurchaseRequestStatus.Pending;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            _context.Add(request);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Purchase request created successfully.";
            return RedirectToAction(nameof(Requests));
        }

        public async Task<IActionResult> ViewPurchaseRequest(string id)
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
    }
}

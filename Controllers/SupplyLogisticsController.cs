using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace PurchaseOrderManagementSystem.Controllers
{
    [Authorize(Roles = "SupplyLogistics")]
    public class SupplyLogisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupplyLogisticsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var requests = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .Where(pr => pr.CreatedByUserId == currentUser.Id)
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
            ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "BranchName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(PurchaseRequest request)
        {
            // Inline item creation logic
            var newItemName = Request.Form["NewItemName"].ToString();
            var newItemDescription = Request.Form["NewItemDescription"].ToString();
            var newItemUnit = Request.Form["NewItemUnit"].ToString();

            if (!string.IsNullOrWhiteSpace(newItemName) && !string.IsNullOrWhiteSpace(newItemUnit))
            {
                var newItem = new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    ItemName = newItemName,
                    Description = newItemDescription,
                    Unit = newItemUnit,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Items.Add(newItem);
                await _context.SaveChangesAsync();
                request.ExistingItemId = newItem.Id;
            }

            request.Id = Guid.NewGuid().ToString();
            request.Status = PurchaseRequestStatus.Pending;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            request.BranchId = currentUser.BranchId;
            request.CreatedByUserId = currentUser.Id;

            _context.Add(request);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Purchase request created successfully.";
            return RedirectToAction(nameof(Requests));
        }

        public async Task<IActionResult> ViewPurchaseRequest(string id)
        {
            var request = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }
    }
}

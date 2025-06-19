using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PurchaseOrderManagementSystem.Controllers
{
    [Authorize(Roles = "Budget")]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .Where(pr => pr.BudgetComment == null)
                .ToListAsync();
            return View(requests);
        }

        public async Task<IActionResult> Review(string id)
        {
            var request = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            // Fetch GoodsReceived records for the current item
            var goodsReceivedRecords = await _context.GoodsReceived
                .Include(gr => gr.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .Where(gr => gr.PurchaseRequest.ExistingItemId == request.ExistingItemId)
                .OrderByDescending(gr => gr.ReceivedDate)
                .ToListAsync();

            return View((request, goodsReceivedRecords));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(string id, string comment)
        {
            var request = await _context.PurchaseRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.BudgetComment = comment;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

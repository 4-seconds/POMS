using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PurchaseOrderManagementSystem.Controllers
{
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
                .FirstOrDefaultAsync(pr => pr.purchaseRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
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

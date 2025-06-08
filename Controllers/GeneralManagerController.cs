using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using System.Threading.Tasks;
using static Enum;

namespace PurchaseOrderManagementSystem.Controllers
{
    public class GeneralManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GeneralManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .Where(pr => pr.Status == PurchaseRequestStatus.Pending && !string.IsNullOrEmpty(pr.BudgetComment))
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
        public async Task<IActionResult> Approve(string id)
        {
            var request = await _context.PurchaseRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = PurchaseRequestStatus.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(string id, string comment)
        {
            var request = await _context.PurchaseRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = PurchaseRequestStatus.Rejected;
            request.remark = comment;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

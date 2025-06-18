using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PurchaseOrderManagementSystem.Models;

namespace PurchaseOrderManagementSystem.Controllers
{
    // [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("SupplyLogistics"))
                {
                    return RedirectToAction("Index", "SupplyLogistics");
                }
                else if (roles.Contains("Budget"))
                {
                    return RedirectToAction("Index", "Budget");
                }
                else if (roles.Contains("GeneralManager"))
                {
                    return RedirectToAction("Index", "GeneralManager");
                }
                else if (roles.Contains("ProcurementOfficer"))
                {
                    return RedirectToAction("Index", "ProcurementOfficer");
                }
                else if (roles.Contains("Supplier"))
                {
                    return RedirectToAction("Index", "Supplier");
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

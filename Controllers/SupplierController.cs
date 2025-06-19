using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

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
                .Include(b => b.Auction.PurchaseRequest.existingItem)
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
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            // Get supplier's bid for this auction if any
            var supplierBid = await _context.Bids
                .Include(b => b.Auction)
                .Include(b => b.Auction.PurchaseRequest)
                .FirstOrDefaultAsync(b => b.AuctionId == id && b.SupplierId == supplier.Id);
            if (supplierBid == null)
            {
                Console.WriteLine("No bid found");
            }

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

        public async Task<IActionResult> Settings()
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

            var viewModel = new SupplierSettingsViewModel
            {
                BusinessName = supplier.BusinessName,
                ContactPerson = supplier.ContactPerson,
                ContactEmail = supplier.ContactEmail,
                PhoneNumber = supplier.PhoneNumber,
                TinNumber = string.IsNullOrEmpty(supplier.TinNumber) ? supplier.Id : supplier.TinNumber,
                Street = supplier.Street,
                City = supplier.City,
                State = supplier.State,
                Country = supplier.Country,
                PaymentMethod1 = supplier.PaymentMethod1,
                PaymentMethod1BankId = supplier.PaymentMethod1BankId,
                PaymentMethod2 = supplier.PaymentMethod2,
                PaymentMethod2BankId = supplier.PaymentMethod2BankId,
                Banks = GetBanksSelectList(supplier.PaymentMethod1BankId)
            };

            return View(viewModel);
        }

        private SelectList GetBanksSelectList(int? selectedId)
        {
            string banksJson = @"[

    {
      ""id"": 130,
      ""slug"": ""abay_bank"",
      ""swift"": ""ABAYETAA"",
      ""name"": ""Abay Bank"",
      ""acct_length"": 16,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": null,
      ""created_at"": ""2023-01-24T04:28:30.000000Z"",
      ""updated_at"": ""2024-08-03T08:10:24.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 772,
      ""slug"": ""addis_int_bank"",
      ""swift"": ""ABSCETAA"",
      ""name"": ""Addis International Bank"",
      ""acct_length"": 15,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-08-12T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-12T04:21:18.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 207,
      ""slug"": ""ahadu_bank"",
      ""swift"": ""AHUUETAA"",
      ""name"": ""Ahadu Bank"",
      ""acct_length"": 10,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-08-12T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-12T04:21:18.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 656,
      ""slug"": ""awash_bank"",
      ""swift"": ""AWINETAA"",
      ""name"": ""Awash Bank"",
      ""acct_length"": 14,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 0,
      ""active"": 1,
      ""is_24hrs"": 0,
      ""created_at"": ""2022-03-17T04:21:30.000000Z"",
      ""updated_at"": ""2024-08-02T20:08:46.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 347,
      ""slug"": ""boa_bank"",
      ""swift"": ""ABYSETAA"",
      ""name"": ""Bank of Abyssinia"",
      ""acct_length"": 8,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 0,
      ""active"": 1,
      ""is_24hrs"": 0,
      ""created_at"": ""2022-07-04T21:33:57.000000Z"",
      ""updated_at"": ""2024-08-02T20:08:45.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 571,
      ""slug"": ""berhan_bank"",
      ""swift"": ""BERHETAA"",
      ""name"": ""Berhan Bank"",
      ""acct_length"": 13,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-08-12T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-12T04:21:18.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 128,
      ""slug"": ""cbebirr"",
      ""swift"": ""CBETETAA"",
      ""name"": ""CBEBirr"",
      ""acct_length"": 10,
      ""country_id"": 1,
      ""is_mobilemoney"": 1,
      ""is_active"": 1,
      ""is_rtgs"": null,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-01-24T14:41:12.000000Z"",
      ""updated_at"": ""2024-08-12T20:16:07.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 946,
      ""slug"": ""cbe_bank"",
      ""swift"": ""CBETETAA"",
      ""name"": ""Commercial Bank of Ethiopia (CBE)"",
      ""acct_length"": 13,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": null,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2022-03-17T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-03T05:56:23.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 893,
      ""slug"": ""ebirr"",
      ""swift"": ""CBORETA"",
      ""name"": ""Coopay-Ebirr"",
      ""acct_length"": 10,
      ""country_id"": 1,
      ""is_mobilemoney"": 1,
      ""is_active"": 1,
      ""is_rtgs"": null,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2023-08-15T08:00:11.000000Z"",
      ""updated_at"": ""2024-08-10T14:30:16.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 880,
      ""slug"": ""dashen_bank"",
      ""swift"": ""DASHETAA"",
      ""name"": ""Dashen Bank"",
      ""acct_length"": 13,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 0,
      ""active"": 1,
      ""is_24hrs"": 0,
      ""created_at"": ""2022-11-15T03:17:43.000000Z"",
      ""updated_at"": ""2024-08-02T20:08:46.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 301,
      ""slug"": ""global_bank"",
      ""swift"": ""DEGAETAA"",
      ""name"": ""Global Bank Ethiopia"",
      ""acct_length"": 13,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-08-12T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-12T04:21:18.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 534,
      ""slug"": ""hibret_bank"",
      ""swift"": ""UNTDETAA"",
      ""name"": ""Hibret Bank"",
      ""acct_length"": 16,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 0,
      ""active"": 1,
      ""is_24hrs"": 0,
      ""created_at"": ""2023-01-06T03:18:43.000000Z"",
      ""updated_at"": ""2024-08-02T20:08:46.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 315,
      ""slug"": ""anbesa_bank"",
      ""swift"": ""LIBSETAA"",
      ""name"": ""Lion International Bank"",
      ""acct_length"": 9,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-08-12T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-12T04:21:18.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 266,
      ""slug"": ""mpesa"",
      ""swift"": ""MPESA"",
      ""name"": ""M-Pesa"",
      ""acct_length"": 10,
      ""country_id"": 1,
      ""is_mobilemoney"": 1,
      ""is_active"": 1,
      ""is_rtgs"": null,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-01-18T14:41:12.000000Z"",
      ""updated_at"": ""2024-08-02T20:08:57.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 979,
      ""slug"": ""nib_bank"",
      ""swift"": ""NIBIETAA"",
      ""name"": ""Nib International Bank"",
      ""acct_length"": 13,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-08-12T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-12T04:21:18.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 423,
      ""slug"": ""oromia_bank"",
      ""swift"": ""ORIRETAA"",
      ""name"": ""Oromia International Bank"",
      ""acct_length"": 12,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2024-08-12T04:21:18.000000Z"",
      ""updated_at"": ""2024-08-12T04:21:18.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 855,
      ""slug"": ""telebirr"",
      ""swift"": ""TELEBIRR"",
      ""name"": ""telebirr"",
      ""acct_length"": 10,
      ""country_id"": 1,
      ""is_mobilemoney"": 1,
      ""is_active"": 1,
      ""is_rtgs"": null,
      ""active"": 1,
      ""is_24hrs"": 1,
      ""created_at"": ""2022-12-12T14:41:12.000000Z"",
      ""updated_at"": ""2024-08-02T20:08:57.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 472,
      ""slug"": ""wegagen_bank"",
      ""swift"": ""WEGAETAA"",
      ""name"": ""Wegagen Bank"",
      ""acct_length"": 13,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 0,
      ""created_at"": ""2022-11-15T03:16:40.000000Z"",
      ""updated_at"": ""2024-08-12T20:15:43.000000Z"",
      ""currency"": ""ETB""
    },
    {
      ""id"": 687,
      ""slug"": ""zemen_bank"",
      ""swift"": ""ZEMEETAA"",
      ""name"": ""Zemen Bank"",
      ""acct_length"": 16,
      ""country_id"": 1,
      ""is_mobilemoney"": null,
      ""is_active"": 1,
      ""is_rtgs"": 1,
      ""active"": 1,
      ""is_24hrs"": 0,
      ""created_at"": ""2022-09-30T12:56:53.000000Z"",
      ""updated_at"": ""2024-08-12T20:14:40.000000Z"",
      ""currency"": ""ETB""
    }
]";
            var banks = JsonConvert.DeserializeObject<List<dynamic>>(banksJson)!;
            return new SelectList(banks, "id", "name", selectedId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SupplierSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Banks = GetBanksSelectList(model.PaymentMethod1BankId);
                return View(model);
            }

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

            supplier.BusinessName = model.BusinessName;
            supplier.ContactPerson = model.ContactPerson;
            supplier.ContactEmail = model.ContactEmail;
            supplier.PhoneNumber = model.PhoneNumber;
            if (string.IsNullOrEmpty(model.TinNumber))
            {
                supplier.TinNumber = supplier.Id; // Set TinNumber to Supplier.Id if it's null or empty
            }
            else
            {
                supplier.TinNumber = model.TinNumber;
            }
            supplier.Street = model.Street;
            supplier.City = model.City;
            supplier.State = model.State;
            supplier.Country = model.Country;
            supplier.PaymentMethod1 = model.PaymentMethod1;
            supplier.PaymentMethod1BankId = model.PaymentMethod1BankId;
            supplier.PaymentMethod2 = model.PaymentMethod2;
            supplier.PaymentMethod2BankId = model.PaymentMethod2BankId;
            supplier.UpdatedAt = DateTime.UtcNow;

            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your settings have been updated successfully.";
            return RedirectToAction(nameof(Settings));
        }
    }
}

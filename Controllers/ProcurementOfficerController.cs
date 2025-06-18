using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Newtonsoft.Json;

namespace PurchaseOrderManagementSystem.Controllers
{
    [Authorize(Roles = "ProcurementOfficer")]
    public class ProcurementOfficerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProcurementOfficerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ApprovedPurchaseRequests()
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .Include(pr => pr.Branch)
                .Where(pr => pr.Status == PurchaseRequestStatus.Approved || pr.Status == PurchaseRequestStatus.AuctionCreated || pr.Status == PurchaseRequestStatus.AuctionCreated)
                .ToListAsync();
            return View(requests);
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

        public async Task<IActionResult> CreateAuction(string id)
        {
            var request = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            // Prevent creating another auction if one already exists for this purchase request
            var existingAuction = await _context.Auctions.AnyAsync(a => a.PurchaseRequestId == id);
            if (existingAuction)
            {
                TempData["ErrorMessage"] = "An auction has already been created for this purchase request.";
                return RedirectToAction(nameof(ViewPurchaseRequest), new { id = id });
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAuction(string id, DateTime auctionEndDate, DateTime deliveryDeadline)
        {
            var request = await _context.PurchaseRequests
                .Include(pr => pr.existingItem)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            // Prevent creating another auction if one already exists for this purchase request
            var existingAuction = await _context.Auctions.AnyAsync(a => a.PurchaseRequestId == id);
            if (existingAuction)
            {
                TempData["ErrorMessage"] = "An auction has already been created for this purchase request.";
                return RedirectToAction(nameof(ViewPurchaseRequest), new { id = id });
            }

            // Create a new auction
            var auction = new Auction
            {
                Id = Guid.NewGuid().ToString(),
                PurchaseRequestId = id,
                StartDate = DateTime.UtcNow,
                EndDate = auctionEndDate,
                DeliveryDeadline = deliveryDeadline,
                Status = AuctionStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Auctions.Add(auction);

            // Update the status of the purchase request
            request.Status = PurchaseRequestStatus.AuctionCreated;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Auction created successfully!";
            return RedirectToAction(nameof(ViewAuction), new { id = auction.Id });
        }

        public async Task<IActionResult> ViewAuction(string id)
        {
            var auction = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            var bids = await _context.Bids
                .Include(b => b.Supplier)
                .Where(b => b.AuctionId == id)
                .ToListAsync();

            // Pass both the auction and its bids to the view
            return View((auction, bids));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseAuction(string id)
        {
            var auction = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            auction.Status = AuctionStatus.Closed;
            auction.UpdatedAt = DateTime.UtcNow;

            // Find the winning bid (lowest price) for this auction
            var winningBid = await _context.Bids
                .Include(b => b.Supplier)
                .Where(b => b.AuctionId == id)
                .OrderBy(b => b.Price)
                .FirstOrDefaultAsync();

            if (winningBid != null)
            {
                winningBid.Status = BidStatus.Won;

                // Create a PurchaseOrder from the winning bid
                var purchaseOrder = new PurchaseOrder
                {
                    OrderId = Guid.NewGuid().ToString(),
                    RequestId = auction.PurchaseRequestId,
                    BidId = winningBid.Id,
                    OrderDate = DateTime.UtcNow,
                    Unit = auction.PurchaseRequest.existingItem.Unit,
                    Quantity = auction.PurchaseRequest.quantity,
                    UnitPrice = (float)winningBid.Price,
                    TotalPrice = (float)(winningBid.Price * auction.PurchaseRequest.quantity),
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending" // Initial status for PurchaseOrder
                };

                _context.PurchaseOrders.Add(purchaseOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageBids(string id)
        {
            var auction = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            var bids = await _context.Bids
                .Include(b => b.Supplier)
                .Where(b => b.AuctionId == id)
                .OrderBy(b => b.Price)
                .ToListAsync();

            ViewBag.Auction = auction;
            return View(bids);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectWinningBid(string auctionId, string bidId)
        {
            var auction = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .ThenInclude(pr => pr.existingItem)
                .FirstOrDefaultAsync(a => a.Id == auctionId);

            if (auction == null)
            {
                return NotFound();
            }

            var winningBid = await _context.Bids
                .Include(b => b.Supplier)
                .FirstOrDefaultAsync(b => b.Id == bidId && b.AuctionId == auctionId);

            if (winningBid == null)
            {
                return NotFound();
            }

            // Close the auction and set the winning bid
            auction.Status = AuctionStatus.Closed;
            auction.UpdatedAt = DateTime.UtcNow;
            winningBid.Status = BidStatus.Won;

            // Set all other bids for this auction to Lost
            var otherBids = await _context.Bids
                .Where(b => b.AuctionId == auctionId && b.Id != bidId)
                .ToListAsync();

            foreach (var bid in otherBids)
            {
                bid.Status = BidStatus.Lost;
            }

            // Create a PurchaseOrder from the winning bid
            var purchaseOrder = new PurchaseOrder
            {
                OrderId = Guid.NewGuid().ToString(),
                RequestId = auction.PurchaseRequestId,
                BidId = winningBid.Id,
                OrderDate = DateTime.UtcNow,
                Unit = auction.PurchaseRequest.existingItem.Unit,
                Quantity = auction.PurchaseRequest.quantity,
                UnitPrice = (float)winningBid.Price,
                TotalPrice = (float)(winningBid.Price * auction.PurchaseRequest.quantity),
                CreatedAt = DateTime.UtcNow,
                Status = "Pending", // Initial status for PurchaseOrder
                OrderedBy = _userManager.GetUserId(User)
            };

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewAuction), new { id = auctionId });
        }

        public async Task<IActionResult> EditAuctionDeadlines(string id)
        {
            var auction = await _context.Auctions
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAuctionDeadlines(string id, DateTime auctionEndDate, DateTime deliveryDeadline)
        {
            var auctionToUpdate = await _context.Auctions
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auctionToUpdate == null)
            {
                return NotFound();
            }

            auctionToUpdate.EndDate = auctionEndDate;
            auctionToUpdate.DeliveryDeadline = deliveryDeadline;
            auctionToUpdate.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewAuction), new { id = id });
        }

        public async Task<IActionResult> AllAuctions()
        {
            var auctions = await _context.Auctions
                .Include(a => a.PurchaseRequest)
                .Include(a => a.PurchaseRequest.existingItem)
                .Include(a => a.Bids)
                .ToListAsync();
            return View(auctions);
        }

        public async Task<IActionResult> MyPurchaseOrders()
        {
            var purchaseOrders = await _context.PurchaseOrders
                .Include(po => po.PurchaseRequest)
                    .ThenInclude(pr => pr.existingItem)
                .Include(po => po.Bid)
                    .ThenInclude(b => b.Supplier)
                .Include(po => po.OrderedByUser)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();

            return View(purchaseOrders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePurchase(string purchaseOrderId)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.PurchaseRequest)
                .Include(po => po.Bid)
                    .ThenInclude(b => b.Supplier)
                .FirstOrDefaultAsync(po => po.OrderId == purchaseOrderId);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // 1. Create GoodsReceived record
                var goodsReceived = new GoodsReceived
                {
                    Id = Guid.NewGuid().ToString(),
                    PurchaseRequestId = purchaseOrder.RequestId,
                    ReceivedById = _userManager.GetUserId(User),
                    ReceivedDate = DateTime.UtcNow,
                    BidId = purchaseOrder.BidId,
                    Quantity = purchaseOrder.Quantity,
                    UnitPrice = (decimal)purchaseOrder.UnitPrice,
                    TotalPrice = (decimal)purchaseOrder.TotalPrice,
                    Status = "Received"
                };
                _context.GoodsReceived.Add(goodsReceived);

                // 2. Update PurchaseOrder status
                purchaseOrder.Status = "Completed";
                purchaseOrder.UpdatedAt = DateTime.UtcNow;

                // 3. Initiate Payment Transfer via Chapa API
                var chapaSecretKey = _configuration["Chapa:SecretKey"];
                if (string.IsNullOrEmpty(chapaSecretKey))
                {
                    TempData["ErrorMessage"] = "Chapa secret key is not configured.";
                    // Manually revert in-memory changes if API key is missing
                    _context.Entry(goodsReceived).State = EntityState.Detached;
                    _context.Entry(purchaseOrder).State = EntityState.Unchanged;
                    return; // Exit the lambda, the outer method will handle redirection
                }

                if (purchaseOrder.Bid?.Supplier == null)
                {
                    TempData["ErrorMessage"] = "Supplier details are missing for payment.";
                    // Manually revert in-memory changes if supplier details are missing
                    _context.Entry(goodsReceived).State = EntityState.Detached;
                    _context.Entry(purchaseOrder).State = EntityState.Unchanged;
                    return; // Exit the lambda, the outer method will handle redirection
                }

                string accountName = purchaseOrder.Bid.Supplier.BusinessName;
                string accountNumber;
                int bankCode;

                // Prioritize PaymentMethod1
                if (!string.IsNullOrEmpty(purchaseOrder.Bid.Supplier.PaymentMethod1) && purchaseOrder.Bid.Supplier.PaymentMethod1BankId != 0)
                {
                    // Assuming PaymentMethod1 stores account number and PaymentMethod1BankId stores bank code
                    accountNumber = purchaseOrder.Bid.Supplier.PaymentMethod1; // This needs to be parsed from PaymentMethod1 or stored separately
                    bankCode = purchaseOrder.Bid.Supplier.PaymentMethod1BankId;
                }
                // Fallback to PaymentMethod2 if PaymentMethod1 is not available
                else if (!string.IsNullOrEmpty(purchaseOrder.Bid.Supplier.PaymentMethod2) && purchaseOrder.Bid.Supplier.PaymentMethod2BankId.HasValue && purchaseOrder.Bid.Supplier.PaymentMethod2BankId.Value != 0)
                {
                    accountNumber = purchaseOrder.Bid.Supplier.PaymentMethod2; // This needs to be parsed from PaymentMethod2 or stored separately
                    bankCode = purchaseOrder.Bid.Supplier.PaymentMethod2BankId.Value;
                }
                else
                {
                    TempData["ErrorMessage"] = "No valid payment method found for the supplier. Please ensure Payment Method 1 or 2 is correctly set with bank details.";
                    // Manually revert in-memory changes if no valid payment method is found
                    _context.Entry(goodsReceived).State = EntityState.Detached;
                    _context.Entry(purchaseOrder).State = EntityState.Unchanged;
                    return; // Exit the lambda, the outer method will handle redirection
                }

                var paymentTransfer = new PaymentTransfer
                {
                    Id = Guid.NewGuid().ToString(),
                    PurchaseOrderId = purchaseOrder.OrderId,
                    InitiatedById = _userManager.GetUserId(User),
                    Amount = (decimal)purchaseOrder.TotalPrice,
                    Currency = "ETB",
                    Reference = $"PO-{purchaseOrder.OrderId.Replace("-", "")}",
                    BankCode = bankCode,
                    AccountNumber = accountNumber,
                    AccountName = accountName,
                    Status = "Pending",
                    TransactionId = string.Empty, // Initialize to empty string to prevent null exception
                    CreatedAt = DateTime.UtcNow
                };
                _context.PaymentTransfers.Add(paymentTransfer);

                // Prepare Chapa API request
                var chapaRequest = new
                {
                    account_name = paymentTransfer.AccountName,
                    account_number = paymentTransfer.AccountNumber,
                    amount = paymentTransfer.Amount.ToString(),
                    currency = "ETB",
                    reference = paymentTransfer.Reference,
                    bank_code = paymentTransfer.BankCode
                };

                // Log the chapaRequest object for debugging
                Console.WriteLine($"Chapa Request: {JsonConvert.SerializeObject(chapaRequest)}");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", chapaSecretKey);
                var content = new StringContent(JsonConvert.SerializeObject(chapaRequest), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.chapa.co/v1/transfers", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    dynamic chapaResponse = JsonConvert.DeserializeObject(responseContent);

                    if (chapaResponse.status == "success")
                    {
                        paymentTransfer.Status = "Completed";
                        paymentTransfer.TransactionId = chapaResponse.data; // Assuming Chapa returns a transaction ID
                        TempData["SuccessMessage"] = "Purchase completed and payment initiated successfully!";
                    }
                    else
                    {
                        paymentTransfer.Status = "Failed";
                        TempData["ErrorMessage"] = $"Payment initiation failed: {chapaResponse.message}";
                        // Manually revert in-memory changes if Chapa API call fails
                        _context.Entry(goodsReceived).State = EntityState.Detached;
                        _context.Entry(purchaseOrder).State = EntityState.Unchanged;
                        _context.Entry(paymentTransfer).State = EntityState.Detached;
                    }
                }
                else
                {
                    paymentTransfer.Status = "Failed";
                    var errorResponseContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Error initiating payment: {response.ReasonPhrase}. Details: {errorResponseContent}";
                    Console.WriteLine($"Chapa API Error: {errorResponseContent}");
                    // Manually revert in-memory changes if Chapa API call fails
                    _context.Entry(goodsReceived).State = EntityState.Detached;
                    _context.Entry(purchaseOrder).State = EntityState.Unchanged;
                    _context.Entry(paymentTransfer).State = EntityState.Detached;
                }

                await _context.SaveChangesAsync();
            });

            return RedirectToAction(nameof(MyPurchaseOrders));
        }

        public async Task<IActionResult> ReceivedGoods()
        {
            var receivedGoods = await _context.GoodsReceived
                .Include(gr => gr.PurchaseRequest)
                .Include(gr => gr.PurchaseRequest.existingItem)
                .Include(gr => gr.ReceivedBy)
                .OrderByDescending(gr => gr.ReceivedDate)
                .ToListAsync();

            return View(receivedGoods);
        }
    }
}

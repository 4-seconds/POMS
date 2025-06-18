using Microsoft.AspNetCore.Identity;
using PurchaseOrderManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace PurchaseOrderManagementSystem.Data
{
    public class DbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<DbSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding process...");

                // Seed roles
                await SeedRolesAsync();

                // Seed admin user
                await SeedAdminUserAsync();

                // Seed Branches
                await SeedBranchesAsync();

                // Seed sample users for each role
                await SeedSampleUsersAsync();

                // Seed items
                await SeedItemsAsync();

                // Seed purchase requests
                await SeedPurchaseRequestsAsync();

                // Seed auctions
                await SeedAuctionsAsync();

                // Seed bids
                await SeedBidsAsync();

                // Seed GoodsReceived
                await SeedGoodsReceivedAsync();

                // Seed PaymentTransfers
                await SeedPaymentTransfersAsync();

                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Starting role seeding...");
            string[] roleNames = { "Admin", "GeneralManager", "ProcurementOfficer", "Supplier", "SupplyLogistics", "Budget" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogInformation($"Creating role: {roleName}");
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Successfully created role: {roleName}");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError($"Failed to create role {roleName}: {errors}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Role already exists: {roleName}");
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            _logger.LogInformation("Starting admin user seeding...");

            // Check if admin user already exists
            var adminUser = await _userManager.FindByEmailAsync("admin@system.com");

            if (adminUser == null)
            {
                _logger.LogInformation("Creating admin user...");
                // Create admin user
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@system.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    AccountStatus = AccountStatus.Active
                };

                // Create the admin user with password
                var result = await _userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    _logger.LogInformation("Admin user created successfully, assigning Admin role...");
                    // Add admin role to user
                    var roleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation("Admin role assigned successfully");
                    }
                    else
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        _logger.LogError($"Failed to assign Admin role: {errors}");
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to create admin user: {errors}");
                }
            }
            else
            {
                _logger.LogInformation("Admin user already exists");
            }
        }

        private async Task SeedItemsAsync()
        {
            _logger.LogInformation("Starting item seeding...");

            if (!await _context.Items.AnyAsync())
            {
                var items = new List<Item>
                {
                    // Office Supplies
                    new Item { ItemName = "A4 Paper", Description = "Standard A4 size paper", Unit = "Ream" },
                    new Item { ItemName = "Ballpoint Pens", Description = "Blue ballpoint pens", Unit = "Box" },
                    new Item { ItemName = "Stapler", Description = "Standard office stapler", Unit = "Piece" },

                    // IT Equipment
                    new Item { ItemName = "Laptop", Description = "Business laptop", Unit = "Piece" },
                    new Item { ItemName = "Printer", Description = "Office printer", Unit = "Piece" },
                    new Item { ItemName = "External Hard Drive", Description = "1TB external hard drive", Unit = "Piece" },

                    // Furniture
                    new Item { ItemName = "Office Chair", Description = "Ergonomic office chair", Unit = "Piece" },
                    new Item { ItemName = "Desk", Description = "Standard office desk", Unit = "Piece" },
                    new Item { ItemName = "Filing Cabinet", Description = "4-drawer filing cabinet", Unit = "Piece" },

                    // Cleaning Supplies
                    new Item { ItemName = "All-Purpose Cleaner", Description = "Multi-surface cleaner, 1 liter", Unit = "Bottle" },
                    new Item { ItemName = "Paper Towels", Description = "Absorbent paper towels, 6 rolls", Unit = "Pack" },
                    new Item { ItemName = "Hand Soap", Description = "Liquid hand soap with dispenser", Unit = "Bottle" },

                    // Kitchen Supplies
                    new Item { ItemName = "Coffee Beans", Description = "Arabica coffee beans, 1kg", Unit = "Bag" },
                    new Item { ItemName = "Sugar", Description = "Granulated sugar, 5kg", Unit = "Bag" },
                    new Item { ItemName = "Tea Bags", Description = "Assorted black tea bags, 100 count", Unit = "Box" },

                    // Maintenance Tools
                    new Item { ItemName = "Screwdriver Set", Description = "Assorted screwdriver set", Unit = "Set" },
                    new Item { ItemName = "Hammer", Description = "Claw hammer, 16 oz", Unit = "Piece" },
                    new Item { ItemName = "Measuring Tape", Description = "Retractable measuring tape, 5m", Unit = "Piece" },

                    // Safety Equipment
                    new Item { ItemName = "First Aid Kit", Description = "Basic first aid kit for office use", Unit = "Kit" },
                    new Item { ItemName = "Fire Extinguisher", Description = "ABC dry chemical fire extinguisher", Unit = "Piece" },
                    new Item { ItemName = "Safety Glasses", Description = "Protective eyewear", Unit = "Pair" },

                    // Miscellaneous
                    new Item { ItemName = "Batteries AA", Description = "Alkaline AA batteries, 12-pack", Unit = "Pack" },
                    new Item { ItemName = "Light Bulbs LED", Description = "LED light bulbs, E27, 60W equivalent", Unit = "Pack" },
                    new Item { ItemName = "Extension Cord", Description = "3-meter extension cord with multiple outlets", Unit = "Piece" }
                };

                await _context.Items.AddRangeAsync(items);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Items seeded successfully");
            }
            else
            {
                _logger.LogInformation("Items already exist");
            }
        }

        private async Task SeedSampleUsersAsync()
        {
            _logger.LogInformation("Starting sample user seeding...");

            var sampleUsers = new[]
            {
                new { Email = "supply.log@system.com", FirstName = "Supply", LastName = "Logistics", Role = "SupplyLogistics", Password = "Supply.Log@123" },
                new { Email = "budget@system.com", FirstName = "Budget", LastName = "Control", Role = "Budget", Password = "Budget@123" },
                new { Email = "manager@system.com", FirstName = "General", LastName = "Manager", Role = "GeneralManager", Password = "Manager@123" },
                new { Email = "procurement@system.com", FirstName = "Procurement", LastName = "Officer", Role = "ProcurementOfficer", Password = "Proc@123" },
                new { Email = "supplier1@gmail.com", FirstName = "Supplier", LastName = "One", Role = "Supplier", Password = "Supplier@1" },
                new { Email = "supplier2@gmail.com", FirstName = "Supplier", LastName = "Two", Role = "Supplier", Password = "Supplier@2" },
                new { Email = "supplier3@gmail.com", FirstName = "Supplier", LastName = "Three", Role = "Supplier", Password = "Supplier@3" },
                new { Email = "supplier4@gmail.com", FirstName = "Supplier", LastName = "Four", Role = "Supplier", Password = "Supplier@4" },
                new { Email = "supplier5@gmail.com", FirstName = "Supplier", LastName = "Five", Role = "Supplier", Password = "Supplier@5" },
                new { Email = "supplier6@gmail.com", FirstName = "Supplier", LastName = "Six", Role = "Supplier", Password = "Supplier@6" },
            };
            var bankIds = new[] {

       130,
       772,
       207,
       656,
       347,
       571,
       128,
       946,
       893,
       880,
       301,
       534,
       315,
       266,
       979,
       423,
       855,
       472,
       687,
            };
            foreach (var userInfo in sampleUsers)
            {
                var user = await _userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        EmailConfirmed = true,
                        AccountStatus = AccountStatus.Active
                    };

                    var result = await _userManager.CreateAsync(user, userInfo.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, userInfo.Role);
                        _logger.LogInformation($"Created sample user: {userInfo.Email} with role {userInfo.Role}");

                        // Assign a branch to SupplyLogistics users
                        if (userInfo.Role == "SupplyLogistics")
                        {
                            var branches = await _context.Branches.ToListAsync();
                            if (branches.Any())
                            {
                                var randomBranch = branches[new Random().Next(branches.Count)];
                                user.BranchId = randomBranch.Id;
                                await _userManager.UpdateAsync(user);
                                _logger.LogInformation($"Assigned branch {randomBranch.BranchName} to SupplyLogistics user {user.Email}");
                            }
                        }

                        // If the user is a supplier, create a corresponding Supplier entry
                        if (userInfo.Role == "Supplier")
                        {
                            int bankIdIndex = new Random().Next(bankIds.Length);

                            var newSupplier = new Supplier
                            {
                                Id = Guid.NewGuid().ToString(),
                                UserId = user.Id,
                                BusinessName = $"{userInfo.FirstName} {userInfo.LastName} Co",
                                ContactPerson = $"{userInfo.FirstName} {userInfo.LastName}",
                                ContactEmail = userInfo.Email,
                                PhoneNumber = "+2519" + (new Random().Next(10000000, 99999999)).ToString(),
                                Street = $"{userInfo.FirstName} Street",
                                City = "Addis Ababa",
                                State = "Addis Ababa",
                                Country = "Ethiopia",
                                Address = $"{userInfo.FirstName} Street, Addis Ababa", // This can be a concatenation of Street, City, State, Country
                                Status = SupplierStatus.Active,
                                PaymentMethod1 = (new Random().Next(100000000, 999999999)).ToString(),
                                PaymentMethod1BankId = bankIds[bankIdIndex],
                                CreatedAt = DateTime.UtcNow
                            };
                            await _context.Suppliers.AddAsync(newSupplier);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Created supplier entry for user: {userInfo.Email}");
                        }
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError($"Failed to create sample user {userInfo.Email}: {errors}");
                    }
                }
            }
        }

        private async Task SeedPurchaseRequestsAsync()
        {
            _logger.LogInformation("Starting purchase request seeding...");

            if (!await _context.PurchaseRequests.AnyAsync())
            {
                var items = await _context.Items.ToListAsync();
                var supplyLogisticsUser = await _userManager.FindByEmailAsync("supply.log@system.com");

                if (items.Any() && supplyLogisticsUser != null)
                {
                    var purchaseRequests = new List<PurchaseRequest>();
                    var random = new Random();
                    var branches = await _context.Branches.ToListAsync();

                    for (int i = 0; i < 20; i++)
                    {
                        var randomItem = items[random.Next(items.Count)];
                        var randomBranch = branches[random.Next(branches.Count)];
                        purchaseRequests.Add(new PurchaseRequest
                        {
                            Id = Guid.NewGuid().ToString(),
                            ExistingItemId = randomItem.Id,
                            quantity = random.Next(1, 100), // Random quantity
                            Status = (PurchaseRequestStatus)random.Next(0, Enum.GetNames(typeof(PurchaseRequestStatus)).Length), // Random status
                            CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 60)), // Random creation date within last 60 days
                            UpdatedAt = DateTime.UtcNow,
                            BranchId = randomBranch.Id // Assign a random branch
                        });
                    }

                    await _context.PurchaseRequests.AddRangeAsync(purchaseRequests);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Purchase requests seeded successfully");
                }
            }
            else
            {
                _logger.LogInformation("Purchase requests already exist");
            }
        }

        private async Task SeedAuctionsAsync()
        {
            _logger.LogInformation("Starting auction seeding...");

            if (!await _context.Auctions.AnyAsync())
            {
                var purchaseRequests = await _context.PurchaseRequests.ToListAsync();
                var random = new Random();

                if (purchaseRequests.Any())
                {
                    var auctions = new List<Auction>();

                    foreach (var pr in purchaseRequests.Take(20)) // Create auctions for a subset of requests
                    {
                        var startDate = DateTime.UtcNow.AddDays(-random.Next(1, 5));
                        var endDate = startDate.AddDays(random.Next(10, 25));
                        var deliveryDeadline = endDate.AddDays(random.Next(5, 20));

                        auctions.Add(new Auction
                        {
                            Id = Guid.NewGuid().ToString(),
                            PurchaseRequestId = pr.Id,
                            StartDate = startDate,
                            EndDate = endDate,
                            DeliveryDeadline = deliveryDeadline,
                            Status = (random.Next(1, 101) <= 80) ? AuctionStatus.Open : (random.Next(0, 2) == 0 ? AuctionStatus.Closed : AuctionStatus.Cancelled), // 80% chance of Open, 10% Closed, 10% Cancelled
                            CreatedAt = startDate,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    await _context.Auctions.AddRangeAsync(auctions);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Auctions seeded successfully");
                }
            }
            else
            {
                _logger.LogInformation("Auctions already exist");
            }
        }

        private async Task SeedBidsAsync()
        {
            _logger.LogInformation("Starting bid seeding...");

            if (!await _context.Bids.AnyAsync())
            {
                var auctions = await _context.Auctions.Include(a => a.PurchaseRequest).ToListAsync();
                var suppliers = await _context.Suppliers.ToListAsync();
                var random = new Random();

                if (auctions.Any() && suppliers.Any())
                {
                    var bids = new List<Bid>();

                    foreach (var auction in auctions.Take(15)) // Create bids for a subset of auctions
                    {
                        // Each auction can have multiple bids from different suppliers
                        var numBids = random.Next(1, Math.Min(3, suppliers.Count)); // 1 to 3 bids per auction
                        var bidders = suppliers.OrderBy(x => random.Next()).Take(numBids).ToList();

                        foreach (var supplier in bidders)
                        {
                            bids.Add(new Bid
                            {
                                Id = Guid.NewGuid().ToString(),
                                SupplierId = supplier.Id,
                                AuctionId = auction.Id,
                                Price = (decimal)(random.NextDouble() * 1000 + 50), // Random price between 50 and 1050
                                Status = auction.Status == AuctionStatus.Open ? BidStatus.Open : (BidStatus)random.Next(0, Enum.GetNames(typeof(BidStatus)).Length), // Random status
                                PurchaseRequestId = auction.PurchaseRequestId,
                                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    await _context.Bids.AddRangeAsync(bids);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Bids seeded successfully");
                }
            }
            else
            {
                _logger.LogInformation("Bids already exist");
            }
        }

        private async Task SeedGoodsReceivedAsync()
        {
            _logger.LogInformation("Starting GoodsReceived seeding...");

            if (!await _context.GoodsReceived.AnyAsync())
            {
                var purchaseOrders = await _context.PurchaseOrders
                    .Include(po => po.PurchaseRequest)
                    .Include(po => po.OrderedByUser)
                    .ToListAsync();
                var random = new Random();

                if (purchaseOrders.Any())
                {
                    var goodsReceivedList = new List<GoodsReceived>();

                    foreach (var po in purchaseOrders.Take(10)) // Seed for a subset of purchase orders
                    {
                        if (po.OrderedByUser != null && po.PurchaseRequest != null)
                        {
                            goodsReceivedList.Add(new GoodsReceived
                            {
                                Id = Guid.NewGuid().ToString(),
                                PurchaseRequestId = po.RequestId,
                                ReceivedById = po.OrderedBy,
                                ReceivedDate = DateTime.UtcNow.AddDays(random.Next(-5, 0)), // Received recently,
                                BidId = po.BidId,
                                Quantity = po.Quantity,
                                UnitPrice = (decimal)po.UnitPrice,
                                TotalPrice = (decimal)po.TotalPrice,
                                Status = "Received",
                                CreatedAt = DateTime.UtcNow.AddDays(-10)
                            });
                        }
                    }

                    await _context.GoodsReceived.AddRangeAsync(goodsReceivedList);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("GoodsReceived seeded successfully.");
                }
            }
            else
            {
                _logger.LogInformation("GoodsReceived already exist.");
            }
        }

        private async Task SeedPaymentTransfersAsync()
        {
            _logger.LogInformation("Starting PaymentTransfer seeding...");

            if (!await _context.PaymentTransfers.AnyAsync())
            {
                var purchaseOrders = await _context.PurchaseOrders
                    .Include(po => po.OrderedByUser)
                    .Include(po => po.Bid)
                        .ThenInclude(b => b.Supplier)
                    .ToListAsync();
                var random = new Random();

                if (purchaseOrders.Any())
                {
                    var paymentTransfers = new List<PaymentTransfer>();

                    foreach (var po in purchaseOrders.Take(10)) // Seed for a subset of purchase orders
                    {
                        if (po.OrderedByUser != null && po.Bid?.Supplier != null)
                        {
                            paymentTransfers.Add(new PaymentTransfer
                            {
                                Id = Guid.NewGuid().ToString(),
                                PurchaseOrderId = po.OrderId,
                                InitiatedById = po.OrderedBy,
                                Amount = (decimal)po.TotalPrice,
                                Currency = "ETB",
                                Reference = $"PO-{po.OrderId}-PAY-{random.Next(1000, 9999)}",
                                BankCode = random.Next(100, 999),
                                AccountNumber = "ACC" + random.Next(10000000, 99999999),
                                AccountName = po.Bid.Supplier.BusinessName,
                                Status = (random.Next(0, 2) == 0) ? "Completed" : "Pending", // Randomly completed or pending
                                TransactionId = "TRX" + Guid.NewGuid().ToString().Substring(0, 8),
                                CreatedAt = DateTime.UtcNow.AddDays(random.Next(-10, 0))
                            });
                        }
                    }

                    await _context.PaymentTransfers.AddRangeAsync(paymentTransfers);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("PaymentTransfers seeded successfully.");
                }
            }
            else
            {
                _logger.LogInformation("PaymentTransfers already exist.");
            }
        }

        private async Task SeedBranchesAsync()
        {
            _logger.LogInformation("Starting branch seeding...");

            if (!await _context.Branches.AnyAsync())
            {
                var branches = new List<Branch>
                {
                    new Branch { BranchName = "Main Office", Location = "Addis Ababa", ContactNumber = "+251111234567", Email = "main.office@example.com" },
                    new Branch { BranchName = "Branch A", Location = "Hawassa", ContactNumber = "+251911234567", Email = "branch.a@example.com" },
                    new Branch { BranchName = "Branch B", Location = "Adama", ContactNumber = "+251922345678", Email = "branch.b@example.com" },
                    new Branch { BranchName = "Branch C", Location = "Bahir Dar", ContactNumber = "+251933456789", Email = "branch.c@example.com" }
                };

                foreach (var branch in branches)
                {
                    branch.Id = Guid.NewGuid();
                    branch.CreatedAt = DateTime.UtcNow;
                    branch.UpdatedAt = DateTime.UtcNow;
                }

                await _context.Branches.AddRangeAsync(branches);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Branches seeded successfully.");
            }
            else
            {
                _logger.LogInformation("Branches already exist.");
            }
        }
    }
}

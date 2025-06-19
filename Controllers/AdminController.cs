using Microsoft.AspNetCore.Mvc;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PurchaseOrderManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace PurchaseOrderManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DbSeeder _dbSeeder;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            DbSeeder dbSeeder,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _dbSeeder = dbSeeder;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Get current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user roles
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            // Log roles for debugging
            _logger.LogInformation($"Current user: {currentUser.Email}, Roles: {string.Join(", ", userRoles)}");

            // Check if user has Admin role
            if (!userRoles.Contains("Admin"))
            {
                // If user doesn't have Admin role, add it
                var result = await _userManager.AddToRoleAsync(currentUser, "Admin");
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Added Admin role to user {currentUser.Email}");
                }
                else
                {
                    _logger.LogError($"Failed to add Admin role to user {currentUser.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Fetch all users with their roles
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<(ApplicationUser User, IList<string> Roles)>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add((user, roles));
            }
            ViewBag.UsersWithRoles = usersWithRoles;

            // Fetch all suppliers with their associated user data
            var suppliers = await _context.Suppliers.Include(s => s.User).ToListAsync();
            ViewBag.Suppliers = suppliers;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.Roles = new[] { "SupplyLogistics", "Budget", "GeneralManager", "ProcurementOfficer" };
            ViewBag.Branches = await _context.Branches.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new[] { "SupplyLogistics", "Budget", "GeneralManager", "ProcurementOfficer" };
                ViewBag.Branches = await _context.Branches.ToListAsync();
                return View(model);
            }

            // Generate username based on name and role
            var username = $"{model.FirstName.ToLower()}.{model.LastName.ToLower()}.{model.Role.ToLower()}";

            // Check if username already exists
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "A user with this name and role already exists.");
                ViewBag.Roles = new[] { "SupplyLogistics", "Budget", "GeneralManager", "ProcurementOfficer" };
                return View(model);
            }

            // Create the user
            var user = new ApplicationUser
            {
                UserName = username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                AccountStatus = AccountStatus.Active
            };

            // Default password: FirstName@123
            var defaultPassword = $"{model.FirstName}@123";

            // Configure password validation
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var passwordResult = await passwordValidator.ValidateAsync(_userManager, user, defaultPassword);

            if (!passwordResult.Succeeded)
            {
                // If the default password doesn't meet requirements, use a more complex one
                defaultPassword = $"{model.FirstName}@123A";
            }

            var result = await _userManager.CreateAsync(user, defaultPassword);

            if (result.Succeeded)
            {
                // Assign the selected role
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["SuccessMessage"] = $"User created successfully. Default password is: {defaultPassword}";
                return RedirectToAction(nameof(ManageUsers));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Roles = new[] { "SupplyLogistics", "Budget", "GeneralManager", "ProcurementOfficer" };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSupplier(string id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            supplier.Status = SupplierStatus.Active;
            await _context.SaveChangesAsync();

            // Add user to Supplier role
            var user = await _userManager.FindByIdAsync(supplier.UserId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Supplier");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectSupplier(string id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            // Remove the associated user account
            var user = await _userManager.FindByIdAsync(supplier.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspendSupplier(string id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            supplier.Status = SupplierStatus.Suspended;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageSuppliers));
        }

        public async Task<IActionResult> ManageSuppliers()
        {
            var suppliers = await _context.Suppliers.Include(s => s.User).ToListAsync();
            return View(suppliers);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<(ApplicationUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add((user, roles));
            }

            return View(usersWithRoles);
        }

        [HttpGet]
        public async Task<IActionResult> ViewUser(string id)
        {
            var user = await _userManager.Users.Include(u => u.Branch).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == user.Id);

            ViewBag.User = user;
            ViewBag.UserRoles = userRoles;
            ViewBag.Supplier = supplier;


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = new[] { "SupplyLogistics", "Budget", "GeneralManager", "ProcurementOfficer", "Supplier" };

            ViewBag.User = user;
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = allRoles;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(string id, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove existing roles.");
                ViewBag.User = user;
                ViewBag.UserRoles = userRoles;
                ViewBag.AllRoles = new[] { "SupplyLogistics", "Budget", "GeneralManager", "ProcurementOfficer", "Supplier" };
                return View();
            }

            if (!string.IsNullOrEmpty(selectedRole))
            {
                result = await _userManager.AddToRoleAsync(user, selectedRole);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add new role.");
                    ViewBag.User = user;
                    ViewBag.UserRoles = userRoles;
                    ViewBag.AllRoles = new[] { "SupplyLogistics", "Budget", "GeneralManager", "ProcurementOfficer", "Supplier" };
                    return View();
                }
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.Users.Include(u => u.Branch).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var branches = await _context.Branches.ToListAsync();
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BranchId = user.BranchId,
                AccountStatus = user.AccountStatus,
                PasswordResetRequired = user.PasswordResetRequired
            };

            ViewBag.Branches = branches;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _context.Branches.ToListAsync();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email; // Assuming email is the username
            user.PhoneNumber = model.PhoneNumber;
            user.BranchId = model.BranchId;
            user.AccountStatus = model.AccountStatus;
            user.PasswordResetRequired = model.PasswordResetRequired;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewBag.Branches = await _context.Branches.ToListAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete user.");
                return View("ManageUsers", await _userManager.Users.ToListAsync());
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpGet]
        public async Task<IActionResult> ViewSupplier(string id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedDatabase()
        {
            try
            {
                await _dbSeeder.SeedAsync();
                TempData["SuccessMessage"] = "Database seeded successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error seeding database: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // Branch Management

        [HttpGet]
        public async Task<IActionResult> Branches()
        {
            var branches = await _context.Branches.ToListAsync();
            return View(branches);
        }

        [HttpGet]
        public IActionResult CreateBranch()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBranch(Branch branch)
        {
            if (ModelState.IsValid)
            {
                branch.Id = Guid.NewGuid();
                _context.Add(branch);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Branch created successfully!";
                return RedirectToAction(nameof(Branches));
            }
            return View(branch);
        }

        [HttpGet]
        public async Task<IActionResult> EditBranch(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBranch(Guid id, [Bind("Id,BranchName,Location,ContactNumber,Email")] Branch branch)
        {
            if (id != branch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Branch updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Branches.Any(e => e.Id == branch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Branches));
            }
            return View(branch);
        }

        [HttpPost, ActionName("DeleteBranch")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranchConfirmed(Guid id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Branch deleted successfully!";
            }
            return RedirectToAction(nameof(Branches));
        }
    }
}

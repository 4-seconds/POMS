using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using PurchaseOrderManagementSystem.Data;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Models.ViewModels;
using PurchaseOrderManagementSystem.Services;
using System.Security.Claims;

namespace PurchaseOrderManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context; // Added
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtService jwtService,
            IWebHostEnvironment hostEnvironment,
            ApplicationDbContext context,
            ILogger<AccountController> logger) // Added logger
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _hostEnvironment = hostEnvironment;
            _context = context; // Initialize context
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            // Try to find user by email or username
            var user = await _userManager.FindByEmailAsync(model.Email) ??
                      await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "User account locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Login not allowed.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                return View(model);
            }

            // Get user roles and log them
            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation($"User {user.Email} logged in with roles: {string.Join(", ", roles)}");

            // Check if this is the first login and password needs to be changed
            if (user.AccountStatus == AccountStatus.Pending)
            {
                // Store the user ID in TempData to use in the password change page
                TempData["UserId"] = user.Id;
                return RedirectToAction("ChangePassword", "Account");
            }

            // Generate JWT
            var token = await _jwtService.GenerateTokenAsync(user);

            // Set JWT in a cookie
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(24)
            });

            // Redirect based on role
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

            // If no specific role is found, redirect to home
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = TempData["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }
            var model = new ChangePasswordViewModel { UserId = userId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.UserId))
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Generate a password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password using the token
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                // Update account status to Active
                user.AccountStatus = AccountStatus.Active;
                await _userManager.UpdateAsync(user);

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Generate JWT
                var jwtToken = await _jwtService.GenerateTokenAsync(user);

                // Set JWT in a cookie
                Response.Cookies.Append("jwt_token", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(24)
                });

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Redirect based on role
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

                // If no specific role is found, redirect to home
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SupplierRegister(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await PopulateBanksInViewBag();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplierRegister(SupplierRegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.ContactPersonName,
                    LastName = model.BusinessName,
                    AccountStatus = AccountStatus.Active
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign "Supplier" role
                    await _userManager.AddToRoleAsync(user, "Supplier");

                    // Handle file uploads
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string? tradeLicenseFileName = null;
                    if (model.TradeLicenseFile != null)
                    {
                        tradeLicenseFileName = Guid.NewGuid().ToString() + "_" + model.TradeLicenseFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, tradeLicenseFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.TradeLicenseFile.CopyToAsync(fileStream);
                        }
                    }

                    string? tinNumberFileName = null;
                    if (model.TinNumberFile != null)
                    {
                        tinNumberFileName = Guid.NewGuid().ToString() + "_" + model.TinNumberFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, tinNumberFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.TinNumberFile.CopyToAsync(fileStream);
                        }
                    }

                    // Create Supplier profile
                    var supplier = new Supplier
                    {
                        Id = model.TinNumber,
                        UserId = user.Id,
                        BusinessName = model.BusinessName,
                        ContactPerson = model.ContactPersonName,
                        ContactEmail = model.Email,
                        TinNumber = model.TinNumber,
                        Street = model.Street,
                        City = model.City,
                        State = model.State,
                        Country = model.Country,
                        Address = $"{model.Street}, {model.City}, {model.State}, {model.Country}", // Concatenate address fields
                        PhoneNumber = model.BusinessPhoneNumber,
                        Status = SupplierStatus.Pending, // Set initial status as Pending
                        TradeLicenseFilePath = tradeLicenseFileName,
                        TinNumberFilePath = tinNumberFileName,
                        PaymentMethod1 = model.PaymentMethod1,
                        PaymentMethod1BankId = model.PaymentMethod1BankId,
                        PaymentMethod2 = model.PaymentMethod2,
                        PaymentMethod2BankId = model.PaymentMethod2BankId
                    };

                    _context.Suppliers.Add(supplier);
                    await _context.SaveChangesAsync();

                    // Sign in the user

                    var token = await _jwtService.GenerateTokenAsync(user);

                    // Set JWT in a cookie
                    Response.Cookies.Append("jwt_token", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddHours(24)
                    });

                    // Redirect to pending verification page
                    return RedirectToAction("PendingVerification");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await PopulateBanksInViewBag();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Supplier")]
        public IActionResult PendingVerification()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supplier = _context.Suppliers.FirstOrDefault(s => s.UserId == currentUserId);

            if (supplier == null)
            {
                return NotFound();
            }

            // If supplier is already active, redirect to supplier dashboard
            if (supplier.Status == SupplierStatus.Active)
            {
                return RedirectToAction("Index", "Supplier");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt_token"); // Remove the JWT cookie
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private Task PopulateBanksInViewBag()
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
            dynamic? banksData = JsonConvert.DeserializeObject(banksJson);
            ViewBag.Banks = banksData;
            return Task.CompletedTask;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}


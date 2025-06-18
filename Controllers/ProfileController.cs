using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PurchaseOrderManagementSystem.Models;
using PurchaseOrderManagementSystem.Models.ViewModels;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PurchaseOrderManagementSystem.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PurchaseOrderManagementSystem.Services.IEmailSender _emailSender;

        public ProfileController(UserManager<ApplicationUser> userManager, PurchaseOrderManagementSystem.Services.IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> UpdateName()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new UpdateNameViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateName(UpdateNameViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["StatusMessage"] = "Your name has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateEmail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new UpdateEmailViewModel
            {
                Email = user.Email,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmail(UpdateEmailViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // if (!ModelState.IsValid)
            // {
            //     Console.WriteLine("ModelState is not valid");
            //     return View(model);
            // }

            if (model.NewEmail != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.NewEmail);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                var callbackUrl = Url.Action(
                    "ConfirmEmailChange",
                    "Profile",
                    new { userId = user.Id, email = model.NewEmail, code = System.Web.HttpUtility.UrlEncode(code) },
                    protocol: HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(
                    model.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

                TempData["StatusMessage"] = "Verification email sent. Please check your email to confirm the change.";
            }
            else
            {
                TempData["StatusMessage"] = "Your email address is already up to date.";
            }

            return RedirectToAction(nameof(UpdateEmail));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToAction(nameof(Index), "Home"); // Redirect to home or an error page
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (result.Succeeded)
            {
                await _userManager.SetUserNameAsync(user, email); // Update username to new email
                TempData["StatusMessage"] = "Thank you for confirming your email change.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["StatusMessage"] = "Error confirming your email change.";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpGet]
        public async Task<IActionResult> UpdatePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new UpdatePasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["StatusMessage"] = "Your password has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePhoneNumber()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new UpdatePhoneNumberViewModel
            {
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhoneNumber(UpdatePhoneNumberViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["StatusMessage"] = "Your phone number has been updated";
            return RedirectToAction(nameof(Index));
        }
    }
}



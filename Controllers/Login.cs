using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurchaseOrderManagementSystem.Models;

namespace PurchaseOrderManagementSystem.Controllers
{
    public class Login : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public Login(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(string username, string password)
        {
            // Find the user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }


            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }


            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ViewBag.ErrorMessage = "Access denied. Only Admins can log in.";
                await _signInManager.SignOutAsync();
                return View();
            }


            return RedirectToAction("AdminIndex", "Admin");
        }
    }
}

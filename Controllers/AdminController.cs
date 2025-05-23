using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurchaseOrderManagementSystem.Models;
using System.Data;
using System.Threading.Tasks;

namespace PurchaseOrderManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult AdminIndex()
        {
            return View();
        }

        public IActionResult UserManagement()
        {
            return View();
        }


        public async Task<IActionResult> UserLists()
        {
            try
            {
                var users = _userManager.Users.ToList();
                return View(users ?? new List<ApplicationUser>());
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while retrieving the user list.";
                return View(new List<ApplicationUser>());
            }
        }

        public async Task<IActionResult> UserRoleLists()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);

        }

        public async Task<IActionResult> UserRoleMapping()
        {
            var users = _userManager.Users.ToList();

            // Fetch all roles
            var roles = _roleManager.Roles.ToList();

            // Create a dictionary to map user IDs to their roles
            var userRoles = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = rolesForUser.ToList();
            }

            // Pass the model to the view
            var model = new Tuple<List<ApplicationUser>, List<IdentityRole>, Dictionary<string, List<string>>>(users, roles, userRoles);
            return View(model);
        }
        public IActionResult RolePopup()
        {
            return PartialView("RolePopup");
        }
        [HttpPost]
        public async Task<IActionResult> RolePopup(Role model)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.RoleExistsAsync(model.Name))
                {
                    TempData["Error"] = "Role already exists.";
                    return RedirectToAction("UserRoleLists");
                }

                var role = new IdentityRole { Name = model.Name };
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Role added successfully.";
                }
                else
                {
                    TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                TempData["Error"] = "Invalid input. Please check the form.";
            }

            return RedirectToAction("UserRoleLists");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateRole(string roleId)
        {
            // Fetch the role by ID
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                TempData["Error"] = "Role not found.";
                return RedirectToAction("UserRoleLists");
            }

            // Return the partial view for the modal
            return PartialView("UpdateRolePopup", new Role { Id = role.Id, Name = role.Name });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(Role model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing role by ID
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    TempData["Error"] = "Role not found.";
                    return RedirectToAction("UserRoleLists");
                }


                var existingRole = await _roleManager.FindByNameAsync(model.Name);
                if (existingRole != null && existingRole.Id != model.Id)
                {
                    TempData["Error"] = "Role already exists.";
                    return RedirectToAction("UserRoleLists");
                }


                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Role updated successfully.";
                }
                else
                {
                    TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                TempData["Error"] = "Invalid input. Please check the form.";
            }

            return RedirectToAction("UserRoleLists");
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            var model = new RegisterUserViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();


                TempData["Error"] = string.Join("<br>", errors);


                return View(model);
            }


            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            var existingUserByUsername = await _userManager.FindByNameAsync(model.UserName);

            if (existingUserByEmail != null)
            {
                TempData["Error"] = "A user with this email already exists.";
                return View(model);
            }

            if (existingUserByUsername != null)
            {
                TempData["Error"] = "A user with this username already exists.";
                return View(model);
            }

            // Validate Password and ConfirmPassword
            if (model.Password != model.ConfirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return View(model);
            }

            try
            {
                // Create the user
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true // Auto-confirm email for admin-created users
                };

                // Save the user with the password
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "User created successfully!";
                    return RedirectToAction("UserLists");
                }
                else
                {
                    TempData["Error"] = string.Join("<br>", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the user.";
            }


            return View(model);
        }
    }
}

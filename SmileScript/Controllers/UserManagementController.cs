using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmileScript.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileScript.Controllers
{
    [Authorize(Roles = "Admin")]
    // FIX 1: Using a primary constructor for more concise code (resolves IDE0290).
    public class UserManagementController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "No Email",
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }
            return Json(new { data = userViewModels });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserForm(string? id = null) // THE FIX IS HERE: string changed to string?
        {
            var model = new UserViewModel();

            if (string.IsNullOrEmpty(id))
            {
                // For 'Create', the model is already new and empty.
            }
            else
            {
                // For 'Edit', we find the user and populate the model's properties.
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                model.Id = user.Id;
                model.Email = user.Email ?? string.Empty;
                model.Role = userRoles.FirstOrDefault() ?? string.Empty;
            }

            model.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

            return PartialView("_UserFormModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveUser(UserViewModel model)
        {
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                bool isCreate = string.IsNullOrEmpty(model.Id);

                if (isCreate)
                {
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        return Json(new { success = false, message = "Password is required for new users." });
                    }
                    var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                    var result = await _userManager.CreateAsync(user, model.Password!);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                        return Json(new { success = true, message = "User created successfully!" });
                    }
                    var errorMessages = result.Errors.Select(e => e.Description).ToList();
                    return Json(new { success = false, message = string.Join(" ", errorMessages) });
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null)
                    {
                        return Json(new { success = false, message = "User not found." });
                    }
                    var oldRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, oldRoles);
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return Json(new { success = true, message = "User updated successfully!" });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(" ", errors) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return Json(new { success = true, message = "User deleted successfully!" });
            }
            return Json(new { success = false, message = "Error: User not found." });
        }
    }
}
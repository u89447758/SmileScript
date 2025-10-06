using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmileScript.Controllers
{
    [Authorize] // Ensures only logged-in users can access this controller
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Redirect user to the appropriate dashboard based on their role
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (User.IsInRole("Author"))
            {
                return RedirectToAction("AuthorDashboard");
            }

            // Optional: Redirect regular users somewhere else, like the home page
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")] // Only users in the "Admin" role can access this
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Author")] // Only users in the "Author" role can access this
        public IActionResult AuthorDashboard()
        {
            return View();
        }
    }
}
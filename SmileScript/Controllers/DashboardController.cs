using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileScript.Data;
using SmileScript.Enums;
using SmileScript.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmileScript.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (User.IsInRole("Author"))
            {
                return RedirectToAction("AuthorDashboard");
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                UserCount = await _userManager.Users.CountAsync(),
                PostCount = await _context.BlogPosts.CountAsync(),
                CategoriesCount = await _context.Categories.CountAsync(),
                PostsPendingReview = await _context.BlogPosts
                                        .Include(p => p.Author)
                                        .Where(p => p.Status == PostStatus.PendingReview)
                                        .OrderBy(p => p.CreatedDate)
                                        .ToListAsync()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Author")]
        public async Task<IActionResult> AuthorDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = new AuthorDashboardViewModel
            {
                PublishedPostsCount = await _context.BlogPosts.CountAsync(p => p.AuthorId == userId && p.Status == PostStatus.Published),
                PendingPostsCount = await _context.BlogPosts.CountAsync(p => p.AuthorId == userId && p.Status == PostStatus.PendingReview),
                RejectedPostsCount = await _context.BlogPosts.CountAsync(p => p.AuthorId == userId && p.Status == PostStatus.Rejected)
            };
            return View(viewModel);
        }
    }
}
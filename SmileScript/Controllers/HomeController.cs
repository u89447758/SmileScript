using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileScript.Data;
using SmileScript.Models;

namespace SmileScript.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var publishedPosts = await _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.Status == Enums.PostStatus.Published)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return View(publishedPosts);
        }


        // *** NEW ACTION ADDED HERE ***
        // GET: /Home/Category/your-category-slug
        [HttpGet("Home/Category/{slug}")]
        public async Task<IActionResult> Category(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            // Find the category that matches the slug
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
            {
                // If no category is found, return a 404 Not Found page
                return NotFound();
            }

            // Fetch all published posts belonging to this category
            var publishedPosts = await _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == category.Id && p.Status == Enums.PostStatus.Published)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            // Store the category name in ViewData to use as a title on the page
            ViewData["CategoryTitle"] = category.Name;

            // Pass the list of posts to the new "Category" view
            return View(publishedPosts);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
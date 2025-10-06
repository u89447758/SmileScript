using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileScript.Data;
using SmileScript.ViewModels; // Add this using directive for the ViewModel

namespace SmileScript.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Blog/my-first-post-slug
        [HttpGet("Blog/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == Enums.PostStatus.Published);

            if (blogPost == null)
            {
                return NotFound();
            }

            // *** NEW LOGIC STARTS HERE ***

            // Fetch the comments for this blog post.
            // We also include the Author of each comment.
            var comments = await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.BlogPostId == blogPost.Id)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            // Create an instance of our new ViewModel.
            var viewModel = new BlogPostDetailViewModel
            {
                BlogPost = blogPost,
                Comments = comments
            };

            // Pass the complete ViewModel to the view.
            return View(viewModel);
        }
    }
}
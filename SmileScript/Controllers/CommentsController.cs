using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmileScript.Data;
using SmileScript.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmileScript.Controllers
{
    [Authorize] // Ensures only logged-in users can access methods in this controller.
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int blogPostId, string content)
        {
            // Basic validation: check if the comment content is empty.
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Comment cannot be empty." });
            }

            // Get the currently logged-in user's ID.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create a new Comment object and populate its properties.
            var comment = new Comment
            {
                BlogPostId = blogPostId,
                Content = content,
                AuthorId = userId!, // The '!' operator tells the compiler we know userId won't be null here.
                CreatedDate = DateTime.UtcNow
            };

            // Add the new comment to the database and save changes.
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Return a success response. The JavaScript will use this to show a toast.
            return Json(new { success = true, message = "Comment posted successfully!" });
        }
    }
}
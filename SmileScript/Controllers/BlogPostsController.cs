using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmileScript.Data;
using SmileScript.Enums;
using SmileScript.Models;
using SmileScript.ViewModels;

namespace SmileScript.Controllers
{
    [Authorize(Roles = "Admin,Author")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogPostsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // The main Index view action is now just a shell.
        // It returns the container page, and all data will be loaded into it via AJAX.
        public IActionResult Index()
        {
            return View();
        }

        // ACTION 1 (NEW): Get all blog posts as JSON data for DataTables.
        [HttpGet]
        public async Task<JsonResult> GetBlogPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<BlogPost> blogPostsQuery = _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category);

            if (User.IsInRole("Author"))
            {
                blogPostsQuery = blogPostsQuery.Where(p => p.AuthorId == userId);
            }

            var blogPostDtos = await blogPostsQuery
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new BlogPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    // *** THE FIX, PART 3: Populate the new properties in the controller action ***
                    Content = p.Content,
                    HeaderImageUrl = p.HeaderImageUrl,
                    AuthorEmail = p.Author.Email,
                    CategoryName = p.Category.Name,
                    Status = (int)p.Status,
                    CreatedDate = p.CreatedDate
                })
                .ToListAsync();

            return Json(new { data = blogPostDtos });
        }


        // ACTION 2 (NEW): Get the form for creating or editing a post.
        // Returns a PartialViewResult containing just the HTML for the form.
        [HttpGet]
        public async Task<IActionResult> GetPostForm(int? id = null)
        {
            BlogPostViewModel model;

            if (id == null)
            {
                // This is a CREATE request, so create a new, empty view model.
                model = new BlogPostViewModel();
            }
            else
            {
                // This is an EDIT request. Find the existing post.
                var blogPost = await _context.BlogPosts.FindAsync(id);
                if (blogPost == null) return NotFound(new { message = "Blog post not found." });

                // Security Check: Authors can only edit their own posts.
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Author") && blogPost.AuthorId != currentUserId)
                {
                    return Forbid();
                }

                model = new BlogPostViewModel { BlogPost = blogPost };
            }

            // Populate the category dropdown list for both Create and Edit forms.
            model.CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return PartialView("_PostFormModalPartial", model);
        }

        // ACTION 3 (NEW): Save a post (handles both Create and Edit).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SavePost([FromForm] BlogPostViewModel model)
        {
            // IMPORTANT: Remove navigation properties from validation.
            ModelState.Remove("BlogPost.Author");
            ModelState.Remove("BlogPost.Category");

            if (ModelState.IsValid)
            {
                bool isCreate = model.BlogPost.Id == 0;

                if (isCreate)
                {
                    // --- CREATE LOGIC ---
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    model.BlogPost.AuthorId = userId!;
                    model.BlogPost.CreatedDate = DateTime.UtcNow;
                    model.BlogPost.Status = User.IsInRole("Author") ? PostStatus.PendingReview : PostStatus.Published;
                }
                else
                {
                    // --- EDIT LOGIC ---
                    var blogPostFromDb = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.BlogPost.Id);
                    if (blogPostFromDb == null) return Json(new { success = false, message = "Post not found." });

                    // Security check: Authors can only edit their own posts.
                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (User.IsInRole("Author") && blogPostFromDb.AuthorId != currentUserId)
                    {
                        return Json(new { success = false, message = "You are not authorized to edit this post." });
                    }

                    // Preserve original creation date and author.
                    model.BlogPost.CreatedDate = blogPostFromDb.CreatedDate;
                    model.BlogPost.AuthorId = blogPostFromDb.AuthorId;
                    model.BlogPost.UpdatedDate = DateTime.UtcNow;

                    // If user is an author, set status back to pending on edit.
                    if (User.IsInRole("Author")) model.BlogPost.Status = PostStatus.PendingReview;
                }

                // --- COMMON LOGIC (CREATE & EDIT) ---
                if (model.HeaderImage != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/headers");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.HeaderImage.FileName.Replace(" ", "-");
                    string filePath = Path.Combine(uploadsDir, uniqueFileName);
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.HeaderImage.CopyToAsync(fileStream);
                    }
                    model.BlogPost.HeaderImageUrl = "/images/headers/" + uniqueFileName;
                }
                else if (!isCreate)
                {
                    // If no new image is uploaded during an edit, keep the old one.
                    var blogPostFromDb = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.BlogPost.Id);
                    model.BlogPost.HeaderImageUrl = blogPostFromDb?.HeaderImageUrl;
                }

                model.BlogPost.Slug = model.BlogPost.Title.ToLower().Replace(" ", "-");

                if (isCreate) _context.Add(model.BlogPost);
                else _context.Update(model.BlogPost);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Blog post {(isCreate ? "created" : "updated")} successfully!" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(" ", errors) });
        }

        // ACTION 4 (MODIFIED): Deletes a post.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete([FromForm] int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null) return Json(new { success = false, message = "Post not found." });

            // Security Check: Authors can only delete their own posts.
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Author") && blogPost.AuthorId != currentUserId)
            {
                return Json(new { success = false, message = "You are not authorized to delete this post." });
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Blog post deleted successfully!" });
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        // THE FIX, PART 1: The 'FromForm' attribute name now matches what TOAST UI Editor sends by default.
        public async Task<IActionResult> UploadImage([FromForm(Name = "image")] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                // Return a Bad Request status code if no file is received.
                return BadRequest(new { error = "No file received or file is empty." });
            }
            try
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/posts");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName).Replace(" ", "-");
                string filePath = Path.Combine(uploadsDir, uniqueFileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // THE FIX, PART 2: Return JSON in the simple { "url": "..." } format that TOAST UI Editor expects.
                // We also need to return the full URL, including the request scheme and host.
                var imageUrl = $"{Request.Scheme}://{Request.Host}/images/posts/{uniqueFileName}";

                return Json(new { url = imageUrl });
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error status code on failure.
                return StatusCode(500, new { error = "An error occurred: " + ex.Message });
            }
        }

        // The Approve/Reject actions are fine as they are called from the dashboard, not this page.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null) return NotFound();
            blogPost.Status = PostStatus.Published;
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Blog post has been approved and published!";
            return RedirectToAction("AdminDashboard", "Dashboard");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null) return NotFound();
            blogPost.Status = PostStatus.Rejected;
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Blog post has been rejected.";
            return RedirectToAction("AdminDashboard", "Dashboard");
        }
    }
}
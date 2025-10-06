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

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IQueryable<BlogPost> blogPosts = _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category);

            if (User.IsInRole("Author"))
            {
                blogPosts = blogPosts.Where(p => p.AuthorId == userId);
            }

            return View(await blogPosts.OrderByDescending(p => p.CreatedDate).ToListAsync());
        }

        // GET: BlogPosts/Create
        public async Task<IActionResult> Create()
        {
            var model = new BlogPostViewModel
            {
                CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name")
            };
            return View(model);
        }

        // POST: BlogPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPostViewModel model)
        {
            // ******************************************************
            // THE DEFINITIVE FIX IS HERE
            // We manually remove the validation errors for the navigation properties,
            // because we only need their IDs from the form.
            ModelState.Remove("BlogPost.Author");
            ModelState.Remove("BlogPost.Category");
            // ******************************************************

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.BlogPost.AuthorId = userId!;
                model.BlogPost.CreatedDate = DateTime.UtcNow;

                if (model.HeaderImage != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/headers");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.HeaderImage.FileName;
                    string filePath = Path.Combine(uploadsDir, uniqueFileName);
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.HeaderImage.CopyToAsync(fileStream);
                    }
                    model.BlogPost.HeaderImageUrl = "/images/headers/" + uniqueFileName;
                }

                model.BlogPost.Slug = model.BlogPost.Title.ToLower().Replace(" ", "-");

                if (User.IsInRole("Author"))
                {
                    model.BlogPost.Status = PostStatus.PendingReview;
                    TempData["ToastMessage"] = "Blog post submitted for review successfully!";
                }
                else
                {
                    model.BlogPost.Status = PostStatus.Published;
                    TempData["ToastMessage"] = "Blog post created and published successfully!";
                }

                _context.Add(model.BlogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.BlogPost.CategoryId);
            return View(model);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Author") && blogPost.AuthorId != currentUserId)
            {
                return Forbid();
            }

            var model = new BlogPostViewModel
            {
                BlogPost = blogPost,
                CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", blogPost.CategoryId)
            };
            return View(model);
        }

        // POST: BlogPosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPostViewModel model)
        {
            if (id != model.BlogPost.Id) return NotFound();

            var blogPostFromDb = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (blogPostFromDb == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Author") && blogPostFromDb.AuthorId != currentUserId)
            {
                return Forbid();
            }

            // Apply the same fix for the Edit action
            ModelState.Remove("BlogPost.Author");
            ModelState.Remove("BlogPost.Category");

            if (ModelState.IsValid)
            {
                try
                {
                    var blogPostToUpdate = model.BlogPost;
                    blogPostToUpdate.UpdatedDate = DateTime.UtcNow;
                    blogPostToUpdate.AuthorId = blogPostFromDb.AuthorId;
                    blogPostToUpdate.CreatedDate = blogPostFromDb.CreatedDate;

                    if (model.HeaderImage != null)
                    {
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/headers");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.HeaderImage.FileName;
                        string filePath = Path.Combine(uploadsDir, uniqueFileName);
                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.HeaderImage.CopyToAsync(fileStream);
                        }
                        blogPostToUpdate.HeaderImageUrl = "/images/headers/" + uniqueFileName;
                    }
                    else
                    {
                        blogPostToUpdate.HeaderImageUrl = blogPostFromDb.HeaderImageUrl;
                    }

                    blogPostToUpdate.Slug = model.BlogPost.Title.ToLower().Replace(" ", "-");

                    if (User.IsInRole("Author"))
                    {
                        blogPostToUpdate.Status = PostStatus.PendingReview;
                    }

                    _context.Update(blogPostToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.BlogPosts.Any(e => e.Id == model.BlogPost.Id)) return NotFound();
                    else throw;
                }
                TempData["ToastMessage"] = "Blog post updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            model.CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.BlogPost.CategoryId);
            return View(model);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Author") && blogPost.AuthorId != currentUserId)
            {
                return Forbid();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Blog post deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> UploadImage([FromForm(Name = "editormd-image-file")] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = 0, message = "No file received or file is empty." });
            }
            try
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/posts");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsDir, uniqueFileName);
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return Json(new { success = 1, message = "Image uploaded successfully!", url = "/images/posts/" + uniqueFileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = "An error occurred: " + ex.Message });
            }
        }
    }
}
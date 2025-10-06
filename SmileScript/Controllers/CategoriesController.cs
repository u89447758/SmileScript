using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Add this
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmileScript.Data;
using SmileScript.Models;

namespace SmileScript.Controllers
{
    [Authorize(Roles = "Admin")] // Add this attribute to secure the whole controller
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Slug,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                // Simple slug generation (can be improved later)
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["ToastMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            // If we get here, something was wrong, but we handle this with AJAX on the page
            // so we will return a partial view or JSON in a later step if needed.
            // For now, redirecting is fine.
            TempData["ToastMessage"] = "Error creating category.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,Description")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Re-generate slug on edit
                    category.Slug = category.Name.ToLower().Replace(" ", "-");
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["ToastMessage"] = "Category updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ToastMessage"] = "Error updating category.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["ToastMessage"] = "Category deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileScript.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SmileScript.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        // Use dependency injection to get an instance of our database context
        public CategoriesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        // This is the main method that will be called when we use the component
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Fetch all categories from the database, ordered by name
            var categories = await _context.Categories
                                           .OrderBy(c => c.Name)
                                           .ToListAsync();

            // Pass the list of categories to the component's view
            return View(categories);
        }
    }
}
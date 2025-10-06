using SmileScript.Models;
using System.Collections.Generic;

namespace SmileScript.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int UserCount { get; set; }
        public int PostCount { get; set; }
        public int CategoriesCount { get; set; }
        public List<BlogPost> PostsPendingReview { get; set; } = new List<BlogPost>();
    }
}
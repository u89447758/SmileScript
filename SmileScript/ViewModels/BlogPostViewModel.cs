using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmileScript.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmileScript.ViewModels
{
    public class BlogPostViewModel
    {
        public BlogPost BlogPost { get; set; } = new BlogPost();

        [Display(Name = "Header Image")]
        public IFormFile? HeaderImage { get; set; }

        public IEnumerable<SelectListItem>? CategoryList { get; set; }
    }
}
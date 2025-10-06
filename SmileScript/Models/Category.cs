using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmileScript.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Fixed: Initialized to non-null value

        [StringLength(150)]
        public string? Slug { get; set; } // Fixed: Made nullable for optional property

        public string? Description { get; set; } // Fixed: Made nullable for optional property

        // Navigation Property: A Category can have many BlogPosts
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    }
}
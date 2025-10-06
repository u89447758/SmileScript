using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmileScript.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty; // Fixed: Initialized to non-null value
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties & Foreign Keys ---

        // Foreign key for BlogPost
        public int BlogPostId { get; set; }
        // Navigation property for BlogPost
        public virtual BlogPost BlogPost { get; set; } = null!; // Fixed: Used null-forgiving operator

        // Foreign key for the user who made the comment
        public string AuthorId { get; set; } = string.Empty; // Fixed: Initialized to non-null value
        // Navigation property for ApplicationUser
        public virtual IdentityUser Author { get; set; } = null!; // Fixed: Used null-forgiving operator
    }
}
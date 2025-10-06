using Microsoft.AspNetCore.Identity;
using SmileScript.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmileScript.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty; // Fixed: Initialized to non-null value

        [Required]
        public string Content { get; set; } = string.Empty; // Fixed: Initialized to non-null value

        [StringLength(300)]
        public string? Slug { get; set; } // Fixed: Made nullable for optional property

        public string? HeaderImageUrl { get; set; } // Fixed: Made nullable for optional property

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public PostStatus Status { get; set; } = PostStatus.Draft;

        // --- Navigation Properties & Foreign Keys ---

        // Foreign key for Category
        public int CategoryId { get; set; }
        // Navigation property for Category
        public virtual Category Category { get; set; } = null!; // Fixed: Used null-forgiving operator

        // Foreign key for the author (user)
        public string AuthorId { get; set; } = string.Empty; // Fixed: Initialized to non-null value
        // Navigation property for ApplicationUser (comes from ASP.NET Core Identity)
        public virtual IdentityUser Author { get; set; } = null!; // Fixed: Used null-forgiving operator

        // A BlogPost can have many Comments
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
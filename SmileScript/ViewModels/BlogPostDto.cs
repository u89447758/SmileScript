using System;

namespace SmileScript.ViewModels
{
    // This DTO now includes all the properties needed by the DataTables grid AND the edit modal.
    public class BlogPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // *** THE FIX, PART 1: Added the Content property ***
        public string Content { get; set; } = string.Empty;

        // *** THE FIX, PART 2: Added the HeaderImageUrl for the edit form's "Current Image" display ***
        public string? HeaderImageUrl { get; set; }

        public string AuthorEmail { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
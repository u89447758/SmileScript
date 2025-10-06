using SmileScript.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmileScript.ViewModels
{
    /// <summary>
    /// Represents the data needed for the blog post detail page.
    /// It holds the post itself, its comments, and the content for a new comment form.
    /// </summary>
    public class BlogPostDetailViewModel
    {
        // Property to hold the main blog post object.
        public BlogPost BlogPost { get; set; } = new BlogPost();

        // Property to hold the list of comments associated with the blog post.
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

        // This property will be bound to the <textarea> in our new comment form.
        // The [Required] attribute ensures a user cannot submit an empty comment.
        [Required(ErrorMessage = "Comment cannot be empty.")]
        public string NewCommentContent { get; set; } = string.Empty;
    }
}
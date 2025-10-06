using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmileScript.ViewModels
{
    public class UserViewModel
    {
        // FIX: Initialized to a non-null value
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        // FIX: Initialized to a non-null value
        public string Email { get; set; } = string.Empty;

        // Added for the create form
        [Required(ErrorMessage = "Password is required for new users.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        // FIX: Initialized to a non-null value
        public string Role { get; set; } = string.Empty;

        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SmileScript.Areas.Identity.Pages.Account
{
    // NOTE: The [AllowAnonymous] attribute is important for AJAX calls to work from any page.
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        // --- THIS CONSTRUCTOR IS WHY WE NEEDED THE FIX IN THE PREVIOUS STEP ---
        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        // --- The OnGetAsync method is largely unchanged, but we no longer need it for the modal flow ---
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        // *** THIS IS THE METHOD WE ARE MODIFYING SIGNIFICANTLY ***
        public async Task<JsonResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    // On success, return a JSON object with a success flag and a redirect URL.
                    return new JsonResult(new { success = true, redirectUrl = returnUrl });
                }
                if (result.RequiresTwoFactor)
                {
                    // Two-factor is an advanced case not handled by this modal, return a generic error.
                    return new JsonResult(new { success = false, message = "Two-factor authentication is required." });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return new JsonResult(new { success = false, message = "This account has been locked out." });
                }
                else
                {
                    // Invalid password or username.
                    return new JsonResult(new { success = false, message = "Invalid login attempt." });
                }
            }

            // If ModelState is invalid, collect the errors and return them as a single message.
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return new JsonResult(new { success = false, message = string.Join(" ", errors) });
        }
    }
}
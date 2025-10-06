// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SmileScript.Areas.Identity.Pages.Account
{
    // We add [AllowAnonymous] to be safe, ensuring it can be called via AJAX.
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // *** THIS IS THE METHOD WE ARE MODIFYING ***
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Determine where to redirect after logout.
            returnUrl ??= Url.Content("~/");

            // Instead of redirecting, we return a JSON response for our AJAX call.
            return new JsonResult(new
            {
                success = true,
                message = "You have been logged out successfully.",
                redirectUrl = returnUrl
            });
        }
    }
}
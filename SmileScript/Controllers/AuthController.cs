using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmileScript.Areas.Identity.Pages.Account; // Required to access the PageModels

namespace SmileScript.Controllers
{
    public class AuthController : Controller
    {
        // --- 1. We request the services our controller needs ---
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<RegisterModel> _registerLogger;
        private readonly ILogger<LoginModel> _loginLogger;
        private readonly IEmailSender _emailSender;

        // The dependency injection system will provide these services automatically.
        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<RegisterModel> registerLogger,
            ILogger<LoginModel> loginLogger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _registerLogger = registerLogger;
            _loginLogger = loginLogger;
            _emailSender = emailSender;
        }


        /// <summary>
        /// This action returns the HTML content for the login form.
        /// It's designed to be called via an AJAX GET request.
        /// </summary>
        [HttpGet]
        public IActionResult GetLoginModal()
        {
            // --- 2. We provide the required services when creating the model ---
            var loginModel = new LoginModel(_signInManager, _loginLogger);
            return PartialView("~/Areas/Identity/Pages/Account/Shared/_LoginModalPartial.cshtml", loginModel);
        }

        /// <summary>
        /// This action returns the HTML content for the register form.
        /// It's designed to be called via an AJAX GET request.
        /// </summary>
        [HttpGet]
        public IActionResult GetRegisterModal()
        {
            // --- 2. We provide the required services when creating the model ---
            var registerModel = new RegisterModel(_userManager, _userStore, _signInManager, _registerLogger, _emailSender);
            return PartialView("~/Areas/Identity/Pages/Account/Shared/_RegisterModalPartial.cshtml", registerModel);
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmileScript.Areas.Identity.Pages.Account;

namespace SmileScript.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<RegisterModel> _registerLogger;
        private readonly ILogger<LoginModel> _loginLogger;
        private readonly IEmailSender _emailSender;

        // Updated constructor with all required services
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

        [HttpGet]
        public IActionResult GetLoginModal()
        {
            var loginModel = new LoginModel(_signInManager, _loginLogger);
            return PartialView("~/Areas/Identity/Pages/Account/Shared/_LoginModalPartial.cshtml", loginModel);
        }

        [HttpGet]
        public IActionResult GetRegisterModal()
        {
            var registerModel = new RegisterModel(_userManager, _userStore, _signInManager, _registerLogger, _emailSender);
            return PartialView("~/Areas/Identity/Pages/Account/Shared/_RegisterModalPartial.cshtml", registerModel);
        }

        // *** NEW ACTION ADDED HERE ***
        [HttpGet]
        public IActionResult GetForgotPasswordModal()
        {
            // ForgotPasswordModel needs UserManager and IEmailSender, which we now have.
            var forgotPasswordModel = new ForgotPasswordModel(_userManager, _emailSender);
            return PartialView("~/Areas/Identity/Pages/Account/Shared/_ForgotPasswordModalPartial.cshtml", forgotPasswordModel);
        }
    }
}
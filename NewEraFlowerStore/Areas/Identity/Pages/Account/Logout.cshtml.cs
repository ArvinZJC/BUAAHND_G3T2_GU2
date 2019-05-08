#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        } // end constructor LogoutModel

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");

                StatusMessage = null;
            } // end if

            if (returnUrl != null)
                return LocalRedirect(returnUrl);

            return RedirectToPage("./Login");
        } // end method OnGet
    } // end class LogoutModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account
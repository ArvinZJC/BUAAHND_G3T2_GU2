﻿#region Using Directives
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
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        } // end constructor ConfirmEmailModel

        public string Username { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId = null, string code = null)
        {
            if (userId == null || code == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, code);
            
            if (!result.Succeeded)
            {
                _logger.LogError("Error! Failed to confirm email for specified user.");
                return NotFound($"Error! Failed to confirm the email for the user with ID \"{userId}\".");
            }

            _logger.LogInformation("Specified user confirmed email successfully.");

            Username = user.UserName;

            return Page();
        } // end method OnGetAsync
    } // end class ConfirmEmailModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account
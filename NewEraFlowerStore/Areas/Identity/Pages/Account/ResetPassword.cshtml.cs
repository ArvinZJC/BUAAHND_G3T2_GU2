// csharp file that contains actions of the page for resetting the password

#region Using Directives
using System.ComponentModel.DataAnnotations;
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
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="ResetPasswordModel"/> decorated with <see cref="AllowAnonymousAttribute"/> contains actions of the page for resetting the password.
    /// </summary>
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, ILogger<ResetPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        } // end constructor ResetPasswordModel

        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        /// <summary>
        /// An <see cref="InputModel"/> object decorated with <see cref="BindPropertyAttribute"/>.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter a user ID.")]
            [DataType(DataType.Text)]
            [StringLength(255)] // the length should be equal to that of the column "Id" in the Identity user table
            [Display(Name = "User ID")]
            public string UserId { get; set; }

            [Required(ErrorMessage = "Please enter a code.")]
            [DataType(DataType.Text)]
            public string Code { get; set; }

            [Required(ErrorMessage = "Please enter a password.")]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 8, ErrorMessage = "Please enter a valid password.")] // the relevant code and tooltips in the register page, the login page, the page for changing password, the page for resetting the password, and the page for deactivating the account need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 8 and at max 20 characters long;
             * at least 1 digit(0 - 9) and 1 letter;
             * no non-alphanumeric character
             */
            [RegularExpression(@"^(?![0-9]+$)(?![A-Za-z]+$)[0-9A-Za-z]{8,20}$", ErrorMessage = "Please enter a valid password.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please confirm the password.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            [Display(Name = "Confirm password")]
            public string ConfirmPassword { get; set; }
        } // end class InputModel

        public async Task<IActionResult> OnGetAsync(string userId = null, string code = null)
        {
            if (userId == null || code == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            StatusMessage = null;
            Input = new InputModel
            {
                UserId = userId,
                Code = code
            };

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            StatusMessage = null;

            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByIdAsync(Input.UserId);

            if (user == null)
                return NotFound();

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Specified user reset password successfully.");
                StatusMessage = string.Format("The password of the user named \"{0}\" has been reset.", user.UserName);
            }
            else
            {
                _logger.LogError("Error! Failed to reset password for specified user.");
                StatusMessage = string.Format("Error! Failed to reset the password for the user named \"{0}\".", user.UserName);
            } // end if...else

            return Page();
        } // end method OnPostAsync
    } // end class ResetPasswordModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account
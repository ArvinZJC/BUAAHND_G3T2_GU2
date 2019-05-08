#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Customer")]
    public class DeletePersonalDataModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHostingEnvironment hostingEnvironment,
            ILogger<DeletePersonalDataModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        } // end constructor DeletePersonalDataModel

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
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
        } // end class InputModel

        public int IncompleteOrdersCount { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            StatusMessage = string.Empty;
            IncompleteOrdersCount = await _context.Orders
                .Include(order => order.User)
                .Where(order => order.UserId == user.Id
                    && order.OrderStatusId != 9
                    && order.OrderStatusId != 11)
                .CountAsync();

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            StatusMessage = string.Empty;
            IncompleteOrdersCount = await _context.Orders
                .Include(order => order.User)
                .Where(order => order.UserId == user.Id
                    && order.OrderStatusId != 9
                    && order.OrderStatusId != 11)
                .CountAsync();

            if (IncompleteOrdersCount == 0)
            {
                if (ModelState.IsValid)
                {
                    if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                    {
                        ModelState.AddModelError("Input.Password", "Wrong password.");
                        return Page();
                    }

                    if (user.AvatarUrl != "_default.jpg")
                    {
                        var avatarFilePath = _hostingEnvironment.WebRootPath + $@"\img\avatars\" + user.AvatarUrl;

                        try
                        {
                            if (System.IO.File.Exists(avatarFilePath))
                            {
                                System.IO.File.Delete(avatarFilePath);
                                _logger.LogInformation("Avatar file deleted successfully.");
                            }
                            else
                                _logger.LogWarning("Avatar file cannot be found.");
                        }
                        catch (Exception e)
                        {
                            StatusMessage = "Error! Failed to delete your customised avatar when deactivating your account. You may try again.";

                            _logger.LogError(e, "Error! Failed to delete avatar file.");
                            return Page();
                        }
                    }

                    var result = await _userManager.DeleteAsync(user);
                    var username = await _userManager.GetUserNameAsync(user);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignOutAsync();
                        _logger.LogInformation("Specified account has been deactivated successfully.");
                        StatusMessage = string.Format("The account with the username \"{0}\" has been deactivated.", username);
                        return LocalRedirect("~/");
                    }

                    _logger.LogError("Error! Failed to deactivate account.");

                    StatusMessage = "Error! Failed to deactivate your account. You may try again.";
                }
            }
            else
                StatusMessage = "Error! You cannot deactivate your account because you have at least 1 incomplete order.";

            return Page();
        } // end method OnPostAsync
    } // end class DeletePersonalDataModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.OtherAdministrators
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor IndexModel

        public bool IsEmailConfirmed { get; set; }

        public List<ApplicationUser> OtherAdministratorList { get; set; }

        public List<object> OtherAdministratorForm { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                OtherAdministratorList = new List<ApplicationUser>();
                OtherAdministratorForm = new List<object>();

                foreach (var item in _context.Users)
                {
                    var roles = await _userManager.GetRolesAsync(item);

                    if (item.Id != user.Id && roles.Contains("Administrator"))
                    {
                        OtherAdministratorList.Add(item);
                        OtherAdministratorForm.Add(new
                        {
                            item.Id,
                            Initials = item.FirstName[0].ToString() + item.LastName[0].ToString(),
                            Username = item.UserName,
                            item.FirstName,
                            item.LastName,
                            item.Email,
                            IsEmailConfirmed = item.EmailConfirmed ? "Yes" : "No",
                            item.PhoneNumber,
                            RegistrationTime = item.RegistrationTime.ToLocalTime(),
                        });
                    }
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostDeleteAsync(string id = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                if (string.IsNullOrWhiteSpace(id))
                    return NotFound();

                var userToDelete = await _userManager.FindByIdAsync(id);

                if (userToDelete == null)
                {
                    StatusMessage = string.Format("Error! It seems that the user with ID \"{0}\" does not exist.", id);
                    return RedirectToPage("/OtherAdministrators/Index", new { area = "Admin" });
                }

                var result = await _userManager.DeleteAsync(userToDelete);
                var username = await _userManager.GetUserNameAsync(userToDelete);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Specified administrator account has been deleted successfully.");
                    StatusMessage = string.Format("The administrator account with the username \"{0}\" has been deleted.", username);
                    return RedirectToPage("/OtherAdministrators/Index", new { area = "Admin" });
                }

                _logger.LogError("Error! Failed to delete specified administrator account.");
                StatusMessage = string.Format("Error! Failed to delete the administrator account with the username \"{0}\". You may try again.", username);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Customers", new { area = "Admin" });
        } // end method OnPostDeleteAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.OtherAdministrators
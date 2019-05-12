// csharp file that contains actions of the registered customer list page

#region Using Directives
using System;
using System.Collections.Generic;
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

namespace NewEraFlowerStore.Areas.Admin.Pages
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="RegisteredCustomersModel"/> decorated with <see cref="AuthorizeAttribute"/> contains actions of the registered customer list page.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class RegisteredCustomersModel : PageModel
    {
        private readonly GenderListItem _genderListItem;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<RegisteredCustomersModel> _logger;

        public RegisteredCustomersModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment,
            ILogger<RegisteredCustomersModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;

            _genderListItem = new GenderListItem();
            GenderList = _genderListItem.GetGenderList();
        } // end constructor RegisteredCustomersModel

        private List<GenderListItem> GenderList { get; }
        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// A registered customer list.
        /// </summary>
        public List<ApplicationUser> RegisteredCustomerList { get; set; }
        /// <summary>
        /// A registered customer form.
        /// </summary>
        public List<object> RegisteredCustomerForm { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
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
                string gender;

                RegisteredCustomerList = new List<ApplicationUser>();
                RegisteredCustomerForm = new List<object>();

                foreach (var item in _context.Users)
                {
                    var roles = await _userManager.GetRolesAsync(item);

                    if (roles.Contains("Customer"))
                    {
                        if (item.GenderId != null)
                        {
                            if (_genderListItem.IsValidId(item.GenderId))
                                gender = GenderList.FirstOrDefault(genderListItem => genderListItem.ID == item.GenderId).DisplayName;
                            else
                                gender = "Error! Invalid value.";
                        }
                        else
                            gender = string.Empty;

                        RegisteredCustomerList.Add(item);
                        RegisteredCustomerForm.Add(new
                        {
                            item.Id,
                            item.AvatarUrl,
                            Username = item.UserName,
                            item.FirstName,
                            item.LastName,
                            item.Email,
                            IsEmailConfirmed = item.EmailConfirmed ? "Yes" : "No",
                            Gender = gender,
                            Dob = item.DOB,
                            item.PhoneNumber,
                            RegistrationTime = item.RegistrationTime.ToLocalTime(),
                        });
                    } // end if
                } // end foreach
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end class OnGetAsync

        public async Task<IActionResult> OnPostClearInvalidGenderValueAsync(string id = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                if (string.IsNullOrWhiteSpace(id))
                    return NotFound();

                var userToUpdate = await _userManager.FindByIdAsync(id);

                if (userToUpdate == null)
                {
                    StatusMessage = string.Format("Error! It seems that the user with ID \"{0}\" does not exist.", id);
                    return RedirectToPage("/RegisteredCustomers", new { area = "Admin" });
                } // end if

                if (!_genderListItem.IsValidId(userToUpdate.GenderId))
                {
                    userToUpdate.GenderId = null;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Gender of specified customer account has been initialised successfully.");
                        StatusMessage = string.Format("The gender of the customer account with the username \"{0}\" has been initialised.", userToUpdate.UserName);
                    }
                    else
                    {
                        _logger.LogError("Error! Failed to initialise gender of specified customer account.");
                        StatusMessage = string.Format("Error! Failed to initialise the gender of the customer account with the username \"{0}\". You may try again.", userToUpdate.UserName);
                    } // end if...else
                } // end if
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/RegisteredCustomers", new { area = "Admin" });
        } // end method OnPostClearInvalidGenderValueAsync

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
                    return RedirectToPage("/RegisteredCustomers", new { area = "Admin" });
                } // end if

                if (await _context.Orders
                        .Include(order => order.User)
                        .Where(order => order.UserId == id
                            && order.OrderStatusId != 9
                            && order.OrderStatusId != 11)
                        .CountAsync() == 0)
                {
                    if (userToDelete.AvatarUrl != "_default.jpg")
                    {
                        var avatarFilePath = _hostingEnvironment.WebRootPath + $@"\img\avatars\" + userToDelete.AvatarUrl;

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
                            _logger.LogError(e, "Error! Avatar file failed to delete.");
                            StatusMessage = string.Format("Error! Failed to delete {0}'s customised avatar when deleting the customer account. You may try again.", userToDelete.UserName);
                            return RedirectToPage("/RegisteredCustomers", new { area = "Admin" });
                        } // end try...catch
                    } // end if

                    var result = await _userManager.DeleteAsync(userToDelete);
                    var username = await _userManager.GetUserNameAsync(userToDelete);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Specified customer account has been deleted successfully.");
                        StatusMessage = string.Format("The customer account with the username \"{0}\" has been deleted.", username);
                        return RedirectToPage("/RegisteredCustomers", new { area = "Admin" });
                    } // end if

                    _logger.LogError("Error! Failed to delete specified customer account.");
                    StatusMessage = string.Format("Error! Failed to delete the customer account with the username \"{0}\". You may try again.", username);
                }
                else
                    StatusMessage = string.Format("Error! You cannot delete the customer account with the username \"{0}\" because it contains at least 1 incomplete order.", userToDelete.UserName);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/RegisteredCustomers", new { area = "Admin" });
        } // end method OnPostDeleteAsync
    } // end class RegisteredCustomersModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages
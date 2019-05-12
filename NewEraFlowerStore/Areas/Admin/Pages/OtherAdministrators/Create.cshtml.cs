// csharp file that contains actions of the page for creating an administrator account

#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.OtherAdministrators
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="DeliveryInfoModel"/> contains actions of the page for creating an administrator account.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            UserManager<ApplicationUser> userManager,
            ILogger<CreateModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        } // end constructor CreateModel

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
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
            [Required(ErrorMessage = "Please enter a username.")]
            [DataType(DataType.Text)]
            [StringLength(25, ErrorMessage = "Please enter a valid username.")] // the relevant regular expression below and tooltips in the page for creating an administrator account need updating after modifying the length
            /* 
             * the maximum length here should be equal to the relevant attribute;
             * at max 25 characters long;
             * no non-alphanumeric character
             */
            [RegularExpression(@"^[0-9A-Za-z]{0,25}$", ErrorMessage = "Please enter a valid username.")]
            [Remote(action: "VerifyUsernameNotInUseAsync", controller: "ApplicationUser")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Please enter a first name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid first name.")] // the relevant code and tooltips in the page for creating an administrator account need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 2 and at max 25 letters long, with only the 1st letter uppercase
             */
            [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid first name.")]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Please enter a last name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid last name.")] // the relevant code and tooltips in the page for creating an administrator account need updating after modifying the lengththe length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 2 and at max 25 letters long, with only the 1st letter uppercase
             */
            [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid last name.")]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Please enter an email address.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            [StringLength(50, ErrorMessage = "Please enter a valid email address.")]
            [Remote(action: "VerifyEmailNotInUseAsync", controller: "ApplicationUser")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Please enter a phone number.")]
            [Phone(ErrorMessage = "Please enter a valid phone number.")]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Please enter a password.")]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 8, ErrorMessage = "Please enter a valid password.")] // the relevant code and tooltips in the page for creating an administrator account need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 8 and at max 20 characters long;
             * at least 1 digit (0 - 9) and 1 letter;
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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if(!IsEmailConfirmed)
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                if (!ModelState.IsValid)
                    return Page();

                var newAdministrator = new ApplicationUser
                {
                    AvatarUrl = string.Empty,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    UserName = Input.Username,
                    Email = Input.Email,
                    EmailConfirmed = true,
                    PhoneNumber = Input.PhoneNumber,
                    RegistrationTime = DateTimeOffset.Now
                };
                var createResult = await _userManager.CreateAsync(newAdministrator, Input.Password);

                if (createResult.Succeeded)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(newAdministrator, "Administrator");

                    if (addToRoleResult.Succeeded)
                    {
                        _logger.LogInformation("Administrator created account with Administrator role successfully.");
                        StatusMessage = string.Format("An administrator account with the username \"{0}\" has been created.", Input.Username);
                    }
                    else
                    {
                        var deleteResult = await _userManager.DeleteAsync(newAdministrator);

                        if (deleteResult.Succeeded)
                        {
                            _logger.LogError("Error! Failed to create account with Administrator role.");
                            ModelState.AddModelError("Input.Email", "Failed to create an account with the Administrator role. You may try again.");
                        }
                        else
                        {
                            _logger.LogError("Error! Failed to create account with Administrator role, and error occurred during process of deleting wrong account.");
                            ModelState.AddModelError("Input.Email", "Failed to create an account with the Administrator role, and an error occurred during the process of deleting the wrong account. You may try again.");
                        }

                        return Page();
                    }
                }
                else
                {
                    _logger.LogError("Error! Failed to create account with given info and password.");
                    ModelState.AddModelError("Input.Email", "Failed to create an account with the given info and password. You may try again.");
                    return Page();
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/OtherAdministrators/Index", new { area = "Admin" });
        } // end method OnPostAsync
    } // end class CreateModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.OtherAdministrators
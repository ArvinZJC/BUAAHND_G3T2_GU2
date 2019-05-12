// csharp file that contains actions of the user profile page

#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="ProfileModel"/> decorated with <see cref="AuthorizeAttribute"/> contains actions of the user profile page.
    /// </summary>
    public class ProfileModel : PageModel
    {
        private readonly GenderListItem _genderListItem;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<ProfileModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;

            _genderListItem = new GenderListItem();
            GenderList = _genderListItem.GetGenderList();
        } // end constructor ProfileModel

        /// <summary>
        /// The URL of the avatar file.
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// The email address.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// Indicate whether the login user is an administrator or not.
        /// </summary>
        public bool IsAdministrator { get; set; }
        /// <summary>
        /// A gender list.
        /// </summary>
        public List<GenderListItem> GenderList { get; }
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
            [Required(ErrorMessage = "Please enter a first name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid first name.")] // the relevant code and tooltips in the user profile page need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 2 and at max 25 letters long, with only the 1st letter uppercase
             */
            [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid first name.")]
            [Display(Name = "First name*")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Please enter a last name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid last name.")] // the relevant code and tooltips in the user profile page need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 2 and at max 25 letters long, with only the 1st letter uppercase
             */
            [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid last name.")]
            [Display(Name = "Last name*")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Please enter a username.")]
            [DataType(DataType.Text)]
            [StringLength(25, ErrorMessage = "Please enter a valid username.")] // the relevant regular expression below and tooltips in the user profile page need updating after modifying the length
            /* 
             * the maximum length here should be equal to the relevant attribute;
             * at max 25 characters long;
             * no non-alphanumeric character
             */
            [RegularExpression(@"^[0-9A-Za-z]{0,25}$", ErrorMessage = "Please enter a valid username.")]
            [Remote(action: "VerifyUsernameNotInUseAsync", controller: "ApplicationUser")]
            [Display(Name = "Username*")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Please enter an email address.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            [StringLength(50, ErrorMessage = "Please enter a valid email address.")]
            [Remote(action: "VerifyEmailNotInUseAsync", controller: "ApplicationUser")]
            [Display(Name = "Email*")]
            public string Email { get; set; }

            [Display(Name = "Gender")]
            public int? GenderId { get; set; }

            [DataType(DataType.Date)]
            public DateTime? DOB { get; set; } // DOB is the abbreviation of "Date of Birth"

            [Phone(ErrorMessage = "Please enter a valid phone number.")]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        } // end class InputModel

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email,
                GenderId = user.GenderId,
                DOB = user.DOB,
                PhoneNumber = user.PhoneNumber
            };

            if (!_genderListItem.IsValidId(Input.GenderId))
                Input.GenderId = null;

            var userRoles = await _userManager.GetRolesAsync(user);
            
            AvatarUrl = user.AvatarUrl;
            Email = user.Email;
            IsEmailConfirmed = user.EmailConfirmed;
            IsAdministrator = userRoles.Contains("Administrator");

            if (!IsEmailConfirmed)
                StatusMessage = string.Format("Error! To fully experience our services, you must verify your email address \"{0}\". Please pay attention that a verification email may be considered spam.", Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostSaveAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            AvatarUrl = user.AvatarUrl;
            Email = user.Email;
            IsEmailConfirmed = user.EmailConfirmed;
            IsAdministrator = userRoles.Contains("Administrator");

            if (!ModelState.IsValid)
            {
                if(!IsEmailConfirmed)
                    StatusMessage = string.Format("Error! To fully experience our services, you must verify your email address \"{0}\". Please pay attention that a verification email may be considered spam.", Email);

                return Page();
            } // end if

            var isChanged = false;
            
            if (user.Email != Input.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);

                if (!setEmailResult.Succeeded)
                {
                    ModelState.AddModelError("Input.Email", "Failed to set the email address.");
                    return Page();
                } // end if

                isChanged = true;
            }
            else if(!IsEmailConfirmed)
            {
                StatusMessage = string.Format("Error! To fully experience our services, you must verify your email address \"{0}\". Please pay attention that a verification email may be considered spam.", Email);
                return Page();
            } // end nested if...else

            if (user.FirstName != Input.FirstName)
            {
                user.FirstName = Input.FirstName;
                isChanged = true;
            } // end if

            if (user.LastName != Input.LastName)
            {
                user.LastName = Input.LastName;
                isChanged = true;
            } // end if

            if (user.UserName != Input.Username)
            {
                user.UserName = Input.Username;
                isChanged = true;
            } // end if

            if (IsAdministrator)
            {
                if (user.GenderId != null)
                {
                    user.GenderId = null;
                    isChanged = true;
                } // end if
            }
            else if (user.GenderId != Input.GenderId && _genderListItem.IsValidId(Input.GenderId))
            {
                user.GenderId = Input.GenderId;
                isChanged = true;
            } // end nested if...else

            if (IsAdministrator)
            {
                if(user.DOB != null)
                {
                    user.DOB = null;
                    isChanged = true;
                } // end if
            }
            else if (user.DOB != Input.DOB)
            {
                user.DOB = Input.DOB;
                isChanged = true;
            } // end nested if...else

            var validPhoneNumberContent = string.IsNullOrWhiteSpace(Input.PhoneNumber) ? null : Input.PhoneNumber.Trim();

            if (user.PhoneNumber != validPhoneNumberContent)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, validPhoneNumberContent);

                if (!setPhoneResult.Succeeded)
                {
                    ModelState.AddModelError("Input.PhoneNumber", "Failed to set the phone number.");
                    return Page();
                } // end if

                isChanged = true;
            } // end if

            if (isChanged)
            {
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    _logger.LogInformation("User updated profile successfully.");

                    StatusMessage = "Your profile has been updated.";
                }
                else
                {
                    _logger.LogError("Error! Failed to update profile.");

                    StatusMessage = "Error! Failed to update your profile. You may try again.";
                } // end if...else
            } // end if

            return Page();
        } // end method OnPostSaveAsync

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email,
                GenderId = user.GenderId,
                DOB = user.DOB,
                PhoneNumber = user.PhoneNumber
            };

            if (!_genderListItem.IsValidId(Input.GenderId))
                Input.GenderId = null;

            var userRoles = await _userManager.GetRolesAsync(user);

            AvatarUrl = user.AvatarUrl;
            Email = user.Email;
            IsEmailConfirmed = user.EmailConfirmed;
            IsAdministrator = userRoles.Contains("Administrator");

            if (!IsEmailConfirmed)
                StatusMessage = string.Format("Error! To fully experience our services, you must verify your email address \"{0}\". Please pay attention that a verification email may be considered spam.", Email);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code },
                protocol: Request.Scheme);
            var privacyPolicyUrl = Url.Page(
                "/Help/PrivacyPolicy",
                pageHandler: null,
                values: null,
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirm Your Email",
                $"<p>Dear {user.UserName},</p>" +
                $"<p>We have received a request from you to verify your email address. To confirm your email, please <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here</a>. If you cannot access the link, you may copy and paste \"{HtmlEncoder.Default.Encode(callbackUrl)}\" into your browser's address bar.</p>" +
                $"<p>For info on New Era Flower Store's privacy policy, please visit \"<a href='{HtmlEncoder.Default.Encode(privacyPolicyUrl)}'>{HtmlEncoder.Default.Encode(privacyPolicyUrl)}</a>\". This email message was auto-generated. Please do not respond.</p>" +
                $"<p>New Era Flower Store</p>" +
                $"<hr />" +
                $"<p>©2019 SHIELD Technology, Inc.</p>");

            StatusMessage = "To confirm your email, a verification email has been sent. Please check your email, and pay attention that a verification email may be considered spam.";

            return Page();
        } // end method OnPostSendVerificationEmailAsync
    } // end class ProfileModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
// csharp file that contains actions of the register page

#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="RegisterModel"/> decorated with <see cref="AllowAnonymousAttribute"/> contains actions of the register page.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        } // end constructor RegisterModel

        /// <summary>
        /// The URL of the page to return to.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// An <see cref="InputModel"/> object decorated with <see cref="BindPropertyAttribute"/>.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter a first name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid first name.")] // the relevant code and tooltips in the register page and the user profile page need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 2 and at max 25 letters long, with only the 1st letter uppercase
             */
            [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid first name.")]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Please enter a last name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid last name.")] // the relevant code and tooltips in the register page and the user profile page need updating after modifying the lengththe length
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

            [Required(ErrorMessage = "Please enter a password.")]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 8, ErrorMessage = "Please enter a valid password.")] // the relevant code and tooltips in the register page need updating after modifying the length
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

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"); // if there is no specified return URL, set the home page to the return URL
        } // end method OnGet

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"); // if there is no specified return URL, set the home page to the return URL

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    AvatarUrl = "_default.jpg",
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    UserName = await CreateUsernameAsync(Input.FirstName, Input.LastName), // call the specified method to create a username with the first name, last name, and creation time
                    Email = Input.Email,
                    RegistrationTime = DateTimeOffset.Now
                };
                var createResult = await _userManager.CreateAsync(user, Input.Password);

                if (createResult.Succeeded)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "Customer");

                    if (addToRoleResult.Succeeded)
                    {
                        _logger.LogInformation("User created account with Customer role successfully.");

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
                            "Confirm Your Acount",
                            $"<p>Dear {user.UserName},</p>" +
                            $"<p>We have received a request from you to create an account. To confirm your account, please <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here</a>. If you cannot access the link, you may copy and paste \"{HtmlEncoder.Default.Encode(callbackUrl)}\" into your browser's address bar.</p>" +
                            $"<p>For info on New Era Flower Store's privacy policy, please visit \"<a href='{HtmlEncoder.Default.Encode(privacyPolicyUrl)}'>{HtmlEncoder.Default.Encode(privacyPolicyUrl)}</a>\". This email message was auto-generated. Please do not respond.</p>" +
                            $"<p>New Era Flower Store</p>" +
                            $"<hr />" +
                            $"<p>©2019 SHIELD Technology, Inc.</p>"); // send a verification email for confirming the account
                        await _signInManager.SignInAsync(user, isPersistent: false); // automatically log in after registering successfully, and the login cookie should not persist after the browser is closed
                        return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        var deleteResult = await _userManager.DeleteAsync(user);

                        if (deleteResult.Succeeded)
                        {
                            _logger.LogError("Error! Failed to create account with Customer role.");
                            ModelState.AddModelError("Input.Email", "Failed to create an account with the Customer role. You may try again.");
                        }
                        else
                        {
                            _logger.LogError("Error! Failed to create account with Customer role, and error occurred during process of deleting wrong account.");
                            ModelState.AddModelError("Input.Email", "Failed to create an account with the Customer role, and an error occurred during the process of deleting the wrong account. You may try again.");
                        } // end if...else
                    } // end if...else
                }
                else
                {
                    _logger.LogError("Error! Failed to create account with given email and password.");
                    ModelState.AddModelError("Input.Email", "Failed to create an account with the given email and password. You may try again.");
                } // end if...else
            } // end if

            return Page();
        } // end method OnPostAsync

        // create a username with the first name, last name, and creation time
        private async Task<string> CreateUsernameAsync(string firstName, string lastName)
        {
            string username;
            var codeName = firstName[0].ToString() + lastName[0].ToString();
            DateTimeOffset creationTime;
            var random = new Random();
            
            do
            {
                creationTime = DateTimeOffset.Now;
                username = codeName + creationTime.Day + creationTime.Month + creationTime.Year.ToString().Substring(2) + creationTime.Hour + creationTime.Minute + creationTime.Second + creationTime.Millisecond + random.Next(100); // at least 11 and at max 20 characters long
            } while (await _userManager.FindByNameAsync(username) != null);

            return username;
        } // end method CreateUsername
    } // end class RegisterModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account
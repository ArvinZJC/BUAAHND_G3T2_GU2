// csharp file that contains actions of the login page

#region Using Directives
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Areas.Identity.Data;
using NewEraFlowerStore.Data;
using NewEraFlowerStore.Services;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="LoginModel"/> decorated with <see cref="AllowAnonymousAttribute"/> contains actions of the login page.
    /// </summary>
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICaptchaManager _captchaManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICaptchaManager captchaManager,
            ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _captchaManager = captchaManager;
            _logger = logger;
        } // end constructor LoginModel

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
            [Required(ErrorMessage = "Please enter a username or email address.")]
            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "Please enter a valid username or email address.")]
            [Display(Name = "Username/email")]
            public string UsernameOrEmail { get; set; }

            [Required(ErrorMessage = "Please enter a password.")]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 8, ErrorMessage = "Please enter a valid password.")] // the relevant code and tooltips in the login page need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 8 and at max 20 characters long;
             * at least 1 digit (0 - 9) and 1 letter;
             * no non-alphanumeric character
             */
            [RegularExpression(@"^(?![0-9]+$)(?![A-Za-z]+$)[0-9A-Za-z]{8,20}$", ErrorMessage = "Please enter a valid password.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please enter the captcha.")]
            [DataType(DataType.Text)]
            [StringLength(4, MinimumLength = 4, ErrorMessage = "Please enter a valid captcha.")] // the relevant regular expression below and tooltips in the login page need updating after modifying the length
            /* 
             * the length here should be equal to the relevant attributes;
             * 4 characters long;
             * no non-alphanumeric character
             */
            [RegularExpression(@"^[0-9A-Za-z]{4}$", ErrorMessage = "Please enter a valid captcha.")]
            public string Captcha { get; set; }

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        } // end class InputModel

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"); // if there is no specified return URL, set the home page to the return URL
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // clear the existing external cookie to ensure a clean login process
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"); // if there is no specified return URL, set the home page to the return URL

            if (ModelState.IsValid)
            {
                Request.Cookies.TryGetValue("CaptchaInfo", out string captchaKey);

                var captchaRequest = new CaptchaRequest { Answer = Input.Captcha, CaptchaKey = captchaKey };
                var captchaResponse = await _captchaManager.VerifyAsync(captchaRequest);
                var user = Input.UsernameOrEmail.Contains('@') ? await _userManager.FindByEmailAsync(Input.UsernameOrEmail) : await _userManager.FindByNameAsync(Input.UsernameOrEmail); // there is no non-alphanumeric chracter in a username
                var maxFailedAccessAttempts = _signInManager.Options.Lockout.MaxFailedAccessAttempts;

                if (user == null)
                {
                    ModelState.AddModelError("Input.UsernameOrEmail", "The user does not exist.");

                    if (captchaResponse.Code != 0)
                        ModelState.AddModelError("Input.Captcha", captchaResponse.Message);

                    return Page();
                } // end if

                var signInResult = await _signInManager.PasswordSignInAsync(
                    user,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true);
                var triesLeft = maxFailedAccessAttempts - user.AccessFailedCount;

                if (captchaResponse.Code == 0)
                {
                    if (signInResult.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(ReturnUrl);
                    } // end if

                    if (signInResult.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        ModelState.AddModelError("Input.Email", "This account has been locked out. Please try again later.");
                        return Page();
                    } // end if

                    if (signInResult.RequiresTwoFactor)
                    {
                        _logger.LogError("Error! Failed to log in. Process of logging in contains function disabled.");
                        ModelState.AddModelError("Input.Email", "Failed to log in. Process of logging in contains function disabled. You may try again.");
                        return Page();
                    } // end if

                    ModelState.AddModelError("Input.Password", "Wrong password. " + (triesLeft > 1 ? triesLeft + " more tries left." : triesLeft + " more try left."));
                    return Page();
                } // end if

                ModelState.AddModelError("Input.Captcha", captchaResponse.Message);

                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError("Input.Email", "This account has been locked out. Please try again later.");
                    return Page();
                } // end if

                if (signInResult.Succeeded)
                    await _signInManager.SignOutAsync();
                else if (signInResult.RequiresTwoFactor)
                    ModelState.AddModelError("Input.Email", "Failed to log in. Process of logging in contains function disabled. You may try again.");
                else
                    ModelState.AddModelError("Input.Password", "Wrong password. " + (triesLeft > 1 ? triesLeft + " more tries left." : triesLeft + " more try left."));
                
                return Page();
            } // end if

            return Page();
        } // end method OnPostAsync
    } // end class LoginModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account
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

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        } // end constructor ForgotPasswordModel

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter a username or email address.")]
            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "Please enter a valid username or email address.")] // the relevant code in the page for sending a verification email to reset the password needs updateing after modifying the length
            [Display(Name = "Username/email")]
            public string UsernameOrEmail { get; set; }
        } // end class InputModel

        public void OnGet()
        {
            StatusMessage = null;
        } // end method OnGet

        public async Task<IActionResult> OnPostAsync()
        {
            StatusMessage = null;

            if (ModelState.IsValid)
            {
                var user = Input.UsernameOrEmail.Contains('@') ? await _userManager.FindByEmailAsync(Input.UsernameOrEmail) : await _userManager.FindByNameAsync(Input.UsernameOrEmail); // there is no non-alphanumeric chracter in a username

                if (user == null)
                {
                    ModelState.AddModelError("Input.UsernameOrEmail", "The user does not exist.");
                    return Page();
                }

                var privacyPolicyUrl = Url.Page(
                    "/Help/PrivacyPolicy",
                    pageHandler: null,
                    values: null,
                    protocol: Request.Scheme);

                if (!user.EmailConfirmed)
                {
                    var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var passwordResetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var emailConfirmationCallbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = emailConfirmationCode },
                        protocol: Request.Scheme);
                    var passwordResetCallbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { userId = user.Id, code = passwordResetCode },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Confirm Your Email and Reset Your Password",
                        $"<p>Dear {user.UserName},</p>" +
                        $"<p>We have known that you forgot your password. You may reset your password by accessing the specified link in this email. However, before this, you need to verify your email address.</p>" +
                        $"<p>First, to confirm your email, please <a href='{HtmlEncoder.Default.Encode(emailConfirmationCallbackUrl)}'>click here</a>. If you cannot access the link, you may copy and paste \"{HtmlEncoder.Default.Encode(emailConfirmationCallbackUrl)}\" into your browser's address bar.</p>" +
                        $"<p>Second, to reset your password, please <a href='{HtmlEncoder.Default.Encode(passwordResetCallbackUrl)}'>click here</a>. If you cannot access the link, you may copy and paste \"{HtmlEncoder.Default.Encode(passwordResetCallbackUrl)}\" into your browser's address bar.</p>" +
                        $"<p>For info on New Era Flower Store's privacy policy, please visit \"<a href='{HtmlEncoder.Default.Encode(privacyPolicyUrl)}'>{HtmlEncoder.Default.Encode(privacyPolicyUrl)}</a>\". This email message was auto-generated. Please do not respond.</p>" +
                        $"<p>New Era Flower Store</p>" +
                        $"<hr />" +
                        $"<p>©2018-{DateTimeOffset.Now.Year} SHIELD Technology, Inc.</p>");

                    StatusMessage = "To confirm your email and reset your password, a verification email has been sent. Please check your email, and pay attention that a verification email may be considered spam.";

                    return Page();
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { userId = user.Id, code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Reset Your Password",
                    $"<p>Dear {user.UserName},</p>" +
                    $"<p>We have known that you forgot your password. To reset your password, please <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here</a>. If you cannot access the link, you may copy and paste \"{HtmlEncoder.Default.Encode(callbackUrl)}\" into your browser's address bar.</p>" +
                    $"<p>For info on New Era Flower Store's privacy policy, please visit \"<a href='{HtmlEncoder.Default.Encode(privacyPolicyUrl)}'>{HtmlEncoder.Default.Encode(privacyPolicyUrl)}</a>\". This email message was auto-generated. Please do not respond.</p>" +
                    $"<p>New Era Flower Store</p>" +
                    $"<hr />" +
                    $"<p>©2018-{DateTimeOffset.Now.Year} SHIELD Technology, Inc.</p>");

                StatusMessage = "To reset your password, a verification email has been sent. Please check your email, and pay attention that a verification email may be considered spam.";
            }

            return Page();
        } // end method OnPostAsync
    } // end class ForgotPasswordModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account
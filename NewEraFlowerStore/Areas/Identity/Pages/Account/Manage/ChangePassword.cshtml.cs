// csharp file that contains actions of the page for changing the password

#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="ChangePasswordModel"/> contains actions of the page for changing the password.
    /// </summary>
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        } // end constructor ChangePasswordModel

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
            [Required(ErrorMessage = "Please enter an old password.")]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 8, ErrorMessage = "Please enter a valid old password.")] // the relevant code and tooltips in the page for changing password need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 8 and at max 20 characters long;
             * at least 1 digit (0 - 9) and 1 letter;
             * no non-alphanumeric character
             */
            [RegularExpression(@"^(?![0-9]+$)(?![A-Za-z]+$)[0-9A-Za-z]{8,20}$", ErrorMessage = "Please enter a valid old password.")]
            [Display(Name = "Old password")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Please enter a new password.")]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 8, ErrorMessage = "Please enter a valid new password.")] // the relevant code and tooltips in the page for changing password need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 8 and at max 20 characters long;
             * at least 1 digit (0 - 9) and 1 letter;
             * no non-alphanumeric character
             */
            [RegularExpression(@"^(?![0-9]+$)(?![A-Za-z]+$)[0-9A-Za-z]{8,20}$", ErrorMessage = "Please enter a valid new password.")]
            [NotSameAs("OldPassword")]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Please confirm the new password.")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            [Display(Name = "Confirm new password")]
            public string ConfirmPassword { get; set; }
        } // end class InputModel

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (!IsEmailConfirmed)
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (!IsEmailConfirmed)
            {
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
                return Page();
            } // end if

            if (!ModelState.IsValid)
                return Page();

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                ModelState.AddModelError("Input.OldPassword", "Wrong password.");
                return Page();
            } // end if

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed password successfully.");

            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        } // end method OnPostAsync
    } // end class ChangePasswordModel

    #region Custom Validation
    public class NotSameAsAttribute : ValidationAttribute, IClientModelValidator
    {
        private string OtherProperty { get; set; } // the property to compare with the current property.

        public NotSameAsAttribute(string otherProperty)
        {
            OtherProperty = otherProperty ?? throw new ArgumentNullException("otherProperty");
        } // end constructor NotSameAsAttribute

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProperty_Info = validationContext.ObjectType.GetProperty(OtherProperty);

            if (otherProperty_Info == null)
                return new ValidationResult(string.Format("Error! Unknown property \"{0}\".", OtherProperty));

            var propertyName = validationContext.ObjectType.GetProperties()
                .Where(info => info.GetCustomAttributes(false)
                    .OfType<DisplayAttribute>()
                    .Any(attribute => attribute.Name == validationContext.DisplayName))
                .Select(info => info.Name)
                .FirstOrDefault() ?? validationContext.DisplayName;

            if (validationContext.ObjectType.GetProperty(propertyName).PropertyType != otherProperty_Info.PropertyType)
                return new ValidationResult(string.Format("Error! Property types of \"{0}\" and \"{1}\" must be the same.", propertyName, OtherProperty));

            var otherProperty_Value = otherProperty_Info.GetValue(validationContext.ObjectInstance, null);

            if (Equals(value, otherProperty_Value))
                return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        } // end method IsValid

        /// <summary>
        /// Provide support for the client-side unobtrusive validation.
        /// </summary>
        /// <param name="context">the context for the client-side model validation</param>
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-notsameas", GetErrorMessage());
            MergeAttribute(context.Attributes, "data-val-notsameas-other", OtherProperty);
        } // end method AddValidation

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
                return false;

            attributes.Add(key, value);
            return true;
        } // end method MergeAttribute

        private string GetErrorMessage()
        {
            return $"Your new password is the same as the old one you entered.";
        } // end method GetErrorMessage
    } // end class NotSameAsAttribute
    #endregion Custom Validation
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
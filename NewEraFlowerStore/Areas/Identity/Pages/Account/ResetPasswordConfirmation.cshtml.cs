﻿// csharp file that contains actions of a page created by Identity but actually not used in the application

#region Using Directives
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="ResetPasswordConfirmationModel"/> decorated with <see cref="AllowAnonymousAttribute"/> contains actions of a page created by Identity but actually not used in the application.
    /// </summary>
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModel
    {
        public IActionResult OnGet()
        {
            return NotFound();
        } // end method OnGet
    } // end class ResetPasswordConfirmationModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account
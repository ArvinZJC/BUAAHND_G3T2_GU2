// csharp file that contains actions of a page created by Identity but actually not used in the application

#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="ExternalLoginsModel"/> contains actions of a page created by Identity but actually not used in the application.
    /// </summary>
    public class ExternalLoginsModel : PageModel
    {
        public IActionResult OnGet()
        {
            return NotFound();
        } // end method OnGet
    } // end class ExternalLoginsModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
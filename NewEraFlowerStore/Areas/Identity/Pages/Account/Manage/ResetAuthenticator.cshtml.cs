#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
{
    public class ResetAuthenticatorModel : PageModel
    {
        public IActionResult OnGet()
        {
            return NotFound();
        } // end method OnGet
    } // end class ResetAuthenticatorModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
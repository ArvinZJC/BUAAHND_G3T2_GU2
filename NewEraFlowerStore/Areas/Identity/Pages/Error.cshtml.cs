#region Using Directives
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages
{
    [AllowAnonymous]
    public class ErrorModel : PageModel
    {
        public IActionResult OnGet()
        {
            return NotFound();
        } // end method OnGet
    } // end class ErrorModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages
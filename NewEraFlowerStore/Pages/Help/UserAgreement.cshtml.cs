#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    public class UserAgreementModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }
    } // end class UserAgreementModel
} // end namespace NewEraFlowerStore.Pages.Help
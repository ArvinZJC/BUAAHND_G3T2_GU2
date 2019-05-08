#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    public class PrivacyPolicyModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }
    } // end class PrivacyPolicyModel
} // end namespace NewEraFlowerStore.Pages.Help
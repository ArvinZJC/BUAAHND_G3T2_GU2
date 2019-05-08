#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    public class AboutUsModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }
    } // end class AboutUsModel
} // end namespace NewEraFlowerStore.Pages.Help
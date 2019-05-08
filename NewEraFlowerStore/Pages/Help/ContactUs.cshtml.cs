#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    public class ContactUsModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }
    } // end class ContactUsModel
} // end namespace NewEraFlowerStore.Pages.Help
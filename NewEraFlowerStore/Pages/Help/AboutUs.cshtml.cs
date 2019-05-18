// csharp file that contains actions of the page for introducing the store

#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    /// <summary>
    /// Extending from the class <see cref="PageModel"/>, the class <see cref="AboutUsModel"/> contains actions of the page for introducing the store.
    /// </summary>
    public class AboutUsModel : PageModel
    {
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
    } // end class AboutUsModel
} // end namespace NewEraFlowerStore.Pages.Help
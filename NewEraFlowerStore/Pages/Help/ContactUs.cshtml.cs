// csharp file that contains actions of the page for detailing how to contact the store

#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    /// <summary>
    /// Extending from the class <see cref="PageModel"/>, the class <see cref="ContactUsModel"/> contains actions of the page for detailing how to contact the store.
    /// </summary>
    public class ContactUsModel : PageModel
    {
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
    } // end class ContactUsModel
} // end namespace NewEraFlowerStore.Pages.Help
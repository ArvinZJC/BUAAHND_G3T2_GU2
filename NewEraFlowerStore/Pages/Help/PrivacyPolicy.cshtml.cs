// csharp file that contains actions of the privacy policy page

#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    /// <summary>
    /// Extending from the class <see cref="PageModel"/>, the class <see cref="PrivacyPolicyModel"/> contains actions of the privacy policy page.
    /// </summary>
    public class PrivacyPolicyModel : PageModel
    {
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
    } // end class PrivacyPolicyModel
} // end namespace NewEraFlowerStore.Pages.Help
// csharp file that contains actions of the user agreement page

#region Using Directives
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    /// <summary>
    /// Extending from the class <see cref="PageModel"/>, the class <see cref="UserAgreementModel"/> contains actions of the user agreement page.
    /// </summary>
    public class UserAgreementModel : PageModel
    {
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
    } // end class UserAgreementModel
} // end namespace NewEraFlowerStore.Pages.Help
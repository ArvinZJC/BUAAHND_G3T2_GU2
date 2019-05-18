// csharp file that controls validation related to colours

#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Controllers
{
    /// <summary>
    /// Extending from class <see cref="Controller"/>, the class <see cref="ColourController"/> controls validation related to colours.
    /// </summary>
    public class ColourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ColourController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor ColourController

        #region Remote Validation
        /// <summary>
        /// Verify whether the specified colour name is in use or not.
        /// This method is decorated with <see cref="HttpGetAttribute"/>.
        /// </summary>
        /// <returns>a <see cref="JsonResult"/> object that serialises the verification result to JSON</returns>
        [HttpGet]
        public async Task<IActionResult> VerifyNameNotInUseAsync()
        {
            var id = Request.Query["Colour.ID"].ToString();
            var name = Request.Query["Colour.Name"].ToString().Trim();
            var user = await _userManager.GetUserAsync(User);
            var idValue = 0;

            if (user != null
                && (string.IsNullOrWhiteSpace(id)
                    || (!string.IsNullOrWhiteSpace(id)
                        && int.TryParse(id, out idValue)))
                && await _context.Colours.FirstOrDefaultAsync(colour => colour.Name.Equals(name)) != null)
            {
                var currentColour = await _context.Colours.FindAsync(idValue);

                if (currentColour == null
                    || (currentColour != null
                        && currentColour.Name != name))
                    return Json($"The name \"{name}\" is already in use.");
            } // end if

            return Json(true);
        } // end method VerifyNameNotInUseAsync
        #endregion Remote Validation
    } // end class ColourController
} // end namespace NewEraFlowerStore.Controllers
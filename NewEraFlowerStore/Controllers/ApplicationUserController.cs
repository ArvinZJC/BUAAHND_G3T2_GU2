// csharp file that controls validation related to application users

#region Using Directives
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Controllers
{
    /// <summary>
    /// Extending from class <see cref="Controller"/>, the class <see cref="ApplicationUserController"/> controls validation related to application users.
    /// </summary>
    public class ApplicationUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor ApplicationUserController

        #region Remote Validation
        /// <summary>
        /// Verify whether the specified username is in use or not.
        /// This method is decorated with <see cref="HttpGetAttribute"/>.
        /// </summary>
        /// <returns>a <see cref="JsonResult"/> object that serialises the verification result to JSON</returns>
        [HttpGet]
        public async Task<IActionResult> VerifyUsernameNotInUseAsync()
        {
            var username = Request.Query["Input.Username"].ToString();

            if (username != User.Identity.Name
                && await _context.Users.FirstOrDefaultAsync(user => user.UserName.Equals(username)) != null)
                return Json($"The username \"{username}\" is already in use.");

            return Json(true);
        } // end method VerifyUsernameNotInUseAsync

        /// <summary>
        /// Verify whether the specified email is in use or not.
        /// This method is decorated with <see cref="HttpGetAttribute"/>.
        /// </summary>
        /// <returns>a <see cref="JsonResult"/> object that serialises the verification result to JSON</returns>
        [HttpGet]
        public async Task<IActionResult> VerifyEmailNotInUseAsync()
        {
            var email = Request.Query["Input.Email"].ToString();
            var loginUser = await _userManager.GetUserAsync(User);

            if (((loginUser != null
                        && email != loginUser.Email)
                    || loginUser == null)
                && await _context.Users.FirstOrDefaultAsync(user => user.Email.Equals(email)) != null)
                return Json($"The email address \"{email}\" is already in use.");

            return Json(true);
        } // end method VerifyEmailNotInUseAsync

        /// <summary>
        /// Verify whether the specified address book name is in use or not.
        /// This method is decorated with <see cref="HttpGetAttribute"/>.
        /// </summary>
        /// <returns>a <see cref="JsonResult"/> object that serialises the verification result to JSON</returns>
        [HttpGet]
        public async Task<IActionResult> VerifyAddressBookNameAsync()
        {
            var id = Request.Query["AddressBook.ID"].ToString();
            var bookName = Request.Query["AddressBook.BookName"].ToString().Trim();
            var user = await _userManager.GetUserAsync(User);
            var idValue = 0;

            if (user != null
                && (string.IsNullOrWhiteSpace(id)
                    || (!string.IsNullOrWhiteSpace(id)
                        && int.TryParse(id, out idValue)))
                && await _context.AddressBooks
                    .Include(addressBook => addressBook.User)
                    .Where(addressBook => addressBook.UserId == user.Id)
                    .FirstOrDefaultAsync(addressBook => addressBook.BookName.Equals(bookName)) != null)
            {
                var currentAddressBook = await _context.AddressBooks.FindAsync(idValue);

                if (currentAddressBook == null
                    || (currentAddressBook != null
                        && currentAddressBook.BookName != bookName))
                    return Json($"You have already used the address book name \"{bookName}\".");
            } // end if

            return Json(true);
        } // end method VerifyAddressBookNameAsync
        #endregion Remote Validation
    } // end class UsersController
} // end namespace NewEraFlowerStore.Controllers
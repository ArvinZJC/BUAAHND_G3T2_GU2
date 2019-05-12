// csharp file that contains actions of the user centre home page

#region Using Directives
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="IndexModel"/> decorated with <see cref="AuthorizeAttribute"/> contains actions of the user centre home page.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor IndexModel

        /// <summary>
        /// The first name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// The last name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// The URL of the avatar file.
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// The username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// ID of a gender.
        /// </summary>
        public int? GenderId { get; set; }
        /// <summary>
        /// The email address.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// Indicate whether the login user is an administrator or not.
        /// </summary>
        public bool IsAdministrator { get; set; }
        /// <summary>
        /// The date of birth.
        /// </summary>
        public DateTime? DOB { get; set; }
        /// <summary>
        /// The string representation of the date of birth.
        /// </summary>
        public string Dob { get; set; }
        /// <summary>
        /// The phone number.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// The registration time.
        /// </summary>
        public string RegistrationTime { get; set; }
        /// <summary>
        /// The number of address books owned by a user.
        /// </summary>
        public int UserAddressBooksCount { get; set; }
        /// <summary>
        /// The number of matching cart details.
        /// </summary>
        public int MatchingCartDetailsCount { get; set; }
        /// <summary>
        /// The number of incomplete orders.
        /// </summary>
        public int IncompleteOrdersCount { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var localRegistrationTime = user.RegistrationTime.ToLocalTime().DateTime;
            var britainCultureInfo = new CultureInfo("en-GB");
            var userRoles = await _userManager.GetRolesAsync(user);

            FirstName = user.FirstName;
            LastName = user.LastName;
            AvatarUrl = user.AvatarUrl;
            Username = user.UserName;
            GenderId = user.GenderId;
            Email = user.Email;
            IsEmailConfirmed = user.EmailConfirmed;
            IsAdministrator = userRoles.Contains("Administrator");
            DOB = user.DOB;
            PhoneNumber = user.PhoneNumber;
            RegistrationTime = localRegistrationTime.ToString("d MMMM yyyy", britainCultureInfo) + " at " + localRegistrationTime.ToString("HH:mm:ss", britainCultureInfo);

            if (DOB != null)
                Dob = ((DateTime)DOB).ToString("d MMMM yyyy", britainCultureInfo);
            else
                Dob = null;

            if (IsEmailConfirmed)
            {
                UserAddressBooksCount = await _context.AddressBooks
                    .Include(addressBook => addressBook.User)
                    .Where(addressBook => addressBook.UserId == user.Id)
                    .CountAsync();
                MatchingCartDetailsCount = await _context.CartDetails
                    .Include(cartDetail => cartDetail.Bouquet)
                    .Include(cartDetail => cartDetail.User)
                    .Where(cartDetail => cartDetail.UserId == user.Id)
                    .CountAsync();
                IncompleteOrdersCount = await _context.Orders
                    .Include(order => order.User)
                    .Where(order => order.UserId == user.Id
                        && order.OrderStatusId != 9
                        && order.OrderStatusId != 11)
                    .CountAsync();
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", Email);

            return Page();
        } // end method OnGetAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage
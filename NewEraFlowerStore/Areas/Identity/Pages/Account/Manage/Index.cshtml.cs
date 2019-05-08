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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor IndexModel

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }

        public string Username { get; set; }

        public int? GenderId { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsAdministrator { get; set; }

        public DateTime? DOB { get; set; }

        public string Dob { get; set; }

        public string PhoneNumber { get; set; }

        public string RegistrationTime { get; set; }

        public int UserAddressBooksCount { get; set; }

        public int MatchingCartDetailsCount { get; set; }

        public int IncompleteOrdersCount { get; set; }

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
// csharp file that contains actions of the address book list page for an administrator

#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="AddressBooksModel"/> decorated with <see cref="AuthorizeAttribute"/> contains actions of the address book list page for an administrator.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class AddressBooksModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AddressBooksModel> _logger;

        public AddressBooksModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<AddressBooksModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor AddressBooksModel

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// An address book form.
        /// </summary>
        public List<object> AddressBookForm { get; set; }
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

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                AddressBookForm = new List<object>();

                foreach (var item in _context.AddressBooks.Include(addressBook => addressBook.User))
                    AddressBookForm.Add(new
                    {
                        Id = item.ID,
                        item.BookName,
                        item.User.AvatarUrl,
                        Username = item.User.UserName,
                        item.FirstName,
                        item.LastName,
                        item.DetailedAddress,
                        item.ZipOrPostalCode,
                        item.PhoneNumber
                    });
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync
    } // end class AddressBooksModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages
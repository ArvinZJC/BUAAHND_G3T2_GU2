// csharp file that contains actions of the page for creating an address book

#region Using Directives
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.AddressBooks
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="CreateModel"/> contains actions of the page for creating an address book.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CreateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor CreateModel

        /// <summary>
        /// ID of a user.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        /// <summary>
        /// An <see cref="NewEraFlowerStore.Data.AddressBook"/> object decorated with <see cref="BindPropertyAttribute"/>.
        /// </summary>
        [BindProperty]
        public AddressBook AddressBook { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            UserId = _userManager.GetUserId(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                var addressBookCount = await _context.AddressBooks
                    .Include(addressBook => addressBook.User)
                    .Where(addressBook => addressBook.UserId == UserId)
                    .CountAsync();

                // the relevant back-end code in this class and code in the address book list page need updating after modifying the value in the condition
                if (addressBookCount == 10)
                    return NotFound();

                // the relevant back-end code in this class and code in the address book list page need updating after modifying the value in the condition
                if (addressBookCount > 10)
                    return NotFound();
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            UserId = _userManager.GetUserId(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                var addressBookCount = await _context.AddressBooks
                    .Include(addressBook => addressBook.User)
                    .Where(addressBook => addressBook.UserId == UserId)
                    .CountAsync();

                // the relevant back-end code in this class and code in the address book list page need updating after modifying the value in the condition
                if (addressBookCount == 10)
                    return NotFound();

                // the relevant back-end code in this class and code in the address book list page need updating after modifying the value in the condition
                if (addressBookCount > 10)
                    return NotFound();

                if (!ModelState.IsValid)
                    return Page();

                AddressBook.BookName = AddressBook.BookName.Trim();
                AddressBook.UserId = UserId;
                AddressBook.DetailedAddress = AddressBook.DetailedAddress.Trim();
                AddressBook.ZipOrPostalCode = AddressBook.ZipOrPostalCode.Trim();
                AddressBook.PhoneNumber = AddressBook.PhoneNumber.Trim();

                var newAddressBook = new AddressBook();

                if (await TryUpdateModelAsync(
                    newAddressBook,
                    "AddressBook",
                    addressBook => addressBook.BookName,
                    addressBook => addressBook.UserId,
                    addressBook => addressBook.FirstName,
                    addressBook => addressBook.LastName,
                    addressBook => addressBook.DetailedAddress,
                    addressBook => addressBook.ZipOrPostalCode,
                    addressBook => addressBook.PhoneNumber))
                {
                    _context.AddressBooks.Add(AddressBook);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User created specified address book successfully.");
                    StatusMessage = string.Format("An address book with the name \"{0}\" has been created.", AddressBook.BookName);
                    return RedirectToPage("/Account/Manage/AddressBooks/Index", new { area = "Identity" });
                } // end if

                _logger.LogError("Error! Failed to create specified address book.");
                StatusMessage = string.Format("Error! Failed to create an address book with the name \"{0}\". You may try again.", AddressBook.BookName);
                return Page();
            } // end if

            StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            return Page();
        } // end method OnPostAsync
    } // end class CreateModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.AddressBooks
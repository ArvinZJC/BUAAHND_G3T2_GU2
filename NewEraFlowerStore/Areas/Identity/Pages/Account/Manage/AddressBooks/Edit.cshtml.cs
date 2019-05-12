// csharp file that contains actions of the page for editing an address book

#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.AddressBooks
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="EditModel"/> contains actions of the page for editing an address book.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<EditModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor EditModel

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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            UserId = _userManager.GetUserId(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                AddressBook = await _context.AddressBooks.FindAsync(id);

                if (AddressBook == null || AddressBook.UserId != UserId)
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
                if (!ModelState.IsValid)
                    return Page();

                var addressBookToUpdate = await _context.AddressBooks.FindAsync(AddressBook.ID);

                if (addressBookToUpdate == null || addressBookToUpdate.UserId != UserId)
                    return NotFound();

                AddressBook.BookName = AddressBook.BookName.Trim();
                AddressBook.DetailedAddress = AddressBook.DetailedAddress.Trim();
                AddressBook.ZipOrPostalCode = AddressBook.ZipOrPostalCode.Trim();
                AddressBook.PhoneNumber = AddressBook.PhoneNumber.Trim();

                if (addressBookToUpdate.BookName != AddressBook.BookName
                    || addressBookToUpdate.FirstName != AddressBook.FirstName
                    || addressBookToUpdate.LastName != AddressBook.LastName
                    || addressBookToUpdate.DetailedAddress != AddressBook.DetailedAddress
                    || addressBookToUpdate.ZipOrPostalCode != AddressBook.ZipOrPostalCode
                    || addressBookToUpdate.PhoneNumber != AddressBook.PhoneNumber)
                {
                    if (await TryUpdateModelAsync(
                        addressBookToUpdate,
                        "AddressBook",
                        addressBook => addressBook.BookName,
                        addressBook => addressBook.FirstName,
                        addressBook => addressBook.LastName,
                        addressBook => addressBook.DetailedAddress,
                        addressBook => addressBook.ZipOrPostalCode,
                        addressBook => addressBook.PhoneNumber))
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User updated specified address book successfully.");
                        StatusMessage = string.Format("The address book with the name \"{0}\" has been updated.", AddressBook.BookName);
                    }
                    else
                    {
                        _logger.LogError("Error! Failed to update specified address book.");
                        StatusMessage = string.Format("Error! Failed to update the address book with the name \"{0}\". You may try again.", AddressBook.BookName);
                        return Page();
                    } // end if...else
                } // end if
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Account/Manage/AddressBooks/Index", new { area = "Identity" });
        } // end method OnPostAsync
    } // end class EditModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.AddressBooks
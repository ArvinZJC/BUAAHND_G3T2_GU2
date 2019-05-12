// csharp file that contains actions of the address book list page for a customer

#region Using Directives
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.AddressBooks
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="IndexModel"/> contains actions of the address book list page for a customer.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor IndexModel

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// The number of address books owned by a user.
        /// </summary>
        public int UserAddressBooksCount { get; set; }
        /// <summary>
        /// The number of matching address books.
        /// </summary>
        public int MatchingAddressBooksCount { get; set; }
        /// <summary>
        /// The index of the current page.
        /// </summary>
        public int? CurrentPageIndex { get; set; }
        /// <summary>
        /// The current filter.
        /// </summary>
        public string CurrentFilter { get; set; }
        /// <summary>
        /// A paginated list containing matching address books.
        /// </summary>
        public PaginatedList<AddressBook> MatchingAddressBookList { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        /// <summary>
        /// An <see cref="NewEraFlowerStore.Data.AddressBook"/> object decorated with <see cref="BindPropertyAttribute"/>.
        /// </summary>
        [BindProperty]
        private AddressBook AddressBook { get; set; }

        public async Task<IActionResult> OnGetAsync(int? pageIndex, string currentFilter, string searchString)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                IQueryable<AddressBook> matchingAddressBooks = from addressBook in _context.AddressBooks
                                                               select addressBook;

                matchingAddressBooks = matchingAddressBooks
                    .Include(addressBook => addressBook.User)
                    .Where(addressBook => addressBook.UserId == user.Id);
                UserAddressBooksCount = await matchingAddressBooks.CountAsync();

                if (UserAddressBooksCount > 0)
                {
                    if (string.IsNullOrWhiteSpace(searchString))
                        searchString = currentFilter;
                    else
                        pageIndex = 1;

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        CurrentFilter = searchString.Trim();
                        var splitSearchString = Regex.Split(CurrentFilter + " ", @"[\s]+");
                        matchingAddressBooks = matchingAddressBooks.Where(addressBook => addressBook.BookName.Contains(CurrentFilter)
                            || addressBook.FirstName.Contains(CurrentFilter)
                            || addressBook.LastName.Contains(CurrentFilter)
                            || (addressBook.FirstName.Contains(splitSearchString[0]) && addressBook.LastName.Contains(splitSearchString[1]))
                            || addressBook.DetailedAddress.Contains(CurrentFilter)
                            || addressBook.ZipOrPostalCode.Contains(CurrentFilter)
                            || addressBook.PhoneNumber.Contains(CurrentFilter));
                        MatchingAddressBooksCount = await matchingAddressBooks.CountAsync();
                    } // end if

                    matchingAddressBooks = matchingAddressBooks.OrderBy(addressBook => addressBook.BookName);

                    // the relevant back-end code in this class and code in the address book list page need updating after modifying the value in the condition
                    if (UserAddressBooksCount == 10)
                        StatusMessage = "Warning! You already have 10 address books. Delete some if you want to create more.";

                    // the relevant back-end code in this class and code in the address book list page need updating after modifying the value in the condition
                    if (UserAddressBooksCount > 10)
                        StatusMessage = string.Format("Error! You have {0} address books which exceed the limit. Only 10 of them are displayed. Please delete some.", UserAddressBooksCount);
                } // end if

                int pageSize = 4; // the code of pagination in the address book list page needs improving after modifying the page size

                CurrentPageIndex = (pageIndex == null || pageIndex <= 0) ? 1 : pageIndex;

                MatchingAddressBookList = await PaginatedList<AddressBook>.CreateAsync(
                    matchingAddressBooks.AsNoTracking(),
                    (int)CurrentPageIndex,
                    pageSize);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                if (id != null)
                {
                    AddressBook = await _context.AddressBooks.FindAsync(id);

                    if (AddressBook == null || AddressBook.UserId != user.Id)
                        return NotFound();

                    try
                    {
                        _context.AddressBooks.Remove(AddressBook);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User deleted specified address book succesfully.");
                        StatusMessage = string.Format("The address book with the name \"{0}\" has been deleted.", AddressBook.BookName);
                    }
                    catch (DbUpdateException e)
                    {
                        _logger.LogError(e, "Error! Failed to delete specified address book.");
                        StatusMessage = string.Format("Error! Failed to delete the address book with the name \"{0}\". You may try again.", AddressBook.BookName);
                    } // end try...catch
                }
                else
                {
                    var matchingAddressBooks = _context.AddressBooks
                        .Include(addressBook => addressBook.User)
                        .Where(addressBook => addressBook.UserId == user.Id);

                    try
                    {
                        _context.AddressBooks.RemoveRange(matchingAddressBooks);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User deleted all address books succesfully.");
                        StatusMessage = "All address books have been deleted.";
                    }
                    catch (DbUpdateException e)
                    {
                        _logger.LogError(e, "Error! Failed to delete all address books.");
                        StatusMessage = "Error! Failed to delete all address books. You may try again.";
                    } // end try...catch
                } // end if...else
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("./Index");
        } // end method OnPostDeleteAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.AddressBooks
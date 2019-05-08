#region Using Directives
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Bouquets
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor DetailsModel

        public int BouquetsCount { get; set; }

        public int MatchingCartDetailsCount { get; set; }

        public int AvaliableStocks { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsAdministrator { get; set; }

        public Bouquet Bouquet { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Bouquet = await _context.Bouquets
                .Include(bouquet => bouquet.Colour)
                .Include(bouquet => bouquet.Flower)
                .Include(bouquet => bouquet.Occasion)
                .FirstOrDefaultAsync(bouquet => bouquet.ID == id);

            if (Bouquet == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            BouquetsCount = await _context.Bouquets.CountAsync();

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                IsEmailConfirmed = user.EmailConfirmed;
                IsAdministrator = userRoles.Contains("Administrator");

                if (IsEmailConfirmed)
                {
                    var matchingCartDetails = _context.CartDetails
                        .Include(cartDetail => cartDetail.Bouquet)
                        .Include(cartDetail => cartDetail.User)
                        .Where(cartDetail => cartDetail.UserId == user.Id);
                    var existingCartDetail = await matchingCartDetails.FirstOrDefaultAsync(cartDetail => cartDetail.BouquetId == id);

                    if (existingCartDetail != null)
                    {
                        var avaliableStocks = Bouquet.Stocks - existingCartDetail.Quantity;

                        AvaliableStocks = avaliableStocks > 0 ? avaliableStocks : 0;
                    }
                    else
                        AvaliableStocks = Bouquet.Stocks;

                    MatchingCartDetailsCount = await matchingCartDetails.CountAsync();
                }
                else
                    StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            }
            else
            {
                IsEmailConfirmed = false;
                IsAdministrator = false;
                MatchingCartDetailsCount = 0;
            }

            return Page();
        } // end method OnGetAsync
    } // end class DetailsModel
} // end namespace NewEraFlowerStore.Pages.Bouquets
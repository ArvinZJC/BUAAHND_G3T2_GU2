// csharp file that contains actions of the bouquet list page for all bouquets or matching bouquets according to the search string, price, colour, flower, and occasion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using NewEraFlowerStore.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Bouquets
{
    /// <summary>
    /// Extending from the class <see cref="PageModel"/>, the class <see cref="ListModel"/> contains actions of the bouquet list page for all bouquets or matching bouquets according to the search string, price, colour, flower, and occasion.
    /// </summary>
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor ListModel

        /// <summary>
        /// The number of bouquets.
        /// </summary>
        public int BouquetsCount { get; set; }
        /// <summary>
        /// The number of matching bouquets.
        /// </summary>
        public int MatchingBouquetsCount { get; set; }
        /// <summary>
        /// The number of matching cart details.
        /// </summary>
        public int MatchingCartDetailsCount { get; set; }
        /// <summary>
        /// ID of the current flower.
        /// </summary>
        public int? CurrentFlowerId { get; set; }
        /// <summary>
        /// ID of the current occasion.
        /// </summary>
        public int? CurrentOccasionId { get; set; }
        /// <summary>
        /// ID of the current colour.
        /// </summary>
        public int? CurrentColourId { get; set; }
        /// <summary>
        /// The current page index.
        /// </summary>
        public int? CurrentPageIndex { get; set; }
        /// <summary>
        /// The current lowest price.
        /// </summary>
        public decimal? CurrentLowestPrice { get; set; }
        /// <summary>
        /// The current highest price.
        /// </summary>
        public decimal? CurrentHighestPrice { get; set; }
        /// <summary>
        /// The current sort order.
        /// </summary>
        public string CurrentSortOrder { get; set; }
        /// <summary>
        /// The current filter.
        /// </summary>
        public string CurrentFilter { get; set; }
        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// Indicate whether the login user is an administrator or not.
        /// </summary>
        public bool IsAdministrator { get; set; }
        /// <summary>
        /// A colour list.
        /// </summary>
        public IList<Colour> ColourList { get; set; }
        /// <summary>
        /// A <see cref="Flower"/> object.
        /// </summary>
        public Flower CurrentFlower { get; set; }
        /// <summary>
        /// A flower list.
        /// </summary>
        public IList<Flower> FlowerList { get; set; }
        /// <summary>
        /// An <see cref="Occasion"/> object.
        /// </summary>
        public Occasion CurrentOccasion { get; set; }
        /// <summary>
        /// An occasion list.
        /// </summary>
        public IList<Occasion> OccasionList { get; set; }
        /// <summary>
        /// Matching cart details.
        /// </summary>
        public IQueryable<CartDetail> MatchingCartDetails { get; set; }
        /// <summary>
        /// A paginated list of matching bouquets.
        /// </summary>
        public PaginatedList<Bouquet> MatchingBouquetList { get;set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(
            string currentFilter,
            string searchString,
            string sortOrder,
            decimal? lowestPrice,
            decimal? highestPrice,
            int? colourId,
            int? flowerId,
            int? occasionId,
            int? pageIndex)
        {

            IQueryable<Bouquet> matchingBouquets = from bouquet in _context.Bouquets
                                                   select bouquet;

            BouquetsCount = await matchingBouquets.CountAsync();
            matchingBouquets = matchingBouquets
                .Include(bouquet => bouquet.Colour)
                .Include(bouquet => bouquet.Flower)
                .Include(bouquet => bouquet.Occasion);
            ColourList = await _context.Colours.ToListAsync();
            FlowerList = await _context.Flowers.ToListAsync();
            OccasionList = await _context.Occasions.ToListAsync();

            if (BouquetsCount > 0)
            {
                if (flowerId != null && occasionId == null)
                {
                    var currentFlower = await _context.Flowers.FindAsync(flowerId);

                    if (flowerId == -2 || flowerId == -1 || flowerId == 0 || currentFlower != null)
                    {
                        CurrentFlower = currentFlower;
                        CurrentFlowerId = flowerId;

                        switch (flowerId)
                        {
                            // the flower menu item "Best sellers"
                            case -2:
                                matchingBouquets = matchingBouquets.Where(bouquet => bouquet.Sales >= 1000); // the relevant code in the home page, bouquet list page, and bouquet detail page needs updating after modifying the lambda expression
                                break;

                            // the flower menu item "New arrivals"
                            case -1:
                                matchingBouquets = matchingBouquets.Where(bouquet => new TimeSpan(DateTime.Now.Ticks - bouquet.LaunchDate.Ticks).TotalDays <= 14); // the relevant code in the home page, bouquet list page, and bouquet detail page needs updating after modifying the lambda expression
                                break;

                            // the flower menu item "Sale"
                            case 0:
                                matchingBouquets = matchingBouquets.Where(bouquet => bouquet.Discount > 0M); // the relevant code in the home page, bouquet list page, and bouquet detail page needs updating after modifying the lambda expression
                                break;

                            default:
                                matchingBouquets = matchingBouquets.Where(bouquet => bouquet.FlowerId == flowerId);
                                break;
                        } // end switch-case
                    }
                    else
                        return NotFound();
                }
                else if (flowerId == null && occasionId != null)
                {
                    var currentOccasion = await _context.Occasions.FindAsync(occasionId);

                    if (currentOccasion != null)
                    {
                        CurrentOccasion = currentOccasion;
                        CurrentOccasionId = occasionId;

                        matchingBouquets = matchingBouquets.Where(bouquet => bouquet.OccasionId == occasionId);
                    }
                    else
                        return NotFound();
                }
                else if (flowerId != null
                    && occasionId != null
                    && (flowerId == -2
                    || flowerId == -1
                    || flowerId == 0
                    || await _context.Flowers.FindAsync(flowerId) != null)
                    && await _context.Occasions.FindAsync(occasionId) != null)
                {
                    CurrentFlowerId = flowerId;
                    CurrentOccasionId = occasionId;

                    matchingBouquets = matchingBouquets.Where(bouquet => bouquet.FlowerId == flowerId && bouquet.OccasionId == occasionId);
                } // end nested if...else

                if (string.IsNullOrWhiteSpace(searchString))
                    searchString = currentFilter;
                else
                    pageIndex = 1;

                if (!string.IsNullOrEmpty(searchString))
                {
                    CurrentFilter = searchString.Trim();
                    matchingBouquets = matchingBouquets.Where(bouquet => bouquet.Name.Contains(CurrentFilter)
                        || bouquet.Colour.Name.Contains(CurrentFilter)
                        || bouquet.Flower.Name.Contains(CurrentFilter)
                        || bouquet.Occasion.Name.Contains(CurrentFilter));
                } // end if

                if (lowestPrice != null
                    && highestPrice != null
                    && lowestPrice >= 0.01M
                    && lowestPrice <= 999.99M
                    && highestPrice >= 0.01M
                    && highestPrice <= 999.99M
                    && lowestPrice <= highestPrice)
                {
                    lowestPrice = DecimalHelper.ToPriceFormat((decimal)lowestPrice);
                    highestPrice = DecimalHelper.ToPriceFormat((decimal)highestPrice);
                    CurrentLowestPrice = lowestPrice;
                    CurrentHighestPrice = highestPrice;

                    matchingBouquets = matchingBouquets.Where(bouquet => bouquet.OriginalPrice * (1M - bouquet.Discount) >= CurrentLowestPrice
                                                              && bouquet.OriginalPrice * (1M - bouquet.Discount) <= CurrentHighestPrice);
                } // end if

                if (colourId != null)
                {
                    if (await _context.Colours.FindAsync(colourId) != null)
                    {
                        CurrentColourId = colourId;

                        matchingBouquets = matchingBouquets.Where(bouquet => bouquet.ColourId == colourId);
                    }
                    else
                        return NotFound();
                } // end if

                CurrentSortOrder = sortOrder != null ? sortOrder.Trim() : string.Empty;

                switch (CurrentSortOrder)
                {
                    case "name_descending":
                        matchingBouquets = matchingBouquets.OrderByDescending(bouquet => bouquet.Name);
                        break;

                    case "price_ascending":
                        matchingBouquets = matchingBouquets.OrderBy(bouquet => bouquet.OriginalPrice * (1M - bouquet.Discount)).ThenBy(bouquet => bouquet.Name);
                        break;

                    case "price_descending":
                        matchingBouquets = matchingBouquets.OrderByDescending(bouquet => bouquet.OriginalPrice * (1M - bouquet.Discount)).ThenBy(bouquet => bouquet.Name);
                        break;

                    case "sales_ascending":
                        matchingBouquets = matchingBouquets.OrderBy(bouquet => bouquet.Sales).ThenBy(bouquet => bouquet.Name);
                        break;

                    case "sales_descending":
                        matchingBouquets = matchingBouquets.OrderByDescending(bouquet => bouquet.Sales).ThenBy(bouquet => bouquet.Name);
                        break;

                    case "launchDate_ascending":
                        matchingBouquets = matchingBouquets.OrderBy(bouquet => bouquet.LaunchDate).ThenBy(bouquet => bouquet.Name);
                        break;

                    case "launchDate_descending":
                        matchingBouquets = matchingBouquets.OrderByDescending(bouquet => bouquet.LaunchDate).ThenBy(bouquet => bouquet.Name);
                        break;

                    default:
                        CurrentSortOrder = string.Empty;
                        matchingBouquets = matchingBouquets.OrderBy(bouquet => bouquet.Name);
                        break;
                } // end switch-case

                var user = await _userManager.GetUserAsync(User);
                MatchingBouquetsCount = await matchingBouquets.CountAsync();

                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    IsEmailConfirmed = user.EmailConfirmed;
                    IsAdministrator = userRoles.Contains("Administrator");

                    if (IsEmailConfirmed)
                    {
                        MatchingCartDetails = _context.CartDetails
                            .Include(cartDetail => cartDetail.Bouquet)
                            .Include(cartDetail => cartDetail.User)
                            .Where(cartDetail => cartDetail.UserId == user.Id);
                        MatchingCartDetailsCount = await MatchingCartDetails.CountAsync();
                    }
                    else
                        StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
                }
                else
                {
                    IsEmailConfirmed = false;
                    IsAdministrator = false;
                    MatchingCartDetails = null;
                    MatchingCartDetailsCount = 0;
                } // end if...else
            }
            else
                StatusMessage = "Error! No bouquet found on shelves.";

            int pageSize = 12; // the code of pagination in the bouquet list page needs improving after modifying the page size

            CurrentPageIndex = (pageIndex == null || pageIndex <= 0) ? 1 : pageIndex;

            MatchingBouquetList = await PaginatedList<Bouquet>.CreateAsync(
                matchingBouquets.AsNoTracking(),
                (int)CurrentPageIndex,
                pageSize);
            return Page();
        } // end method OnGetAsync
    } // end class ListModel
} // end namespace NewEraFlowerStore.Pages.Bouquets
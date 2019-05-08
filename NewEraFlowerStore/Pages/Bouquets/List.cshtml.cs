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
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor ListModel

        public int BouquetsCount { get; set; }

        public int MatchingBouquetsCount { get; set; }

        public int MatchingCartDetailsCount { get; set; }

        public int? CurrentFlowerId { get; set; }

        public int? CurrentOccasionId { get; set; }

        public int? CurrentColourId { get; set; }

        public int? CurrentPageIndex { get; set; }

        public decimal? CurrentLowestPrice { get; set; }

        public decimal? CurrentHighestPrice { get; set; }

        public string CurrentSortOrder { get; set; }

        public string CurrentFilter { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsAdministrator { get; set; }

        public IList<Colour> ColourList { get; set; }

        public Flower CurrentFlower { get; set; }

        public IList<Flower> FlowerList { get; set; }

        public Occasion CurrentOccasion { get; set; }

        public IList<Occasion> OccasionList { get; set; }

        public IQueryable<CartDetail> MatchingCartDetails { get; set; }

        public PaginatedList<Bouquet> MatchingBouquetList { get;set; }

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
                StatusMessage = "Error! No bouquet found on shelves";

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
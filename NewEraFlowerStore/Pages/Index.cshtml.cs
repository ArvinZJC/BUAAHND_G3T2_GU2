// csharp file that contains actions of the home page

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
#endregion Using Directives

namespace NewEraFlowerStore.Pages
{
    /// <summary>
    /// Extending from the class <see cref="PageModel"/>, the class <see cref="IndexModel"/> contains actions of the home page.
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
        /// The number of bouquets.
        /// </summary>
        public int BouquetsCount { get; set; }
        /// <summary>
        /// The number of flowers.
        /// </summary>
        public int FlowersCount { get; set; }
        /// <summary>
        /// The number of occasions.
        /// </summary>
        public int OccasionsCount { get; set; }
        /// <summary>
        /// The number of best sellers.
        /// </summary>
        public int BestSellersCount { get; set; }
        /// <summary>
        /// The number of new arrivals.
        /// </summary>
        public int NewArrivalsCount { get; set; }
        /// <summary>
        /// The number of bouquets on sale.
        /// </summary>
        public int SaleCount { get; set; }
        /// <summary>
        /// The number of matching cart details.
        /// </summary>
        public int MatchingCartDetailsCount { get; set; }
        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// Indicate whether the login user is an administrator or not.
        /// </summary>
        public bool IsAdministrator { get; set; }
        /// <summary>
        /// A flower list.
        /// </summary>
        public IList<Flower> FlowerList { get; set; }
        /// <summary>
        /// An occasion list.
        /// </summary>
        public IList<Occasion> OccasionList { get; set; }
        /// <summary>
        /// A list of best sellers.
        /// </summary>
        public IList<Bouquet> BestSellerList { get; set; }
        /// <summary>
        /// A list of new arrivals.
        /// </summary>
        public IList<Bouquet> NewArrivalList { get; set; }
        /// <summary>
        /// A list of bouquets on sale.
        /// </summary>
        public IList<Bouquet> SaleList { get; set; }
        /// <summary>
        /// Matching cart details.
        /// </summary>
        public IQueryable<CartDetail> MatchingCartDetails { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var flowers = _context.Flowers.OrderBy(flower => flower.Name);
            var occasions = _context.Occasions.OrderBy(occasion => occasion.Name);
            var bestSellers = _context.Bouquets
                .Where(bouquet => bouquet.Sales >= 1000)
                .OrderByDescending(bouquet => bouquet.Sales)
                .ThenBy(product => product.Name); // the lambda expression should be the same as that in the back-end code of the bouquet list page
            var newArrivals = _context.Bouquets
                .Where(bouquet => new TimeSpan(DateTime.Now.Ticks - bouquet.LaunchDate.ToLocalTime().Ticks).TotalDays <= 14)
                .OrderByDescending(bouquet => bouquet.LaunchDate)
                .ThenBy(product => product.Name); // the lambda expression should be the same as that in the back-end code of the bouquet list page
            var sale = _context.Bouquets
                .Where(bouquet => bouquet.Discount > 0M)
                .OrderByDescending(bouquet => bouquet.Discount)
                .ThenBy(product => product.Name); // the lambda expression should be the same as that in the back-end code of the bouquet list page
            var user = await _userManager.GetUserAsync(User);
            BouquetsCount = await _context.Bouquets.CountAsync();
            FlowerList = flowers.ToList();
            FlowersCount = await flowers.CountAsync();
            OccasionList = occasions.ToList();
            OccasionsCount = await occasions.CountAsync();
            BestSellerList = bestSellers.ToList();
            BestSellersCount = await bestSellers.CountAsync();
            NewArrivalList = newArrivals.ToList();
            NewArrivalsCount = await newArrivals.CountAsync();
            SaleList = sale.ToList();
            SaleCount = await sale.CountAsync();

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
        } // end method OnGetAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Pages
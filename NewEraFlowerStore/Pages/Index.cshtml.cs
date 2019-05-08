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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } // end constructor IndexModel

        public int BouquetsCount { get; set; }

        public int FlowersCount { get; set; }

        public int OccasionsCount { get; set; }

        public int BestSellersCount { get; set; }

        public int NewArrivalsCount { get; set; }

        public int SaleCount { get; set; }

        public int MatchingCartDetailsCount { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsAdministrator { get; set; }

        public IList<Flower> FlowerList { get; set; }

        public IList<Occasion> OccasionList { get; set; }

        public IList<Bouquet> BestSellerList { get; set; }

        public IList<Bouquet> NewArrivalList { get; set; }

        public IList<Bouquet> SaleList { get; set; }

        public IQueryable<CartDetail> MatchingCartDetails { get; set; }

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
            }
        } // end method OnGetAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Pages
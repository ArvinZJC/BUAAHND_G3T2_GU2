#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using NewEraFlowerStore.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class DashboardModel : PageModel
    {
        private readonly OrderStatusListItem _orderStatusListItem;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            _orderStatusListItem = new OrderStatusListItem();
            OrderStatusList = _orderStatusListItem.GetOrderStatusList();
        } // end constructor DashboardModel

        public bool IsEmailConfirmed { get; set; }

        public bool HasRecentMonthsSalesAmountChart { get; set; }

        public int RunningDays { get; set; }

        public int RunningMonths { get; set; }

        public int RegisteredCustomersCount { get; set; }

        public int ColoursCount { get; set; }

        public int FlowersCount { get; set; }

        public int OccasionsCount { get; set; }

        public int BouquetsSoldOutCount { get; set; }

        public decimal TotalSalesAmount { get; set; }

        public decimal SalesAmount1 { get; set; }

        public List<OrderStatusListItem> OrderStatusList { get; set; }

        public List<object> RecentlyRegisteredCustomersChart { get; set; }

        public List<object> SortByColourChart { get; set; }

        public List<object> SortByFlowerChart { get; set; }

        public List<object> SortByOccasionChart { get; set; }

        public List<object> IncompleteOrdersForm { get; set; }

        public List<object> RecentMonthsSalesAmountChart { get; set; }

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
                #region Registered Customers
                RunningDays = (int)new TimeSpan(DateTime.Now.Ticks - new DateTime(2018, 12, 1).Ticks).TotalDays;

                var dayInterval = 0;
                var customersCount7 = 0;
                var customersCount14 = 0;
                var customersCount28 = 0;

                foreach (var item in _context.Users)
                {
                    var roles = await _userManager.GetRolesAsync(item);

                    if (roles.Contains("Customer"))
                    {
                        RegisteredCustomersCount++;
                        dayInterval = (int)new TimeSpan(DateTimeOffset.Now.Ticks - item.RegistrationTime.ToLocalTime().Ticks).TotalDays;

                        if (dayInterval <= 7)
                            customersCount7++;

                        if (dayInterval <= 14)
                            customersCount14++;

                        if (dayInterval <= 28)
                            customersCount28++;
                    } // end if
                } // end foreach

                RecentlyRegisteredCustomersChart = new List<object>
                {
                    new
                    {
                        Category = "Last week",
                        Count = customersCount7
                    },

                    new
                    {
                        Category = "Last 2 weeks",
                        Count = customersCount14
                    },

                    new
                    {
                        Category = "Last 4 weeks",
                        Count = customersCount28
                    }
                };
                #endregion Registered Customers

                #region Bouquets
                ColoursCount = await _context.Colours.CountAsync();
                FlowersCount = await _context.Flowers.CountAsync();
                OccasionsCount = await _context.Occasions.CountAsync();
                BouquetsSoldOutCount = await _context.Bouquets.Where(bouquet => bouquet.Stocks == 0).CountAsync();

                if (ColoursCount > 0)
                {
                    SortByColourChart = new List<object>();

                    foreach (var item in _context.Colours)
                    {
                        SortByColourChart.Add(new
                        {
                            item.Name,
                            BouquetsCount = await _context.Bouquets.Where(bouquet => bouquet.ColourId == item.ID).CountAsync()
                        });
                    } // end foreach
                } // end if

                if (FlowersCount > 0)
                {
                    SortByFlowerChart = new List<object>();

                    var random = new Random();

                    foreach (var item in _context.Flowers)
                    {
                        SortByFlowerChart.Add(new
                        {
                            item.Name,
                            BouquetsCount = await _context.Bouquets.Where(bouquet => bouquet.FlowerId == item.ID).CountAsync(),
                            Radius = random.Next(100, 161).ToString(), // select a random integer between 100 and 160 and convert it to its equivalent string representation as the pie radius
                        });
                    } // end foreach
                } // end if

                if (OccasionsCount > 0)
                {
                    SortByOccasionChart = new List<object>();

                    foreach (var item in _context.Occasions)
                    {
                        SortByOccasionChart.Add(new
                        {
                            item.Name,
                            BouquetsCount = await _context.Bouquets.Where(bouquet => bouquet.OccasionId == item.ID).CountAsync()
                        });
                    } // end foreach
                } // end if
                #endregion Bouquets

                #region Incomplete Orders
                IncompleteOrdersForm = new List<object>
                {
                    new
                    {
                        Failed = await _context.Orders.Where(order => order.OrderStatusId == 2).CountAsync(),
                        Refunding = await _context.Orders.Where(order => order.OrderStatusId == 10).CountAsync(),
                        Fulfillment = await _context.Orders.Where(order => order.OrderStatusId == 5).CountAsync(),
                        Delivery = await _context.Orders.Where(order => order.OrderStatusId == 6).CountAsync(),
                        Delivering = await _context.Orders.Where(order => order.OrderStatusId == 7).CountAsync(),
                        Info = await _context.Orders.Where(order => order.OrderStatusId == 1).CountAsync(),
                        Payment = await _context.Orders.Where(order => order.OrderStatusId == 4).CountAsync(),
                        Delivered = await _context.Orders.Where(order => order.OrderStatusId == 8).CountAsync(),
                    }
                };
                #endregion Incomplete Orders

                #region Sales Amount
                RunningMonths = (DateTime.Now.Year - 2018) * 12 + DateTime.Now.Month - 12;
                HasRecentMonthsSalesAmountChart = false;

                var monthInterval = 0;
                var salesAmount2 = 0M;
                var salesAmount3 = 0M;
                var salesAmount4 = 0M;

                foreach (var item in _context.SalesRecords)
                {
                    TotalSalesAmount += item.SalesAmount;
                    monthInterval = (DateTimeOffset.Now.Year - item.CreationTime.Year) * 12 + DateTimeOffset.Now.Month - item.CreationTime.Month;

                    if (monthInterval > 0)
                    {
                        switch (monthInterval)
                        {
                            case 1:
                                SalesAmount1 += item.SalesAmount;
                                break;

                            case 2:
                                salesAmount2 += item.SalesAmount;
                                break;

                            case 3:
                                salesAmount3 += item.SalesAmount;
                                break;

                            case 4:
                                salesAmount4 += item.SalesAmount;
                                break;
                        } // end switch-case
                    } // end if
                } // end foreach

                if (RunningMonths >= 2)
                {
                    var month1 = DateTime.Now.AddMonths(-1);
                    var month2 = DateTime.Now.AddMonths(-2);

                    HasRecentMonthsSalesAmountChart = true;
                    RecentMonthsSalesAmountChart = new List<object>
                    {
                        new
                        {
                            Month = new DateTime(month1.Year, month1.Month, 1),
                            SalesAmount = DecimalHelper.ToPriceFormat(SalesAmount1)
                        },

                        new
                        {
                            Month = new DateTime(month2.Year, month2.Month, 1),
                            SalesAmount = DecimalHelper.ToPriceFormat(salesAmount2)
                        }
                    };

                    if (RunningMonths >= 3)
                    {
                        var month3 = DateTime.Now.AddMonths(-3);

                        RecentMonthsSalesAmountChart.Add(new
                        {
                            Month = new DateTime(month3.Year, month3.Month, 1),
                            SalesAmount = DecimalHelper.ToPriceFormat(salesAmount3)
                        });
                    } // end if
                } // end if
                #endregion Sales Amount
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync
    } // end class DashboardModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages
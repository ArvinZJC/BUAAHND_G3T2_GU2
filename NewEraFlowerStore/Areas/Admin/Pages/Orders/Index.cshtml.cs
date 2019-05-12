// csharp file that contains actions of the order list page for an administrator

#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Orders
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="IndexModel"/> contains actions of the order list page for an administrator.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly OrderStatusListItem _orderStatusListItem;
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

            _orderStatusListItem = new OrderStatusListItem();
            OrderStatusList = _orderStatusListItem.GetOrderStatusList();
        } // end constructor IndexModel

        private List<OrderStatusListItem> OrderStatusList { get; }
        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// An order form.
        /// </summary>
        public List<object> OrderForm { get; set; }
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
                var isChanged = false;

                OrderForm = new List<object>();

                foreach (var item in _context.Orders.Include(orders => orders.User))
                {
                    if (item.OrderStatusId == 1 && new TimeSpan(DateTimeOffset.Now.Ticks - item.OrderTime.ToLocalTime().Ticks).TotalMinutes > 15)
                    {
                        item.OrderStatusId = 2;
                        _context.Attach(item).State = EntityState.Modified;
                        isChanged = true;
                    }

                    if (item.OrderStatusId == 4 && new TimeSpan(DateTimeOffset.Now.Ticks - item.OrderTime.ToLocalTime().Ticks).TotalMinutes > 30)
                    {
                        item.OrderStatusId = 2;
                        _context.Attach(item).State = EntityState.Modified;
                        isChanged = true;
                    }

                    if (item.OrderStatusId == 8)
                    {
                        if (item.DeliveryTime != null)
                        {
                            if (new TimeSpan(DateTimeOffset.Now.Ticks - ((DateTimeOffset)item.DeliveryTime).ToLocalTime().Ticks).TotalHours > 24)
                            {
                                var matchingOrderDetails = _context.OrderDetails
                                    .Include(orderDetail => orderDetail.Order)
                                    .Where(orderDetail => orderDetail.OrderId == item.ID);
                                var subtotal = 0M;

                                if (await matchingOrderDetails.CountAsync() > 0)
                                {
                                    foreach (var orderDetail in await matchingOrderDetails.ToListAsync())
                                        subtotal += DecimalHelper.ToPriceFormat(orderDetail.Price) * orderDetail.Quantity;

                                    if (subtotal > 0)
                                    {
                                        var newSalesRecord = new SalesRecord
                                        {
                                            SalesAmount = DecimalHelper.ToPriceFormat(subtotal),
                                            CreationTime = DateTimeOffset.Now
                                        };

                                        _context.SalesRecords.Add(newSalesRecord);
                                    }
                                }

                                item.OrderStatusId = 9;
                                item.CompletionTime = DateTimeOffset.Now;
                                _context.Attach(item).State = EntityState.Modified;
                                isChanged = true;
                            }
                        }
                        else
                            StatusMessage = "Error! Missing delivery time has been detected. Please check your orders delivered.";
                    }

                    OrderForm.Add(new
                    {
                        Id = item.ID,
                        item.User.AvatarUrl,
                        Username = item.User.UserName,
                        item.OrderStatusId,
                        OrderStatus = OrderStatusList.FirstOrDefault(orderStatusListItem => orderStatusListItem.ID == item.OrderStatusId).DisplayName,
                        OrderTime = item.OrderTime.ToLocalTime(),
                        PaymentTime = item.PaymentTime == null ? item.PaymentTime : ((DateTimeOffset)item.PaymentTime).ToLocalTime(),
                        DeliveryTime = item.DeliveryTime == null ? item.DeliveryTime : ((DateTimeOffset)item.DeliveryTime).ToLocalTime(),
                        CompletionTime = item.CompletionTime == null ? item.CompletionTime : ((DateTimeOffset)item.CompletionTime).ToLocalTime()
                    });
                }

                if (isChanged)
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Relevant order status has been updated successfully due to timeout.");

                        StatusMessage = "Error! Timeout has been detected. The relevant order status has been updated.";
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        _logger.LogError(e, "Error! Failed to update relevant order status, although timeout has been detected.");

                        StatusMessage = "Error! Failed to update the relevant order status, although timeout has been detected. You may try again.";
                    }
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Orders
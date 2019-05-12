// csharp file that contains actions of the order detail page for an administrator

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
    /// Extending from class <see cref="PageModel"/>, the class <see cref="DetailsModel"/> contains actions of the order detail page for an administrator.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly OrderStatusListItem _orderStatusListItem;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DetailsModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;

            _orderStatusListItem = new OrderStatusListItem();
            OrderStatusList = _orderStatusListItem.GetOrderStatusList();
        } // end constructor DetailsModel

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// The number of matching order details.
        /// </summary>
        public int MatchingOrderDetailsCount { get; set; }
        /// <summary>
        /// The subtotal of an order.
        /// </summary>
        public decimal Subtotal { get; set; }
        /// <summary>
        /// A matching <see cref="Order"/> object.
        /// </summary>
        public Order MatchingOrder { get; set; }
        /// <summary>
        /// An order status list.
        /// </summary>
        public List<OrderStatusListItem> OrderStatusList { get; }
        /// <summary>
        /// A matching order detail list.
        /// </summary>
        public IList<OrderDetail> MatchingOrderDetailList { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                MatchingOrder = await _context.Orders.FindAsync(id);

                if (MatchingOrder == null)
                    return NotFound($"Error! A valid ID of an order is required.");

                var matchingOrderDetails = _context.OrderDetails
                    .Include(orderDetail => orderDetail.Order)
                    .Where(orderDetail => orderDetail.OrderId == id);
                MatchingOrderDetailList = await matchingOrderDetails
                    .OrderBy(orderDetail => orderDetail.BouquetName)
                    .ToListAsync();
                MatchingOrderDetailsCount = await matchingOrderDetails.CountAsync();

                if (MatchingOrderDetailsCount > 0)
                    foreach (var item in MatchingOrderDetailList)
                        Subtotal += DecimalHelper.ToPriceFormat(item.Price) * item.Quantity;
                else
                    StatusMessage = string.Format("Error! Failed to get details of the order with ID \"{0}\". You may try again.", id);

                if (MatchingOrder.OrderStatusId == 1 && new TimeSpan(DateTimeOffset.UtcNow.Ticks - MatchingOrder.OrderTime.ToLocalTime().Ticks).TotalMinutes > 15)
                {
                    MatchingOrder.OrderStatusId = 2;
                    _context.Attach(MatchingOrder).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Specified order is failed due to timeout of confirming delivery info.");
                        StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because the customer has failed to confirm delivery info in 15 minutes after creating an order.", MatchingOrder.ID);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        if (!await _context.Orders.AnyAsync(order => order.ID == MatchingOrder.ID))
                        {
                            _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of confirming delivery info has been detected.");
                            StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of confirming delivery info has been detected. You may try again.", MatchingOrder.ID);
                            return RedirectToPage("/Orders/Index", new { area = "Admin" });
                        }
                        else
                        {
                            _logger.LogError(e, "Error! Failed to set status of specified order to \"Failed\".");
                            StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because the customer has failed to confirm delivery info in 15 minutes after creating an order. However, the system has failed to set the status of the order to \"Failed\". You may try again.", MatchingOrder.ID);
                        }
                    }

                    return Page();
                }

                if (MatchingOrder.OrderStatusId == 4 && new TimeSpan(DateTimeOffset.Now.Ticks - MatchingOrder.OrderTime.ToLocalTime().Ticks).TotalMinutes > 30)
                {
                    MatchingOrder.OrderStatusId = 2;
                    _context.Attach(MatchingOrder).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Specified order is failed due to timeout of confirming payment.");
                        StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because the customer has failed to confirm payment in 30 minutes after creating an order.", MatchingOrder.ID);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        if (!await _context.Orders.AnyAsync(order => order.ID == MatchingOrder.ID))
                        {
                            _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of confirming payment has been detected.");
                            StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of confirming payment has been detected. You may try again.", MatchingOrder.ID);
                            return RedirectToPage("/Orders/Index", new { area = "Admin" });
                        }
                        else
                        {
                            _logger.LogError(e, "Error! Failed to set status of specified order to \"Failed\", although timeout of confirming payment has been detected.");
                            StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because the customer has failed to confirm payment in 30 minutes after creating an order. However, the system has failed to set the status of the order to \"Failed\". You may try again.", MatchingOrder.ID);
                        }
                    }

                    return Page();
                }

                if (MatchingOrder.OrderStatusId == 8)
                {
                    if (MatchingOrder.DeliveryTime != null)
                    {
                        if (new TimeSpan(DateTimeOffset.Now.Ticks - ((DateTimeOffset)MatchingOrder.DeliveryTime).ToLocalTime().Ticks).TotalHours > 24)
                        {
                            MatchingOrder.OrderStatusId = 9;
                            MatchingOrder.CompletionTime = DateTimeOffset.Now;
                            _context.Attach(MatchingOrder).State = EntityState.Modified;

                            if (Subtotal > 0)
                            {
                                var newSalesRecord = new SalesRecord
                                {
                                    SalesAmount = DecimalHelper.ToPriceFormat(Subtotal),
                                    CreationTime = DateTimeOffset.Now
                                };

                                _context.SalesRecords.Add(newSalesRecord);
                            }

                            try
                            {
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Order status has been updated successfully due to timeout of asking for refund.");
                                StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because the customer has not asked for a refund in 24 hours after delivery.", MatchingOrder.ID);
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == MatchingOrder.ID))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of asking for a refund has been detected. You may try again.", MatchingOrder.ID);
                                    return RedirectToPage("/Orders/Index", new { area = "Admin" });
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because the customer has not asked for a refund in 24 hours after delivery. However, the system has failed to set the status of the order to \"Completed\". You may try again.", MatchingOrder.ID);
                                }
                            }
                        }
                    }
                    else
                        StatusMessage = "Error! Missing delivery time has been detected. Please check orders delivered.";
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync
    } // end class DetailsModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Orders
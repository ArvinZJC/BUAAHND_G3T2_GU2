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

namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.Orders
{
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

        public bool IsEmailConfirmed { get; set; }

        public int? CurrentOrderId { get; set; }

        public int MatchingOrderDetailsCount { get; set; }

        public decimal Subtotal { get; set; }

        public Order MatchingOrder { get; set; }

        public List<OrderStatusListItem> OrderStatusList { get; }

        public IList<OrderDetail> MatchingOrderDetailList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            CurrentOrderId = id;

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                MatchingOrder = await _context.Orders.FindAsync(id);

                if (MatchingOrder == null || MatchingOrder.UserId != user.Id)
                    return NotFound();

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

                if (MatchingOrder.OrderStatusId == 1 && new TimeSpan(DateTimeOffset.Now.Ticks - MatchingOrder.OrderTime.ToLocalTime().Ticks).TotalMinutes > 15)
                {
                    MatchingOrder.OrderStatusId = 2;
                    _context.Attach(MatchingOrder).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Specified order is failed due to timeout of confirming delivery info.");
                        StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because you have failed to confirm delivery info in 15 minutes after creating an order.", MatchingOrder.ID);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        if (!await _context.Orders.AnyAsync(order => order.ID == MatchingOrder.ID))
                        {
                            _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of confirming delivery info has been detected.");
                            StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of confirming delivery info has been detected. You may try again.", MatchingOrder.ID);
                            return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                        }
                        else
                        {
                            _logger.LogError(e, "Error! Failed to set status of specified order to \"Failed\".");
                            StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because you have failed to confirm delivery info in 15 minutes after creating an order. However, the system has failed to set the status of the order to \"Failed\". You may try again.", MatchingOrder.ID);
                        } // end if...else
                    } // end try...catch

                    return Page();
                } // end if

                if (await HasTimeoutAsync(MatchingOrder) == -1)
                    return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                else if (await HasTimeoutAsync(MatchingOrder) == 1)
                    return Page();

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
                            } // end if

                            try
                            {
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Order status has been updated successfully due to timeout of asking for refund.");
                                StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because you have not asked for a refund in 24 hours after delivery.", MatchingOrder.ID);
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == MatchingOrder.ID))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of asking for a refund has been detected. You may try again.", MatchingOrder.ID);
                                    return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because you have not asked for a refund in 24 hours after delivery. However, the system has failed to set the status of the order to \"Completed\". You may try again.", MatchingOrder.ID);
                                } // end if...else
                            } // end try...catch
                        } // end if
                    }
                    else
                        StatusMessage = "Error! Missing delivery time has been detected. Please check your orders delivered.";
                } // end if
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostPayBillAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                var orderToUpdate = await _context.Orders.FindAsync(id);

                if (orderToUpdate == null || orderToUpdate.UserId != user.Id)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 4)
                {
                    if (await HasTimeoutAsync(orderToUpdate) == -1 || await HasTimeoutAsync(orderToUpdate) == 1)
                        return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });

                    var matchingOrderDetails = _context.OrderDetails
                        .Include(orderDetail => orderDetail.Order)
                        .Where(orderDetail => orderDetail.OrderId == id);
                    MatchingOrderDetailList = await matchingOrderDetails
                        .OrderBy(orderDetail => orderDetail.BouquetName)
                        .ToListAsync();
                    MatchingOrderDetailsCount = await matchingOrderDetails.CountAsync();

                    if (MatchingOrderDetailsCount > 0)
                    {
                        foreach (var item in MatchingOrderDetailList)
                        {
                            var matchingBouquet = await _context.Bouquets.FindAsync(item.BouquetId);

                            if (matchingBouquet != null)
                            {
                                if (item.Quantity > 0 && matchingBouquet.Stocks >= item.Quantity)
                                {
                                    matchingBouquet.Sales += item.Quantity;
                                    matchingBouquet.Stocks -= item.Quantity;
                                    _context.Attach(matchingBouquet).State = EntityState.Modified;
                                }
                                else if (matchingBouquet.Stocks < item.Quantity)
                                {
                                    orderToUpdate.OrderStatusId = 2;
                                    _context.Attach(orderToUpdate).State = EntityState.Modified;

                                    try
                                    {
                                        await _context.SaveChangesAsync();

                                        if (item.Quantity > 0)
                                        {
                                            _logger.LogInformation("Specified order is failed due to insufficient stocks.");
                                            StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because insufficient stocks have been detected.", id);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("Specified order is failed due to invalid quantity.");
                                            StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because invalid quantity has been detected.", id);
                                        } // end if...else
                                    }
                                    catch (DbUpdateConcurrencyException e)
                                    {
                                        _logger.LogError(e, "Error! Failed to set status of specified order to \"Failed\", although a problem about quantity has occured.");
                                        StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because a problem about quantity has occured. However, the system has failed to set the status of the order to \"Failed\". You may try again.", id);
                                        return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                                    } // end try...catch

                                    return RedirectToPage("/Account/Manage/Orders/Details", new { area = "Identity", id });
                                } // end if...else
                            } // end if
                        } // end foreach

                        try
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Sales and stocks of specified bouquet have been updated successfully.");
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            _logger.LogError(e, "Error! Failed to update sales and stocks of specified bouquet.");

                            StatusMessage = "Error! Failed to update the sales and stocks of the specified bouquet. The bill will not be paid. You may try again.";

                            return RedirectToPage("/Account/Manage/Orders/Details", new { area = "Identity", id });
                        } // end try...catch
                    }
                    else
                    {
                        orderToUpdate.OrderStatusId = 2;
                        _context.Attach(orderToUpdate).State = EntityState.Modified;

                        try
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogError("Error! Specified order is failed because order details cannot be found during payment.");
                            StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because details of the order cannot be found during the payment.", id);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            _logger.LogError(e, "Error! Failed to set status of specified order to \"Failed\", although order details cannot be found during payment.");
                            StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because details of the order cannot be found during the payment. However, the system has failed to set the status of the order to \"Failed\". You may try again.", id);
                        } // end try...catch
                        
                        return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                    } // end if...else

                    orderToUpdate.OrderStatusId = 5;
                    orderToUpdate.PaymentTime = DateTimeOffset.Now;
                    _context.Attach(orderToUpdate).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User confirmed payment of specified order successfully.");
                        StatusMessage = string.Format("Payment of the order with ID \"{0}\" has been confirmed.", orderToUpdate.ID);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        if (!await _context.Orders.AnyAsync(order => order.ID == orderToUpdate.ID))
                        {
                            _logger.LogError(e, "Error! Failed to find specified order.");
                            StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\". The bill will not be paid. You may try again.", orderToUpdate.ID);
                            return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                        }
                        else
                        {
                            _logger.LogError(e, "Error! Failed to set status of specified order to \"Awaiting fulfillment\".");
                            StatusMessage = string.Format("Error! Failed to set the status of the order with ID \"{0}\" to \"Awaiting fulfillment\". The bill will not be paid. You may try again.", orderToUpdate.ID);
                        } // end if...else
                    } // end try...catch
                }
                else
                    StatusMessage = "Error! For the current order status, you cannot pay the bill.";
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Account/Manage/Orders/Details", new { area = "Identity", id });
        } // end method OnPostPayBillAsync

        private async Task<int> HasTimeoutAsync(Order matchingOrder)
        {
            if (matchingOrder.OrderStatusId == 4 && new TimeSpan(DateTimeOffset.Now.Ticks - matchingOrder.OrderTime.ToLocalTime().Ticks).TotalMinutes > 30)
            {
                matchingOrder.OrderStatusId = 2;
                _context.Attach(matchingOrder).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified order is failed due to timeout of confirming payment.");
                    StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because you have failed to confirm payment in 30 minutes after creating an order.", matchingOrder.ID);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!await _context.Orders.AnyAsync(order => order.ID == matchingOrder.ID))
                    {
                        _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of confirming payment has been detected.");
                        StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of confirming payment has been detected. You may try again.", matchingOrder.ID);
                        return -1;
                    }
                    else
                    {
                        _logger.LogError(e, "Error! Failed to set status of specified order to \"Failed\", although timeout of confirming payment has been detected.");
                        StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because you have failed to confirm payment in 30 minutes after creating an order. However, the system has failed to set the status of the order to \"Failed\". You may try again.", matchingOrder.ID);
                    } // end if...else
                } // end try...catch

                return 1;
            } // end if

            return 0;
        } // end method HasTimeoutAsync
    } // end class DetailsModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.Orders
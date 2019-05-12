// csharp file that contains actions of the order list page for a customer

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
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="IndexModel"/> contains actions of the order list page for a customer.
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

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// The number of a user's orders.
        /// </summary>
        public int UserOrdersCount { get; set; }
        /// <summary>
        /// The number of matching orders.
        /// </summary>
        public int MatchingOrdersCount { get; set; }
        /// <summary>
        /// ID of the current order status.
        /// </summary>
        public int CurrentOrderStatusId { get; set; }
        /// <summary>
        /// The index of the current page.
        /// </summary>
        public int? CurrentPageIndex { get; set; }
        /// <summary>
        /// An order status list.
        /// </summary>
        public List<OrderStatusListItem> OrderStatusList { get; }
        /// <summary>
        /// A paginated list containing matching orders.
        /// </summary>
        public PaginatedList<Order> MatchingOrderList { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? orderStatusId, int? pageIndex)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                IQueryable<Order> matchingOrders = from order in _context.Orders
                                                   select order;

                matchingOrders = matchingOrders
                    .Include(order => order.User)
                    .Where(order => order.UserId == user.Id);
                UserOrdersCount = await matchingOrders.CountAsync();

                orderStatusId = orderStatusId != 0 ? orderStatusId : null;
                CurrentOrderStatusId = orderStatusId ?? 0;

                if (UserOrdersCount > 0)
                {
                    var isChanged = false;

                    foreach (var item in matchingOrders)
                    {
                        if (item.OrderStatusId == 1 && new TimeSpan(DateTimeOffset.Now.Ticks - item.OrderTime.ToLocalTime().Ticks).TotalMinutes > 15)
                        {
                            item.OrderStatusId = 2;
                            _context.Attach(item).State = EntityState.Modified;
                            isChanged = true;
                        } // end if

                        if (item.OrderStatusId == 4 && new TimeSpan(DateTimeOffset.Now.Ticks - item.OrderTime.ToLocalTime().Ticks).TotalMinutes > 30)
                        {
                            item.OrderStatusId = 2;
                            _context.Attach(item).State = EntityState.Modified;
                            isChanged = true;
                        } // end if

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
                                        } // end if
                                    } // end if

                                    item.OrderStatusId = 9;
                                    item.CompletionTime = DateTimeOffset.Now;
                                    _context.Attach(item).State = EntityState.Modified;
                                    isChanged = true;
                                } // end if
                            }
                            else
                                StatusMessage = "Error! Missing delivery time has been detected. Please check your orders delivered.";
                        } // end if
                    } // end foreach

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
                        } // end try...catch
                    } // end if

                    if (orderStatusId != null)
                    {
                        matchingOrders = matchingOrders.Where(order => order.OrderStatusId == orderStatusId);
                        MatchingOrdersCount = await matchingOrders.CountAsync();
                    } // end if

                    matchingOrders = matchingOrders.OrderByDescending(order => order.OrderTime).ThenByDescending(order => order.ID);
                }

                int pageSize = 8; // the code of pagination in the order list page needs improving after modifying the page size

                CurrentPageIndex = (pageIndex == null || pageIndex <= 0) ? 1 : pageIndex;

                MatchingOrderList = await PaginatedList<Order>.CreateAsync(
                    matchingOrders.AsNoTracking(),
                    (int)CurrentPageIndex,
                    pageSize);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostConfirmReceiptAsync(int? id, string returnUrl = null)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                returnUrl = returnUrl ?? Url.Content("~/Identity/Account/Manage/Orders/Index");

                var orderToUpdate = await _context.Orders.FindAsync(id);

                if (orderToUpdate == null || orderToUpdate.UserId != userId)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 8)
                {
                    if (orderToUpdate.DeliveryTime != null)
                    {
                        var matchingOrderDetails = _context.OrderDetails
                                    .Include(orderDetail => orderDetail.Order)
                                    .Where(orderDetail => orderDetail.OrderId == orderToUpdate.ID);
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
                            } // end if
                        } // end if

                        orderToUpdate.OrderStatusId = 9;
                        orderToUpdate.CompletionTime = DateTimeOffset.Now;
                        _context.Attach(orderToUpdate).State = EntityState.Modified;

                        if (new TimeSpan(DateTimeOffset.Now.Ticks - ((DateTimeOffset)orderToUpdate.DeliveryTime).ToLocalTime().Ticks).TotalHours > 24)
                        {
                            try
                            {
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Order status has been updated successfully due to timeout of asking for refund.");
                                StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because you have not asked for a refund in 24 hours after delivery.", id);
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == id))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of asking for a refund has been detected. You may try again.", id);
                                    return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because you have not asked for a refund in 24 hours after delivery. However, the system has failed to set the status of the order to \"Completed\". You may try again.", id);
                                } // end if...else
                            } // end try...catch
                        }
                        else
                        {
                            try
                            {
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("User confirmed receipt of specified order successfully.");
                                StatusMessage = string.Format("The receipt of the order with ID \"{0}\" has been confirmed.", id);
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == orderToUpdate.ID))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\". You may try again.", id);
                                    returnUrl = Url.Content("~/Identity/Account/Manage/Orders/Index");
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to confirm receipt of specified order.");
                                    StatusMessage = string.Format("Error! Failed to confirm the receipt of the order with ID \"{0}\". You may try again.", id);
                                } // end if...else
                            } // end try...catch
                        } // end if...else
                    }
                    else
                        StatusMessage = "Error! Missing delivery time has been detected. Please check your orders delivered.";
                }
                else
                    StatusMessage = string.Format("Error! For the current order status, you cannot confirm the receipt of the order with ID \"{0}\".", id);
            }
            else
            {
                returnUrl = Url.Content("~/Identity/Account/Manage/Orders/Index");
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            } // end if...else

            return LocalRedirect(returnUrl);
        } // end method OnPostConfirmReceiptAsync

        public async Task<IActionResult> OnPostCancelAsync(int? id, string returnUrl = null)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                returnUrl = returnUrl ?? Url.Content("~/Identity/Account/Manage/Orders/Index");

                var orderToUpdate = await _context.Orders.FindAsync(id);

                if (orderToUpdate == null || orderToUpdate.UserId != user.Id)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 1 || orderToUpdate.OrderStatusId == 4)
                {
                    orderToUpdate.OrderStatusId = 3;
                    _context.Attach(orderToUpdate).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User cancelled specified order successfully.");
                        StatusMessage = string.Format("The order with ID \"{0}\" has been cancelled.", id);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        if (!await _context.Orders.AnyAsync(order => order.ID == orderToUpdate.ID))
                        {
                            _logger.LogError(e, "Error! Failed to find specified order.");
                            StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\". You may try again.", id);
                            returnUrl = Url.Content("~/Identity/Account/Manage/Orders/Index");
                        }
                        else
                        {
                            _logger.LogError(e, "Error! Failed to cancel specified order.");
                            StatusMessage = string.Format("Error! Failed to cancel the order with ID \"{0}\". You may try again.", id);
                        } // end if...else
                    } // end try...catch
                }
                else
                    StatusMessage = string.Format("Error! For the current order status, you cannot cancel the order with ID \"{0}\".", id);
            }
            else
            {
                returnUrl = Url.Content("~/Identity/Account/Manage/Orders/Index");
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            } // end if...else

            return LocalRedirect(returnUrl);
        } // end method OnPostCancelAsync

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                var orderToDelete = await _context.Orders.FindAsync(id);

                if (orderToDelete == null || orderToDelete.UserId != user.Id)
                    return NotFound();

                if (orderToDelete.OrderStatusId == 2 || orderToDelete.OrderStatusId == 3 || orderToDelete.OrderStatusId == 9 || orderToDelete.OrderStatusId == 11)
                {
                    try
                    {
                        _context.Orders.Remove(orderToDelete);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User deleted specified order succesfully.");
                        StatusMessage = string.Format("The order with ID \"{0}\" has been deleted.", id);
                    }
                    catch (DbUpdateException e)
                    {
                        _logger.LogError(e, "Error! Failed to delete specified order.");
                        StatusMessage = string.Format("Error! Failed to delete the order with ID \"{0}\". You may try again.", id);
                    } // end try...catch
                }
                else
                    StatusMessage = string.Format("Error! For the current order status, you cannot delete the order with ID \"{0}\".", id);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
        } // end method OnPostDeleteAsync

        public async Task<IActionResult> OnPostRefundAsync(int? id, string returnUrl = null)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                returnUrl = returnUrl ?? Url.Content("~/Identity/Account/Manage/Orders/Index");

                var orderToUpdate = await _context.Orders.FindAsync(id);

                if (orderToUpdate == null || orderToUpdate.UserId != user.Id)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 8)
                {
                    if (orderToUpdate.DeliveryTime != null)
                    {
                        if (new TimeSpan(DateTimeOffset.Now.Ticks - ((DateTimeOffset)orderToUpdate.DeliveryTime).ToLocalTime().Ticks).TotalHours > 24)
                        {
                            var matchingOrderDetails = _context.OrderDetails
                                    .Include(orderDetail => orderDetail.Order)
                                    .Where(orderDetail => orderDetail.OrderId == orderToUpdate.ID);
                            var subtotal = 0M;

                            if (await matchingOrderDetails.CountAsync() > 0)
                            {
                                foreach (var orderDetail in await matchingOrderDetails.ToListAsync())
                                    subtotal += orderDetail.Price * orderDetail.Quantity;

                                if (subtotal > 0)
                                {
                                    var newSalesRecord = new SalesRecord
                                    {
                                        SalesAmount = DecimalHelper.ToPriceFormat(subtotal),
                                        CreationTime = DateTimeOffset.Now
                                    };

                                    _context.SalesRecords.Add(newSalesRecord);
                                } // end if
                            } // end if

                            orderToUpdate.OrderStatusId = 9;
                            orderToUpdate.CompletionTime = DateTimeOffset.Now;
                            _context.Attach(orderToUpdate).State = EntityState.Modified;

                            try
                            {
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Order status has been updated successfully due to timeout of asking for refund.");
                                StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because you have not asked for a refund in 24 hours after delivery.", id);
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == id))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of asking for a refund has been detected. You may try again.", id);
                                    return RedirectToPage("/Account/Manage/Orders/Index", new { area = "Identity" });
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to update order status, although timeout of asking for refund has been detected.");
                                    StatusMessage = string.Format("Error! The order with ID \"{0}\" is automatically completed because you have not asked for a refund in 24 hours after delivery. However, the system has failed to set the status of the order to \"Completed\". You may try again.", id);
                                } // end if...else
                            } // end try...catch
                        }
                        else
                        {
                            orderToUpdate.OrderStatusId = 10;
                            _context.Attach(orderToUpdate).State = EntityState.Modified;

                            try
                            {
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("User asked for refund for specified order successfully.");
                                StatusMessage = string.Format("You have asked for a refund for the order with ID \"{0}\". We are sorry for any dissatisfaction. Please await processed results.", id);
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == orderToUpdate.ID))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\". You may try again.", id);
                                    returnUrl = Url.Content("~/Identity/Account/Manage/Orders/Index");
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to ask for refund for specified order.");
                                    StatusMessage = string.Format("Error! Failed to ask for a refund for the order with ID \"{0}\". You may try again.", id);
                                } // end if...else
                            } // end try...catch
                        } // end if...else
                    }
                    else
                        StatusMessage = "Error! Missing delivery time has been detected. Please check your orders delivered.";
                }
                else
                    StatusMessage = string.Format("Error! For the current order status, you cannot ask for a refund for the order with ID \"{0}\".", id);
            }
            else
            {
                returnUrl = Url.Content("~/Identity/Account/Manage/Orders/Index");
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            } // end if...else

            return LocalRedirect(returnUrl);
        } // end method OnPostRefundAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Identity.Pages.Account.Manage.Orders
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
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Orders
{
    public class StatusModel : PageModel
    {
        private readonly OrderStatusListItem _orderStatusListItem;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StatusModel> _logger;

        public StatusModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StatusModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;

            _orderStatusListItem = new OrderStatusListItem();
        } // end constructor DetailsModel

        public bool IsEmailConfirmed { get; set; }

        public List<OrderStatusListItem> OrderStatusList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            public int OrderStatusId { get; set; }
        } // end class InputModel

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
                var orderToUpdate = await _context.Orders.FindAsync(id);

                if (orderToUpdate == null)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 5
                    || orderToUpdate.OrderStatusId == 6
                    || orderToUpdate.OrderStatusId == 7
                    || orderToUpdate.OrderStatusId == 10)
                {
                    Input = new InputModel
                    {
                        Id = orderToUpdate.ID,
                        OrderStatusId = orderToUpdate.OrderStatusId
                    };
                    OrderStatusList = new List<OrderStatusListItem>();

                    foreach (var item in _orderStatusListItem.GetOrderStatusList())
                    {
                        switch (orderToUpdate.OrderStatusId)
                        {
                            case 5:
                                if (item.ID == 5 || item.ID == 6 || item.ID == 7 || item.ID == 8)
                                    OrderStatusList.Add(item);
                                break;

                            case 6:
                                if (item.ID == 6 || item.ID == 7 || item.ID == 8)
                                    OrderStatusList.Add(item);
                                break;

                            case 7:
                                if (item.ID == 7 || item.ID == 8)
                                    OrderStatusList.Add(item);
                                break;

                            case 10:
                                if (item.ID == 10 || item.ID == 11)
                                    OrderStatusList.Add(item);
                                break;
                        } // end switch-case
                    } // end foreach
                }
                else
                {
                    StatusMessage = string.Format("Error! For the current status of the order with ID \"{0}\", you cannot change order status.", id);
                    return RedirectToPage("/Orders/Details", new { area = "Admin", id });
                } // end if...else
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                var orderToUpdate = await _context.Orders.FindAsync(Input.Id);

                if (orderToUpdate == null)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 5
                    || orderToUpdate.OrderStatusId == 6
                    || orderToUpdate.OrderStatusId == 7
                    || orderToUpdate.OrderStatusId == 10)
                {
                    if (ModelState.IsValid
                        && _orderStatusListItem.IsValidId(Input.OrderStatusId)
                        && orderToUpdate.OrderStatusId != Input.OrderStatusId)
                    {
                        switch (orderToUpdate.OrderStatusId)
                        {
                            case 5:
                                if (Input.OrderStatusId != 6
                                    && Input.OrderStatusId != 7
                                    && Input.OrderStatusId != 8
                                    && Input.OrderStatusId != 10)
                                    return RedirectToPage("/Orders/Details", new { area = "Admin", Input.Id });
                                break;

                            case 6:
                                if (Input.OrderStatusId != 7
                                    && Input.OrderStatusId != 8
                                    && Input.OrderStatusId != 10)
                                    return RedirectToPage("/Orders/Details", new { area = "Admin", Input.Id });
                                break;

                            case 7:
                                if (Input.OrderStatusId != 8
                                    && Input.OrderStatusId != 10)
                                    return RedirectToPage("/Orders/Details", new { area = "Admin", Input.Id });
                                break;

                            case 10:
                                if (Input.OrderStatusId != 11)
                                    return RedirectToPage("/Orders/Details", new { area = "Admin", Input.Id });
                                break;
                        } // end switch-case

                        if (await TryUpdateModelAsync(
                                orderToUpdate,
                                "Input",
                                order => order.OrderStatusId))
                        {
                            if (Input.OrderStatusId == 8)
                            {
                                orderToUpdate.DeliveryTime = DateTimeOffset.Now;
                                _context.Attach(orderToUpdate).State = EntityState.Modified;
                            } // end if

                            if (Input.OrderStatusId == 11)
                            {
                                var matchingOrderDetails = _context.OrderDetails
                                    .Include(orderDetail => orderDetail.Order)
                                    .Where(orderDetail => orderDetail.OrderId == Input.Id);

                                if (await matchingOrderDetails.CountAsync() > 0)
                                {
                                    orderToUpdate.CompletionTime = DateTimeOffset.Now;
                                    _context.Attach(orderToUpdate).State = EntityState.Modified;

                                    foreach (var item in matchingOrderDetails)
                                    {
                                        var matchingBouquet = await _context.Bouquets.FindAsync(item.BouquetId);

                                        if (matchingBouquet != null)
                                        {
                                            if (item.Quantity > 0)
                                            {
                                                if (matchingBouquet.Sales - item.Quantity >= 0)
                                                    matchingBouquet.Sales -= item.Quantity;
                                                else
                                                    matchingBouquet.Sales = 0;
                                                
                                                _context.Attach(matchingBouquet).State = EntityState.Modified;
                                            } // end if
                                        } // end if
                                    } // end foreach
                                }
                                else
                                {
                                    orderToUpdate.OrderStatusId = 10;
                                    _context.Attach(orderToUpdate).State = EntityState.Modified;
                                } // end if...else
                            } // end if

                            try
                            {
                                await _context.SaveChangesAsync();

                                if (Input.OrderStatusId == 11 && orderToUpdate.CompletionTime == null)
                                {
                                    _logger.LogError("Error! Failed to get details of specified order.");
                                    StatusMessage = string.Format("Error! Failed to get details of the order with ID \"{0}\". You may try again.", Input.Id);
                                }
                                else
                                {
                                    _logger.LogInformation("Status of specified order has been changed successfully.");
                                    StatusMessage = string.Format("The status of the order with ID \"{0}\" has been changed.", Input.Id);
                                } // end if...else
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == Input.Id))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\". You may try again.", Input.Id);
                                    return RedirectToPage("/Orders/Index", new { area = "Admin" });
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to change status of specified order.");
                                    StatusMessage = string.Format("Error! Failed to change the status of the order with ID \"{0}\". You may try again.", Input.Id);
                                } // end if...else
                            } // end try...catch
                        }
                        else
                        {
                            _logger.LogError("Error! Failed to change status of specified order.");
                            StatusMessage = string.Format("Error! Failed to change status of the order with ID \"{0}\". You may try again.", Input.Id);
                        } // end if...else
                    } // end if
                }
                else
                    StatusMessage = string.Format("Error! For the current status of the order with ID \"{0}\", you cannot change order status.", Input.Id);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Orders/Details", new { area = "Admin", Input.Id });
        } // end method OnPostAsync
    } // end class StatusModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Orders
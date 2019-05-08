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
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Pages
{
    [Authorize(Roles = "Customer")]
    public class CartModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public CartModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor CartModel

        public bool IsEmailConfirmed { get; set; }

        public int MatchingCartDetailsCount { get; set; }

        public IList<CartDetail> MatchingCartDetailList { get; set; }

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
                var matchingCartDetails = _context.CartDetails
                    .Include(cartDetail => cartDetail.Bouquet)
                    .Include(cartDetail => cartDetail.User)
                    .Where(cartDetail => cartDetail.UserId == user.Id);
                MatchingCartDetailsCount = await matchingCartDetails.CountAsync();

                if (MatchingCartDetailsCount > 0)
                {
                    MatchingCartDetailList = await matchingCartDetails
                        .OrderBy(cartDetail => cartDetail.Bouquet.Name)
                        .ToListAsync();

                    // the relevant back-end code in this class and code in the cart page need updating after modifying the value in the condition
                    if (MatchingCartDetailsCount == 10)
                        StatusMessage = "Warning! The cart already contains 10 different bouquets. Remove some if you want to add more.";

                    // the relevant back-end code in this class and code in the cart page need updating after modifying the value in the condition
                    if (MatchingCartDetailsCount > 10)
                        StatusMessage = string.Format("Error! The cart contains {0} different bouquets which exceed the limit. Only 10 of them are displayed. Please remove some, or you cannot check out.", MatchingCartDetailsCount);
                } // end if
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            var returnUrl = Url.Content("~/Cart");

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            if (user.EmailConfirmed)
            {
                var bouquetIdValue = Request.Form["bouquetId"];
                var quantityValue = Request.Form["quantity"];

                if (int.TryParse(bouquetIdValue, out var bouquetId) && int.TryParse(quantityValue, out var quantity))
                {
                    var cartDetailToUpdate = await _context.CartDetails
                        .Include(cartDetail => cartDetail.Bouquet)
                        .Include(cartDetail => cartDetail.User)
                        .Where(cartDetail => cartDetail.UserId == user.Id)
                        .FirstOrDefaultAsync(cartDetail => cartDetail.BouquetId == bouquetId);

                    if (cartDetailToUpdate != null && quantity > 0 && quantity <= cartDetailToUpdate.Bouquet.Stocks)
                    {
                        cartDetailToUpdate.Quantity = quantity;
                        _context.Attach(cartDetailToUpdate).State = EntityState.Modified;

                        try
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("User update quantity of bouquet in cart successfully.");
                            StatusMessage = string.Format("The quantity of the bouquet with the name \"{0}\" has been updated.", cartDetailToUpdate.Bouquet.Name);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            if (!await _context.CartDetails.AnyAsync(cartDetail => cartDetail.ID == cartDetailToUpdate.ID))
                            {
                                _logger.LogError(e, "Error! Failed to find specified bouquet in cart.");
                                StatusMessage = "Error! Failed to find the specified bouquet in the cart. You may try again.";
                            }
                            else
                            {
                                _logger.LogError(e, "Error! Failed to update quantity of bouquet in cart.");
                                StatusMessage = string.Format("Error! Failed to update the quantity of the bouquet with the name \"{0}\" in the cart. You may try again.", cartDetailToUpdate.Bouquet.Name);
                            } // end if...else
                        } // end try...catch
                    }
                    else
                        StatusMessage = "Error! Failed to update the quantity of the specified bouquet in the cart. You may try again.";
                }
                else
                    StatusMessage = "Error! Failed to get necessary info to update the cart. You may try again.";
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return LocalRedirect(returnUrl);
        } // end method OnPostAsync

        public async Task<IActionResult> OnPostAddAsync(int? bouquetId, int? quantity, string returnUrl = null)
        {
            if (bouquetId == null)
                return NotFound();

            returnUrl = returnUrl ?? Url.Content("~/Cart"); // if there is no specified return URL, set the cart page to the return URL

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            if (user.EmailConfirmed)
            {
                var matchingCartDetails = _context.CartDetails
                    .Include(cartDetail => cartDetail.Bouquet)
                    .Include(cartDetail => cartDetail.User)
                    .Where(cartDetail => cartDetail.UserId == user.Id);
                MatchingCartDetailsCount = await matchingCartDetails.CountAsync();

                // the relevant back-end code in this class and code in the cart page need updating after modifying the value in the condition
                if (MatchingCartDetailsCount == 10)
                    StatusMessage = "Error! You already have 10 different bouquets. Remove some if you want to add more.";
                // the relevant back-end code in this class and code in the cart page need updating after modifying the value in the condition
                else if (MatchingCartDetailsCount > 10)
                    StatusMessage = string.Format("Error! You have {0} different bouquets which exceed the limit. Please remove some, or you cannot check out.", MatchingCartDetailsCount);
                else
                {
                    if (quantity > 0)
                    {
                        var existingCartDetail = await matchingCartDetails.FirstOrDefaultAsync(cartDetail => cartDetail.BouquetId == bouquetId);

                        if (existingCartDetail != null)
                        {
                            var actualQuantity = existingCartDetail.Quantity + quantity;

                            if (actualQuantity <= existingCartDetail.Bouquet.Stocks)
                            {
                                existingCartDetail.Quantity = (int)actualQuantity;
                                _context.Attach(existingCartDetail).State = EntityState.Modified;

                                try
                                {
                                    await _context.SaveChangesAsync();
                                    _logger.LogInformation("User increased quantity of specified bouquet already in cart successfully.");

                                    if (actualQuantity > 1)
                                        StatusMessage = string.Format("{0} bouquets with the name \"{1}\" have been added to the cart.", actualQuantity, existingCartDetail.Bouquet.Name);
                                    else
                                        StatusMessage = string.Format("1 bouquet with the name \"{0}\" has been added to the cart.", existingCartDetail.Bouquet.Name);
                                }
                                catch (DbUpdateConcurrencyException e)
                                {
                                    if (!await _context.CartDetails.AnyAsync(cartDetail => cartDetail.ID == existingCartDetail.ID))
                                    {
                                        _logger.LogError(e, "Error! Failed to find specified bouquet in cart.");
                                        StatusMessage = "Error! Failed to find the specified bouquet in the cart. You may try again.";
                                    }
                                    else
                                    {
                                        _logger.LogError(e, "Error! Failed to increase quantity of specified bouquet already in cart.");
                                        StatusMessage = string.Format("Error! Failed to increase the quantity of the bouquet with the name \"{0}\" already in the cart. You may try again.", existingCartDetail.Bouquet.Name);
                                    } // end if...else
                                } // end try...catch
                            }
                            else
                                StatusMessage = string.Format("Error! The quantity of the bouquet with the name \"{0}\" is invalid. You may try again.", existingCartDetail.Bouquet.Name);
                        }
                        else
                        {
                            var bouquet = await _context.Bouquets.FindAsync(bouquetId);

                            if (bouquet == null)
                            {
                                StatusMessage = "Error! Failed to add the specified bouquet because it cannot be found. You may try again.";

                                return LocalRedirect(returnUrl);
                            } // end if

                            if (bouquet.Stocks > 0 && quantity <= bouquet.Stocks)
                            {
                                var newCartDetail = new CartDetail()
                                {
                                    UserId = user.Id,
                                    BouquetId = (int)bouquetId,
                                    Quantity = (int)quantity
                                };

                                try
                                {
                                    _context.CartDetails.Add(newCartDetail);
                                    await _context.SaveChangesAsync();
                                    _logger.LogInformation("User added specified bouquet with specified quantity to cart successfully.");

                                    if (newCartDetail.Quantity > 1)
                                        StatusMessage = string.Format("{0} bouquets with the name \"{1}\" have been added to the cart.", newCartDetail.Quantity, bouquet.Name);
                                    else
                                        StatusMessage = string.Format("1 bouquet with the name \"{0}\" has been added to the cart.", bouquet.Name);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, "Error! Failed to add specified bouquet to cart.");
                                    StatusMessage = string.Format("Error! Failed to add the bouquet with the name \"{0}\" to the cart. You may try again.", bouquet.Name);
                                } // end try...catch
                            }
                            else
                                StatusMessage = string.Format("Error! Stocks of the bouquet with the name \"{0}\" are insufficient. You may try again.", bouquet.Name);
                        } // end if...else
                    }
                    else
                        StatusMessage = "Error! The quantity of the specified bouquet is invalid. You may try again.";
                } // end nested if...else
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return LocalRedirect(returnUrl);
        } // end method OnPostAddAsync

        public async Task<IActionResult> OnPostRemoveAsync(int? bouquetId)
        {
            var returnUrl = Url.Content("~/Cart");

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            if (user.EmailConfirmed)
            {
                var matchingCartDetails = _context.CartDetails
                    .Include(cartDetail => cartDetail.Bouquet)
                    .Include(cartDetail => cartDetail.User)
                    .Where(cartDetail => cartDetail.UserId == user.Id);
                MatchingCartDetailsCount = await matchingCartDetails.CountAsync();

                if (MatchingCartDetailsCount == 0)
                    return NotFound();

                if (bouquetId != null)
                {
                    var cartDetailToRemove = await matchingCartDetails.FirstOrDefaultAsync(cartDetail => cartDetail.BouquetId == bouquetId);

                    if (cartDetailToRemove != null)
                    {
                        try
                        {
                            _context.CartDetails.Remove(cartDetailToRemove);
                            await _context.SaveChangesAsync();

                            if (cartDetailToRemove.Quantity > 1)
                            {
                                _logger.LogInformation("User removed specified bouquets from cart succesfully.");
                                StatusMessage = string.Format("The bouquets with the name \"{0}\" have been removed from the cart.", cartDetailToRemove.Bouquet.Name);
                            }
                            else
                            {
                                _logger.LogInformation("User removed specified bouquet from cart succesfully.");
                                StatusMessage = string.Format("The bouquet with the name \"{0}\" has been removed from the cart.", cartDetailToRemove.Bouquet.Name);
                            } // end if...else
                        }
                        catch (DbUpdateException e)
                        {
                            _logger.LogError(e, "Error! Failed to remove specified bouquet from cart.");
                            StatusMessage = string.Format("Error! Failed to remove the bouquet with the name \"{0}\" from the cart. You may try again.", cartDetailToRemove.Bouquet.Name);
                        } // end try...catch
                    }
                    else
                        StatusMessage = "Error! The specified bouquet is not in the cart. You may try again.";
                }
                else
                {
                    try
                    {
                        _context.CartDetails.RemoveRange(matchingCartDetails);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User removed all bouquets from cart succesfully.");
                        StatusMessage = "All bouquets have been removed from the cart.";
                    }
                    catch (DbUpdateException e)
                    {
                        _logger.LogError(e, "Error! Failed to remove all bouquets from cart.");
                        StatusMessage = "Error! Failed to remove all bouquets from the cart. You may try again.";
                    } // end try...catch
                } // end if...else
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return LocalRedirect(returnUrl);
        } // end method OnPostRemoveAsync

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            var returnUrl = Url.Content("~/Cart");

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            if (user.EmailConfirmed)
            {
                var matchingCartDetails = _context.CartDetails
                    .Include(cartDetail => cartDetail.Bouquet)
                    .Include(cartDetail => cartDetail.User)
                    .Where(cartDetail => cartDetail.UserId == user.Id);
                MatchingCartDetailsCount = await matchingCartDetails.CountAsync();

                if (MatchingCartDetailsCount == 0)
                    return NotFound();

                foreach (var item in matchingCartDetails)
                {
                    if (await _context.Bouquets.FindAsync(item.BouquetId) == null)
                    {
                        StatusMessage = "Error! At least 1 bouquet off the shelves. You may try again.";

                        return LocalRedirect(returnUrl);
                    } // end if

                    if (item.Bouquet.Stocks == 0 || item.Quantity > item.Bouquet.Stocks)
                    {
                        StatusMessage = string.Format("Error! Stocks of the bouquet with the name \"{0}\" are insufficient. You may try again.", item.Bouquet.Name);
                        return LocalRedirect(returnUrl);
                    } // end if
                } // end foreach

                var random = new Random();
                var newOrder = new Order()
                {
                    UserId = user.Id,
                    OrderStatusId = 1,
                    Postage = DecimalHelper.ToPriceFormat(random.Next(5, 16)), // set a random integer between 5 and 15 to the postage
                    OrderTime = DateTimeOffset.Now
                };

                try
                {
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User created order successfully.");
                }
                catch (Exception e)
                {
                    StatusMessage = "Error! Failed to create an order. You may try again.";

                    _logger.LogError(e, "Error! Failed to create order.");
                    return LocalRedirect(returnUrl);
                } // end try...catch

                foreach (var item in matchingCartDetails)
                {
                    var newOrderDetail = new OrderDetail()
                    {
                        OrderId = newOrder.ID,
                        BouquetId = item.BouquetId,
                        BouquetName = item.Bouquet.Name,
                        Price = DecimalHelper.ToPriceFormat(item.Bouquet.OriginalPrice * (1 - item.Bouquet.Discount)),
                        Quantity = item.Quantity
                    };

                    _context.OrderDetails.Add(newOrderDetail);
                } // end foreach

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User supplemented order details successfully.");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error! Failed to supplement order details.");
                    StatusMessage = string.Format("Error! Failed to supplement details of the order with ID \"{0}\". The incorrect order will be deleted automatically.", newOrder.ID);

                    var orderToDelete = await _context.Orders.FindAsync(newOrder.ID);

                    if (orderToDelete != null)
                    {
                        try
                        {
                            _context.Orders.Remove(await _context.Orders.FindAsync(newOrder.ID));
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("System deleted incorrect order succesfully.");
                        }
                        catch (DbUpdateException ex)
                        {
                            _logger.LogError(ex, "Error! Failed to delete incorrect order.");
                            StatusMessage = string.Format("Error! Failed to supplement details of the order with ID \"{0}\", and an exception occurred when the system tried to delete the incorrect order. You may try again.", newOrder.ID);
                        } // end try...catch
                    }
                    else
                        StatusMessage = string.Format("Error! Failed to supplement details of the order with ID \"{0}\", and the incorrect order cannot be found. You may try again.", newOrder.ID);

                    return LocalRedirect(returnUrl);
                } // end try...catch

                matchingCartDetails = _context.CartDetails
                    .Include(cartDetail => cartDetail.Bouquet)
                    .Include(cartDetail => cartDetail.User)
                    .Where(cartDetail => cartDetail.UserId == _userManager.GetUserId(User));
                MatchingCartDetailsCount = await matchingCartDetails.CountAsync();

                if (MatchingCartDetailsCount == 0)
                    return NotFound();

                try
                {
                    _context.CartDetails.RemoveRange(matchingCartDetails);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("System removed all bouquets from cart succesfully.");
                }
                catch (DbUpdateException e)
                {
                    _logger.LogError(e, "Error! Failed to remove all bouquets from cart.");
                } // end try...catch

                return RedirectToPage("/Account/Manage/Orders/DeliveryInfo", new { area = "Identity", id = newOrder.ID });
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return LocalRedirect(returnUrl);
        } // end method OnPostCheckOutAsync
    } // end class CartModel
} // end namespace NewEraFlowerStore.Pages
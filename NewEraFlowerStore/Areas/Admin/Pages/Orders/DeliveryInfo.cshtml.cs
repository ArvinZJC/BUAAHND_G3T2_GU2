#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
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
    public class DeliveryInfoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DeliveryInfoModel> _logger;

        public DeliveryInfoModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DeliveryInfoModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor DeliveryInfoModel

        public bool IsEmailConfirmed { get; set; }

        public int? CurrentOrderId { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter a first name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid first name.")] // the relevant regular expression and tooltips in the page for confirming delivery info need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 2 and at max 25 letters long, with only the 1st letter uppercase
             */
            [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid first name.")]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Please enter a last name.")]
            [DataType(DataType.Text)]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid last name.")] // the relevant regular expression and tooltips in the page for confirming delivery info need updating after modifying the length
            /* 
             * the minimum and maximum length here should be equal to the relevant attributes;
             * at least 2 and at max 25 letters long, with only the 1st letter uppercase
             */
            [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid last name.")]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Please enter a detailed address.")]
            [DataType(DataType.Text)]
            [StringLength(300, ErrorMessage = "Please enter a valid detailed address.")] // the relevant regular expression and tooltips in the page for confirming delivery info need updating after modifying the length
            [Display(Name = "Detailed address")]
            public string DetailedAddress { get; set; }

            [Required(ErrorMessage = "Please enter a zip/postal code.")]
            [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid zip/postal code.")]
            [Display(Name = "Zip/postal code")]
            public string ZipOrPostalCode { get; set; }

            [Required(ErrorMessage = "Please enter a phone number.")]
            [Phone(ErrorMessage = "Please enter a valid phone number.")]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        } // end class InputModel

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
                var orderToUpdate = await _context.Orders.FindAsync(id);

                if (orderToUpdate == null)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 1 || orderToUpdate.OrderStatusId == 4 || orderToUpdate.OrderStatusId == 5)
                {
                    if (await HasTimeoutAsync(orderToUpdate))
                        return RedirectToPage("/Orders/Index", new { area = "Admin" });

                    Input = new InputModel
                    {
                        FirstName = orderToUpdate.FirstName,
                        LastName = orderToUpdate.LastName,
                        DetailedAddress = orderToUpdate.DetailedAddress,
                        ZipOrPostalCode = orderToUpdate.ZipOrPostalCode,
                        PhoneNumber = orderToUpdate.PhoneNumber
                    };
                }
                else
                    StatusMessage = string.Format("Error! For the current status of the order with ID \"{0}\", you cannot edit delivery info.", id);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync(int? id)
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
                var orderToUpdate = await _context.Orders.FindAsync(id);

                if (orderToUpdate == null)
                    return NotFound();

                if (orderToUpdate.OrderStatusId == 1 || orderToUpdate.OrderStatusId == 4 || orderToUpdate.OrderStatusId == 5)
                {
                    if (await HasTimeoutAsync(orderToUpdate))
                        return RedirectToPage("/Orders/Details", new { area = "Admin", id });

                    if (!ModelState.IsValid)
                        return Page();

                    var validDetailedAddressContent = Input.DetailedAddress.Trim();
                    var validZipOrPostalCodeContent = Input.ZipOrPostalCode.Trim();
                    var validPhoneNumberContent = Input.PhoneNumber.Trim();

                    if (orderToUpdate.FirstName != Input.FirstName
                        || orderToUpdate.LastName != Input.LastName
                        || orderToUpdate.DetailedAddress != validDetailedAddressContent
                        || orderToUpdate.ZipOrPostalCode != validZipOrPostalCodeContent
                        || orderToUpdate.PhoneNumber != validPhoneNumberContent)
                    {
                        if (await TryUpdateModelAsync(
                            orderToUpdate,
                            "Input",
                            order => order.FirstName,
                            order => order.LastName,
                            order => order.DetailedAddress,
                            order => order.ZipOrPostalCode,
                            order => order.PhoneNumber))
                        {
                            if (orderToUpdate.OrderStatusId == 1)
                            {
                                orderToUpdate.OrderStatusId = 4;
                                _context.Attach(orderToUpdate).State = EntityState.Modified;
                            }

                            try
                            {
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Delivery info of specified order has been updated successfully.");
                                StatusMessage = string.Format("Delivery info of the order with ID \"{0}\" has been updated.", id);
                            }
                            catch (DbUpdateConcurrencyException e)
                            {
                                if (!await _context.Orders.AnyAsync(order => order.ID == orderToUpdate.ID))
                                {
                                    _logger.LogError(e, "Error! Failed to find specified order.");
                                    StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\". You may try again.", id);
                                    return RedirectToPage("/Orders/Index", new { area = "Admin" });
                                }
                                else
                                {
                                    _logger.LogError(e, "Error! Failed to set status of specified order to \"Pending payment\".");
                                    StatusMessage = string.Format("Error! Failed to set the status of the order with ID \"{0}\" to \"Pending payment\". You may try again.", id);
                                }
                            }
                        }
                        else
                        {
                            _logger.LogError("Error! Failed to update delivery info of specified order.");
                            StatusMessage = string.Format("Error! Failed to update delivery info of the order with ID \"{0}\". You may try again.", id);
                        }
                    }
                }
                else
                    StatusMessage = string.Format("Error! For the current status of the order with ID \"{0}\", you cannot edit delivery info.", id);

                return RedirectToPage("/Orders/Details", new { area = "Admin", id });
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnPostAsync

        private async Task<bool> HasTimeoutAsync(Order orderToUpdate)
        {
            if (orderToUpdate.OrderStatusId == 1 && new TimeSpan(DateTimeOffset.Now.Ticks - orderToUpdate.OrderTime.ToLocalTime().Ticks).TotalMinutes > 15)
            {
                orderToUpdate.OrderStatusId = 2;
                _context.Attach(orderToUpdate).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified order is failed due to timeout of confirming delivery info.");
                    StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because the customer has failed to confirm delivery info in 15 minutes after creating an order.", orderToUpdate.ID);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!await _context.Orders.AnyAsync(order => order.ID == orderToUpdate.ID))
                    {
                        _logger.LogError(e, "Error! Failed to find specified order to update order status, although timeout of confirming delivery info has been detected.");
                        StatusMessage = string.Format("Error! Failed to find the order with ID \"{0}\" to update the order status, although timeout of confirming delivery info has been detected. You may try again.", orderToUpdate.ID);
                    }
                    else
                    {
                        _logger.LogError(e, "Error! Failed to set status of specified order to \"Failed\", although timeout of confirming delivery info has been detected.");
                        StatusMessage = string.Format("Error! The order with ID \"{0}\" is failed because the customer has failed to confirm delivery info in 15 minutes after creating an order. However, the system has failed to set the status of the order to \"Failed\". You may try again.", orderToUpdate.ID);
                    }
                }

                return true;
            }

            return false;
        } // end method HasTimeoutAsync
    } // end class DeliveryInfoModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Orders
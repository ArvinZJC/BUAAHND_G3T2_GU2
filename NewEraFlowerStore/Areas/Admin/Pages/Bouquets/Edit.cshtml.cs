// csharp file that contains actions of the page for editing a bouquet

#region Using Directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewEraFlowerStore.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Bouquets
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="EditModel"/> contains actions of the page for editing a bouquet.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<EditModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor EditModel

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// Indicate whether the date and time is default or not.
        /// </summary>
        public bool IsDefaultDateTime { get; set; }
        /// <summary>
        /// The URL of Photo 1.
        /// </summary>
        public string PhotoUrl1 { get; set; }
        /// <summary>
        /// The URL of Photo 2.
        /// </summary>
        public string PhotoUrl2 { get; set; }
        /// <summary>
        /// A colour list.
        /// </summary>
        public IList<Colour> ColourList { get; set; }
        /// <summary>
        /// A flower list.
        /// </summary>
        public IList<Flower> FlowerList { get; set; }
        /// <summary>
        /// An occasion list.
        /// </summary>
        public IList<Occasion> OccasionList { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        /// <summary>
        /// A <see cref="NewEraFlowerStore.Data.Bouquet"/> object decorated with <see cref="BindPropertyAttribute"/>.
        /// </summary>
        [BindProperty]
        public Bouquet Bouquet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;
            ColourList = await _context.Colours.ToListAsync();
            FlowerList = await _context.Flowers.ToListAsync();
            OccasionList = await _context.Occasions.ToListAsync();

            if (IsEmailConfirmed)
            {
                Bouquet = await _context.Bouquets.FindAsync(id);

                if (Bouquet == null)
                    return NotFound();

                IsDefaultDateTime = Bouquet.LaunchDate == new DateTime() ? true : false;
                PhotoUrl1 = Bouquet.PhotoUrl1;
                PhotoUrl2 = Bouquet.PhotoUrl2;
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
            ColourList = await _context.Colours.ToListAsync();
            FlowerList = await _context.Flowers.ToListAsync();
            OccasionList = await _context.Occasions.ToListAsync();

            if (IsEmailConfirmed)
            {
                var bouquetToUpdate = await _context.Bouquets.FindAsync(Bouquet.ID);

                if (bouquetToUpdate == null)
                    return NotFound();

                IsDefaultDateTime = Bouquet.LaunchDate == new DateTime() ? true : false;
                PhotoUrl1 = bouquetToUpdate.PhotoUrl1;
                PhotoUrl2 = bouquetToUpdate.PhotoUrl2;

                var isValid = true;

                if (Bouquet.LaunchDate == new DateTime())
                {
                    ModelState.AddModelError("Bouquet.LaunchDate", "Please enter a launch date.");

                    isValid = false;
                } // end if

                if (Bouquet.ColourId <= 0 || (Bouquet.ColourId > 0 && await _context.Colours.FindAsync(Bouquet.ColourId) == null))
                {
                    ModelState.AddModelError("Bouquet.ColourId", "Please select a colour.");

                    Bouquet.ColourId = 0;
                    isValid = false;
                } // end if

                if (Bouquet.FlowerId <= 0 || (Bouquet.FlowerId > 0 && await _context.Flowers.FindAsync(Bouquet.FlowerId) == null))
                {
                    ModelState.AddModelError("Bouquet.FlowerId", "Please select a flower.");

                    Bouquet.FlowerId = 0;
                    isValid = false;
                } // end if

                if (Bouquet.OccasionId <= 0 || (Bouquet.OccasionId > 0 && await _context.Occasions.FindAsync(Bouquet.OccasionId) == null))
                {
                    ModelState.AddModelError("Bouquet.OccasionId", "Please select an occasion.");

                    Bouquet.OccasionId = 0;
                    isValid = false;
                } // end if

                // the relevant code in the page for editing a bouquet needs updateing after modifying the condition
                if (Bouquet.OriginalPrice < 0.01M || Bouquet.OriginalPrice > 999.99M)
                {
                    ModelState.AddModelError("Bouquet.OriginalPrice", "Invalid original price. It has been reset.");

                    Bouquet.OriginalPrice = 0.01M;
                    isValid = false;
                } // end if

                // the relevant code in the page for editing a bouquet needs updateing after modifying the condition
                if (Bouquet.Discount < 0M || Bouquet.Discount > 0.99M)
                {
                    ModelState.AddModelError("Bouquet.Discount", "Invalid discount. It has been reset.");

                    Bouquet.Discount = 0M;
                    isValid = false;
                } // end if

                // the relevant code in the page for editing a bouquet needs updateing after modifying the condition
                if (Bouquet.Stocks < 0)
                {
                    ModelState.AddModelError("Bouquet.Stocks", "Invalid stocks. It has been reset.");

                    Bouquet.Stocks = 0;
                    isValid = false;
                } // end if

                if (!ModelState.IsValid)
                    return Page();

                if (!isValid)
                    return Page();

                Bouquet.Name = Bouquet.Name.Trim();
                Bouquet.Description = Bouquet.Description.Trim();
                Bouquet.OriginalPrice = DecimalHelper.ToPriceFormat(Bouquet.OriginalPrice);
                Bouquet.Discount = DecimalHelper.ToPriceFormat(Bouquet.Discount);

                if (bouquetToUpdate.Name != Bouquet.Name
                    || bouquetToUpdate.Description != Bouquet.Description
                    || bouquetToUpdate.LaunchDate != Bouquet.LaunchDate
                    || bouquetToUpdate.ColourId != Bouquet.ColourId
                    || bouquetToUpdate.FlowerId != Bouquet.FlowerId
                    || bouquetToUpdate.OccasionId != Bouquet.OccasionId
                    || bouquetToUpdate.OriginalPrice != Bouquet.OriginalPrice
                    || bouquetToUpdate.Discount != Bouquet.Discount
                    || bouquetToUpdate.Stocks != Bouquet.Stocks)
                {
                    if (await TryUpdateModelAsync(
                        bouquetToUpdate,
                        "Bouquet",
                        bouquet => bouquet.Name,
                        bouquet => bouquet.Description,
                        bouquet => bouquet.LaunchDate,
                        bouquet => bouquet.ColourId,
                        bouquet => bouquet.FlowerId,
                        bouquet => bouquet.OccasionId,
                        bouquet => bouquet.OriginalPrice,
                        bouquet => bouquet.Discount,
                        bouquet => bouquet.Stocks))
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Specified bouquet has been updated successfully.");
                        StatusMessage = string.Format("The bouquet with the name \"{0}\" has been updated.", Bouquet.Name);
                    }
                    else
                    {
                        _logger.LogError("Error! Failed to update specified bouquet.");
                        StatusMessage = string.Format("Error! Failed to update the bouquet with the name \"{0}\". You may try again.", Bouquet.Name);
                        Bouquet = await _context.Bouquets.FindAsync(Bouquet.ID);
                        return Page();
                    } // end if...else
                } // end if
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Bouquets/Index", new { area = "Admin" });
        } // end method OnPostAsync
    } // end class EditModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Bouquets
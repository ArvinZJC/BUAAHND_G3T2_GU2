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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CreateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor CreateModel

        public bool IsEmailConfirmed { get; set; }

        public bool IsDefaultDateTime { get; set; }

        public IList<Colour> ColourList { get; set; }

        public IList<Flower> FlowerList { get; set; }

        public IList<Occasion> OccasionList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Bouquet Bouquet { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;
            IsDefaultDateTime = true;
            ColourList = await _context.Colours.ToListAsync();
            FlowerList = await _context.Flowers.ToListAsync();
            OccasionList = await _context.Occasions.ToListAsync();

            Bouquet = new Bouquet
            {
                OriginalPrice = 0.01M,
                Discount = 0M,
                Stocks = 0
            };

            if (!IsEmailConfirmed)
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);


            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;
            IsDefaultDateTime = Bouquet.LaunchDate == new DateTime() ? true : false;
            ColourList = await _context.Colours.ToListAsync();
            FlowerList = await _context.Flowers.ToListAsync();
            OccasionList = await _context.Occasions.ToListAsync();

            if (IsEmailConfirmed)
            {
                var isValid = true;

                if (Bouquet.LaunchDate == new DateTime())
                {
                    ModelState.AddModelError("Bouquet.LaunchDate", "Please select a launch date.");

                    isValid = false;
                }  

                if (Bouquet.ColourId <= 0 || (Bouquet.ColourId > 0 && await _context.Colours.FindAsync(Bouquet.ColourId) == null))
                {
                    ModelState.AddModelError("Bouquet.ColourId", "Please select a colour.");

                    Bouquet.ColourId = 0;
                    isValid = false;
                }

                if (Bouquet.FlowerId <= 0 || (Bouquet.FlowerId > 0 && await _context.Flowers.FindAsync(Bouquet.FlowerId) == null))
                {
                    ModelState.AddModelError("Bouquet.FlowerId", "Please select a flower.");

                    Bouquet.FlowerId = 0;
                    isValid = false;
                }

                if (Bouquet.OccasionId <= 0 || (Bouquet.OccasionId > 0 && await _context.Occasions.FindAsync(Bouquet.OccasionId) == null))
                {
                    ModelState.AddModelError("Bouquet.OccasionId", "Please select an occasion.");

                    Bouquet.OccasionId = 0;
                    isValid = false;
                }

                if (Bouquet.OriginalPrice < 0.01M || Bouquet.OriginalPrice > 999.99M)
                {
                    ModelState.AddModelError("Bouquet.OriginalPrice", "Invalid original price. It has been reset.");

                    Bouquet.OriginalPrice = 0.01M;
                    isValid = false;
                }

                if (Bouquet.Discount < 0M || Bouquet.Discount > 0.99M)
                {
                    ModelState.AddModelError("Bouquet.Discount", "Invalid discount. It has been reset.");

                    Bouquet.Discount = 0M;
                    isValid = false;
                }

                if (Bouquet.Stocks < 0)
                {
                    ModelState.AddModelError("Bouquet.Stocks", "Invalid stocks. It has been reset.");

                    Bouquet.Stocks = 0;
                    isValid = false;
                }

                if (!ModelState.IsValid)
                    return Page();

                if (!isValid)
                    return Page();

                Bouquet.Name = Bouquet.Name.Trim();
                Bouquet.PhotoUrl1 = "_default1.jpg";
                Bouquet.PhotoUrl2 = "_default2.jpg";
                Bouquet.Description = Bouquet.Description.Trim();
                Bouquet.OriginalPrice = DecimalHelper.ToPriceFormat(Bouquet.OriginalPrice);
                Bouquet.Discount = DecimalHelper.ToPriceFormat(Bouquet.Discount);
                Bouquet.Sales = 0;

                var newBouquet = new Bouquet();

                if (await TryUpdateModelAsync(
                    newBouquet,
                    "Bouquet",
                    bouquet => bouquet.Name,
                    bouquet => bouquet.PhotoUrl1,
                    bouquet => bouquet.PhotoUrl2,
                    bouquet => bouquet.Description,
                    bouquet => bouquet.LaunchDate,
                    bouquet => bouquet.ColourId,
                    bouquet => bouquet.FlowerId,
                    bouquet => bouquet.OccasionId,
                    bouquet => bouquet.OriginalPrice,
                    bouquet => bouquet.Discount,
                    bouquet => bouquet.Stocks,
                    bouquet => bouquet.Sales))
                {
                    _context.Bouquets.Add(Bouquet);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified bouquet has been created successfully.");
                    StatusMessage = string.Format("A bouquet with the name \"{0}\" has been created.", Bouquet.Name);
                    return RedirectToPage("/Bouquets/Index", new { area = "Admin" });
                }

                _logger.LogError("Error! Failed to create specified bouquet.");
                StatusMessage = string.Format("Error! Failed to create a bouquet with the name \"{0}\". You may try again.", Bouquet.Name);
                return Page();
            }

            StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            return Page();
        } // end method OnPostAsync
    } // end class CreateModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Bouquets
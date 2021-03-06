// csharp file that contains actions of the page for editing a flower

#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Flowers
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="EditModel"/> contains actions of the page for editing a flower.
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
        /// The URL of a cover photo.
        /// </summary>
        public string CoverPhotoUrl { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        /// <summary>
        /// A <see cref="NewEraFlowerStore.Data.Flower"/> object decorated with <see cref="BindPropertyAttribute"/>.
        /// </summary>
        [BindProperty]
        public Flower Flower { get; set; }

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
                Flower = await _context.Flowers.FindAsync(id);

                if (Flower == null)
                    return NotFound();

                CoverPhotoUrl = Flower.CoverPhotoUrl;
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
                var flowerToUpdate = await _context.Flowers.FindAsync(Flower.ID);

                if (flowerToUpdate == null)
                    return NotFound();

                CoverPhotoUrl = flowerToUpdate.CoverPhotoUrl;

                if (!ModelState.IsValid)
                    return Page();

                Flower.Name = Flower.Name.Trim();
                Flower.Description = Flower.Description.Trim();

                if (flowerToUpdate.Name != Flower.Name
                    || flowerToUpdate.Description != Flower.Description)
                {
                    if (await TryUpdateModelAsync(
                        flowerToUpdate,
                        "Flower",
                        flower => flower.Name,
                        flower => flower.Description))
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Specified flower has been updated successfully.");
                        StatusMessage = string.Format("The flower with the name \"{0}\" has been updated.", Flower.Name);
                    }
                    else
                    {
                        _logger.LogError("Error! Failed to update specified flower.");
                        StatusMessage = string.Format("Error! Failed to update the flower with the name \"{0}\". You may try again.", Flower.Name);
                        Flower = await _context.Flowers.FindAsync(Flower.ID);
                        return Page();
                    } // end if...else
                } // end if
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Flowers/Index", new { area = "Admin" });
        } // end method OnPostAsync
    } // end class EditModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Flowers
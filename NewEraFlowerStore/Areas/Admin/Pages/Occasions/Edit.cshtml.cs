// csharp file that contains actions of the page for editing an occasion

#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Occasions
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="EditModel"/> contains actions of the page for editing an occasion.
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
        /// An <see cref="NewEraFlowerStore.Data.Occasion"/> object decorated with <see cref="BindPropertyAttribute"/>.
        /// </summary>
        [BindProperty]
        public Occasion Occasion { get; set; }

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
                Occasion = await _context.Occasions.FindAsync(id);

                if (Occasion == null)
                    return NotFound();

                CoverPhotoUrl = Occasion.CoverPhotoUrl;
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
                var occasionToUpdate = await _context.Occasions.FindAsync(Occasion.ID);

                if (occasionToUpdate == null)
                    return NotFound();

                CoverPhotoUrl = occasionToUpdate.CoverPhotoUrl;

                if (!ModelState.IsValid)
                    return Page();

                Occasion.Name = Occasion.Name.Trim();
                Occasion.Description = Occasion.Description.Trim();

                if (occasionToUpdate.Name != Occasion.Name
                    || occasionToUpdate.Description != Occasion.Description)
                {
                    if (await TryUpdateModelAsync(
                        occasionToUpdate,
                        "Occasion",
                        occasion => occasion.Name,
                        occasion => occasion.Description))
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Specified occasion has been updated successfully.");
                        StatusMessage = string.Format("The occasion with the name \"{0}\" has been updated.", Occasion.Name);
                    }
                    else
                    {
                        _logger.LogError("Error! Failed to update specified occasion.");
                        StatusMessage = string.Format("Error! Failed to update the occasion with the name \"{0}\". You may try again.", Occasion.Name);
                        Occasion = await _context.Occasions.FindAsync(Occasion.ID);
                        return Page();
                    } // end if...else
                } // end if
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Occasions/Index", new { area = "Admin" });
        } // end method OnPostAsync
    } // end class EditModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Occasions
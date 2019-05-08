#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Colours
{
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

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Colour Colour { get; set; }

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
                Colour = await _context.Colours.FindAsync(id);

                if (Colour == null)
                    return NotFound();
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
                if (!ModelState.IsValid)
                    return Page();

                var colourToUpdate = await _context.Colours.FindAsync(Colour.ID);

                if (colourToUpdate == null)
                    return NotFound();

                Colour.Name = Colour.Name.Trim();

                if (colourToUpdate.Name != Colour.Name)
                {
                    if (await TryUpdateModelAsync(
                        colourToUpdate,
                        "Colour",
                        colour => colour.Name))
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Specified colour has been updated successfully.");
                        StatusMessage = string.Format("The colour with the name \"{0}\" has been updated.", Colour.Name);
                    }
                    else
                    {
                        _logger.LogError("Error! Failed to update specified colour.");
                        StatusMessage = string.Format("Error! Failed to update the colour with the name \"{0}\". You may try again.", Colour.Name);
                        return Page();
                    }
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Colours/Index", new { area = "Admin" });
        } // end method OnPostAsync
    } // end class EditModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Colours
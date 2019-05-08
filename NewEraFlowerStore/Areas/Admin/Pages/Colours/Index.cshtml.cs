#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Colours
{
    public class IndexModel : PageModel
    {
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
        } // end constructor IndexModel

        public bool IsEmailConfirmed { get; set; }

        public List<Colour> ColourForm { get; set; }

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
                ColourForm = new List<Colour>();

                foreach (var item in _context.Colours)
                    ColourForm.Add(item);
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return Page();
        } // end method OnGetAsync

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

            if (IsEmailConfirmed)
            {
                if (id == null)
                    return NotFound();

                var colourToDelete = await _context.Colours.FindAsync(id);

                if (colourToDelete == null)
                {
                    StatusMessage = string.Format("Error! It seems that the colour with ID \"{0}\" does not exist.", id);
                    return RedirectToPage("/Colours/Index", new { area = "Admin" });
                }

                try
                {
                    _context.Colours.Remove(colourToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified colour has been deleted succesfully.");
                    StatusMessage = string.Format("The colour with the name \"{0}\" has been deleted.", colourToDelete.Name);
                }
                catch (DbUpdateException e)
                {
                    _logger.LogError(e, "Error! Failed to delete specified colour.");
                    StatusMessage = string.Format("Error! Failed to delete the colour with the name \"{0}\". You may try again.", colourToDelete.Name);
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Colours/Index", new { area = "Admin" });
        } // end method OnPostDeleteAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Colours
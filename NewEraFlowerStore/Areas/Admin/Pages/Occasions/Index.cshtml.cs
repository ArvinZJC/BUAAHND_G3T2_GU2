#region Using Directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Occasions
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment,
            ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        } // end constructor IndexModel

        public bool IsEmailConfirmed { get; set; }

        public List<Occasion> OccasionForm { get; set; }

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
                OccasionForm = new List<Occasion>();

                foreach (var item in _context.Occasions)
                    OccasionForm.Add(item);
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

                var occasionToDelete = await _context.Occasions.FindAsync(id);

                if (occasionToDelete == null)
                {
                    StatusMessage = string.Format("Error! It seems that the occasion with ID \"{0}\" does not exist.", id);
                    return RedirectToPage("/Occasions/Index", new { area = "Admin" });
                }

                if (occasionToDelete.CoverPhotoUrl != "_default.jpg")
                {
                    var coverPhotoFilePath = _hostingEnvironment.WebRootPath + $@"\img\cover_photos\occasions\" + occasionToDelete.CoverPhotoUrl;

                    try
                    {
                        if (System.IO.File.Exists(coverPhotoFilePath))
                        {
                            System.IO.File.Delete(coverPhotoFilePath);
                            _logger.LogInformation("Cover photo deleted successfully.");
                        }
                        else
                            _logger.LogWarning("Cover photo cannot be found.");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error! Failed to delete cover photo.");
                        StatusMessage = string.Format("Error! Failed to delete the cover photo when deleting the occasion with the name \"{0}\". You may try again.", occasionToDelete.Name);
                        return RedirectToPage("/Occasions/Index", new { area = "Admin" });
                    }
                }

                try
                {
                    _context.Occasions.Remove(occasionToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified occasion has been deleted succesfully.");
                    StatusMessage = string.Format("The occasion with the name \"{0}\" has been deleted.", occasionToDelete.Name);
                }
                catch (DbUpdateException e)
                {
                    _logger.LogError(e, "Error! Failed to delete specified occasion.");
                    StatusMessage = string.Format("Error! Failed to delete the occasion with the name \"{0}\". You may try again.", occasionToDelete.Name);
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Occasions/Index", new { area = "Admin" });
        } // end method OnPostDeleteAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Occasions
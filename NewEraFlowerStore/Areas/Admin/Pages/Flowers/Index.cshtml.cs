// csharp file that contains actions of the flower list page

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

namespace NewEraFlowerStore.Areas.Admin.Pages.Flowers
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="IndexModel"/> contains actions of the flower list page.
    /// </summary>
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

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// A flower form.
        /// </summary>
        public List<Flower> FlowerForm { get; set; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
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
                FlowerForm = new List<Flower>();

                foreach (var item in _context.Flowers)
                    FlowerForm.Add(item);
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

                var flowerToDelete = await _context.Flowers.FindAsync(id);

                if (flowerToDelete == null)
                {
                    StatusMessage = string.Format("Error! It seems that the flower with ID \"{0}\" does not exist.", id);
                    return RedirectToPage("/Flowers/Index", new { area = "Admin" });
                } // end if

                if (flowerToDelete.CoverPhotoUrl != "_default.jpg")
                {
                    var coverPhotoFilePath = _hostingEnvironment.WebRootPath + $@"\img\cover_photos\flowers\" + flowerToDelete.CoverPhotoUrl;

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
                        StatusMessage = string.Format("Error! Failed to delete the cover photo when deleting the flower with the name \"{0}\". You may try again.", flowerToDelete.Name);
                        return RedirectToPage("/Flowers/Index", new { area = "Admin" });
                    } // end try...catch
                } // end if

                try
                {
                    _context.Flowers.Remove(flowerToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified flower has been deleted succesfully.");
                    StatusMessage = string.Format("The flower with the name \"{0}\" has been deleted.", flowerToDelete.Name);
                }
                catch (DbUpdateException e)
                {
                    _logger.LogError(e, "Error! Failed to delete specified flower.");
                    StatusMessage = string.Format("Error! Failed to delete the flower with the name \"{0}\". You may try again.", flowerToDelete.Name);
                } // end try...catch
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Flowers/Index", new { area = "Admin" });
        } // end method OnPostDeleteAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Flowers
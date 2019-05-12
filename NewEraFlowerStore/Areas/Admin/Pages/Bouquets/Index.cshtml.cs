// csharp file that contains actions of the bouquet list page for an administrator

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
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages.Bouquets
{
    /// <summary>
    /// Extending from class <see cref="PageModel"/>, the class <see cref="IndexModel"/> contains actions of the bouquet list page for an administrator.
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
        /// A bouquet list.
        /// </summary>
        public List<Bouquet> BouquetList { get; set; }
        /// <summary>
        /// A bouquet form.
        /// </summary>
        public List<object> BouquetForm { get; set; }
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
                BouquetList = new List<Bouquet>();
                BouquetForm = new List<object>();

                foreach (var item in _context.Bouquets
                    .Include(bouquet => bouquet.Colour)
                    .Include(bouquet => bouquet.Flower)
                    .Include(bouquet => bouquet.Occasion))
                {
                    BouquetList.Add(item);
                    BouquetForm.Add(new
                    {
                        Id = item.ID,
                        item.Name,
                        item.Stocks,
                        item.Sales,
                        item.LaunchDate,
                        Colour = item.Colour.Name,
                        Flower = item.Flower.Name,
                        Occasion = item.Occasion.Name,
                        Price = DecimalHelper.ToPriceFormat(item.OriginalPrice * (1 - item.Discount)),
                        item.OriginalPrice,
                        item.Discount,
                        item.Description,
                        item.PhotoUrl1,
                        item.PhotoUrl2
                    });
                } // end foreach
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

                var bouquetToDelete = await _context.Bouquets.FindAsync(id);

                if (bouquetToDelete == null)
                {
                    StatusMessage = string.Format("Error! It seems that the bouquet with ID \"{0}\" does not exist.", id);
                    return RedirectToPage("/Bouquets/Index", new { area = "Admin" });
                } // end if

                if (bouquetToDelete.PhotoUrl1 != "_default1.jpg")
                {
                    var photo1FilePath = _hostingEnvironment.WebRootPath + $@"\img\bouquets\" + bouquetToDelete.PhotoUrl1;

                    try
                    {
                        if (System.IO.File.Exists(photo1FilePath))
                        {
                            System.IO.File.Delete(photo1FilePath);
                            _logger.LogInformation("Photo 1 deleted successfully.");
                        }
                        else
                            _logger.LogWarning("Photo 1 cannot be found.");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error! Failed to delete Photo 1.");
                        StatusMessage = string.Format("Error! Failed to delete Photo 1 when deleting the bouquet with the name \"{0}\". You may try again.", bouquetToDelete.Name);
                        return RedirectToPage("/Bouquets/Index", new { area = "Admin" });
                    } // end try...catch
                } // end if

                if (bouquetToDelete.PhotoUrl2 != "_default2.jpg")
                {
                    var photo2FilePath = _hostingEnvironment.WebRootPath + $@"\img\bouquets\" + bouquetToDelete.PhotoUrl2;

                    try
                    {
                        if (System.IO.File.Exists(photo2FilePath))
                        {
                            System.IO.File.Delete(photo2FilePath);
                            _logger.LogInformation("Photo 2 deleted successfully.");
                        }
                        else
                            _logger.LogWarning("Photo 2 cannot be found.");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error! Failed to delete Photo 2.");
                        StatusMessage = string.Format("Error! Failed to delete Photo 2 when deleting the bouquet with the name \"{0}\". You may try again.", bouquetToDelete.Name);
                        return RedirectToPage("/Bouquets/Index", new { area = "Admin" });
                    } // end try...catch
                } // end if

                try
                {
                    _context.Bouquets.Remove(bouquetToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified bouquet has been deleted succesfully.");
                    StatusMessage = string.Format("The bouquet with the name \"{0}\" has been deleted.", bouquetToDelete.Name);
                }
                catch (DbUpdateException e)
                {
                    _logger.LogError(e, "Error! Failed to delete specified bouquet.");
                    StatusMessage = string.Format("Error! Failed to delete the bouquet with the name \"{0}\". You may try again.", bouquetToDelete.Name);
                } // end try...catch
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/Bouquets/Index", new { area = "Admin" });
        } // end method OnPostDeleteAsync
    } // end class IndexModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Bouquets
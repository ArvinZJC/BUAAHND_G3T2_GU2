#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class SalesRecordsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SalesRecordsModel> _logger;

        public SalesRecordsModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SalesRecordsModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        } // end constructor SalesRecordsModel

        public bool IsEmailConfirmed { get; set; }

        public List<SalesRecord> SalesRecordList { get; set; }

        public List<object> SalesRecordForm { get; set; }

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
                SalesRecordList = new List<SalesRecord>();
                SalesRecordForm = new List<object>();

                foreach (var item in _context.SalesRecords)
                {
                    SalesRecordList.Add(item);
                    SalesRecordForm.Add(new
                    {
                        Id = item.ID,
                        SalesAmount = DecimalHelper.ToPriceFormat(item.SalesAmount),
                        CreationTime = item.CreationTime.ToLocalTime()
                    });
                }
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

                var salesRecordToDelete = await _context.SalesRecords.FindAsync(id);

                if (salesRecordToDelete == null)
                {
                    StatusMessage = string.Format("Error! It seems that the sales record with ID \"{0}\" does not exist.", id);
                    return RedirectToPage("/SalesRecords", new { area = "Admin" });
                }

                try
                {
                    _context.SalesRecords.Remove(salesRecordToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified sales record has been deleted succesfully.");
                    StatusMessage = string.Format("The sales record with ID \"{0}\" has been deleted.", id);
                }
                catch (DbUpdateException e)
                {
                    _logger.LogError(e, "Error! Failed to delete specified sales record.");
                    StatusMessage = string.Format("Error! Failed to delete the sales record with ID \"{0}\". You may try again.", id);
                }
            }
            else
                StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);

            return RedirectToPage("/SalesRecords", new { area = "Admin" });
        } // end method OnPostDeleteAsync
    } // end class SalesRecordsModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages
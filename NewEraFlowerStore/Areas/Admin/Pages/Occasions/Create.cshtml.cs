// csharp file that contains actions of the page for creating an occasion

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
    /// Extending from class <see cref="PageModel"/>, the class <see cref="CreateModel"/> contains actions of the page for creating an occasion.
    /// </summary>
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

        /// <summary>
        /// Indicate whether the email is confirmed or not.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            IsEmailConfirmed = user.EmailConfirmed;

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

            if (IsEmailConfirmed)
            {
                if (!ModelState.IsValid)
                    return Page();

                Occasion.Name = Occasion.Name.Trim();
                Occasion.Description = Occasion.Description.Trim();
                Occasion.CoverPhotoUrl = "_default.jpg";

                var newOccasion = new Occasion();

                if (await TryUpdateModelAsync(
                    newOccasion,
                    "Occasion",
                    occasion => occasion.Name,
                    occasion => occasion.Description,
                    occasion => occasion.CoverPhotoUrl))
                {
                    _context.Occasions.Add(Occasion);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified occasion has been created successfully.");
                    StatusMessage = string.Format("An occasion with the name \"{0}\" has been created.", Occasion.Name);
                    return RedirectToPage("/Occasions/Index", new { area = "Admin" });
                } // end if

                _logger.LogError("Error! Failed to create specified occasion.");
                StatusMessage = string.Format("Error! Failed to create an occasion with the name \"{0}\". You may try again.", Occasion.Name);
                return Page();
            } // end if

            StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            return Page();
        } // end method OnPostAsync
    } // end class CreateModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Occasions
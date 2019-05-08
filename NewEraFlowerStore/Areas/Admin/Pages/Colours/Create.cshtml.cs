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

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Colour Colour { get; set; }

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

                Colour.Name = Colour.Name.Trim();

                var newColour = new Colour();

                if (await TryUpdateModelAsync(
                    newColour,
                    "Colour",
                    colour => colour.Name))
                {
                    _context.Colours.Add(Colour);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Specified colour has been created successfully.");
                    StatusMessage = string.Format("A colour with the name \"{0}\" has been created.", Colour.Name);
                    return RedirectToPage("/Colours/Index", new { area = "Admin" });
                }

                _logger.LogError("Error! Failed to create specified colour.");
                StatusMessage = string.Format("Error! Failed to create a colour with the name \"{0}\". You may try again.", Colour.Name);
                return Page();
            }

            StatusMessage = string.Format("Error! Your email address \"{0}\" has not been verified. Please verify it from your profile.", user.Email);
            return Page();
        } // end method OnPostAsync
    } // end class CreateModel
} // end namespace NewEraFlowerStore.Areas.Admin.Pages.Colours
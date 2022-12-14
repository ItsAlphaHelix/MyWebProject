
namespace MovieDG.Web.Areas.Identity.Pages.Account.Manage
{
    using AspNetCoreHero.ToastNotification.Abstractions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using MovieDG.Data.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ChangePasswordModel> logger;
        private readonly INotyfService toastNotification;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            INotyfService toastNotification)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.toastNotification = toastNotification;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            private const int MaxLengthNewPassword = 100;
            private const int MinLengthNewPassword = 6;

            private const string NewPasswordErrorMessageMaxAndMinLength = "The {0} must be at least {2} and at max {1} characters long.";
            private const string NewPasswordConfirmationError = "The new password and confirmation password do not match.";

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(MaxLengthNewPassword, ErrorMessage = NewPasswordErrorMessageMaxAndMinLength, MinimumLength = MinLengthNewPassword)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = NewPasswordConfirmationError)]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
            }

            var hasPassword = await this.userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
            }

            if (Input.OldPassword == Input.NewPassword)
            {
                ModelState.AddModelError(string.Empty, "The new password cannot be the same as the old!");

                return Page();
            }

            var changePasswordResult = await this.userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await this.signInManager.RefreshSignInAsync(user);
            this.logger.LogInformation("User changed their password successfully.");
            this.toastNotification.Success("Your password has been changed.");

            return RedirectToPage();
        }
    }
}

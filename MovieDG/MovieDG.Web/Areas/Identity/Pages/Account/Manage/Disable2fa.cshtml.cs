namespace MovieDG.Web.Areas.Identity.Pages.Account.Manage
{
    using AspNetCoreHero.ToastNotification.Abstractions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using MovieDG.Data.Data.Models;
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<Disable2faModel> logger;
        private readonly INotyfService toastNotification;

        public Disable2faModel(
            UserManager<ApplicationUser> userManager,
            ILogger<Disable2faModel> logger,
            INotyfService toastNotification)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.toastNotification = toastNotification;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            if (!await this.userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException($"Cannot disable 2FA for user with ID '{this.userManager.GetUserId(this.User)}' as it's not currently enabled.");
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            var disable2faResult = await this.userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            this.logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", this.userManager.GetUserId(this.User));
            this.toastNotification.Success("2fa has been disabled. You can reenable 2fa when you setup an authenticator app");
            return this.RedirectToPage("./TwoFactorAuthentication");
        }
    }
}

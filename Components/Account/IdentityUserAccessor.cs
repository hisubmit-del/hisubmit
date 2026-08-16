using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;


namespace Web.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<BlazorHeroUser> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<BlazorHeroUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}

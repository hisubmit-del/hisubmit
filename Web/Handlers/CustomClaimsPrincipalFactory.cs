using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Infrastructure.Models.Identity;
using HiSubmit.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using HiSubmit.Infrastructure.Contexts;

namespace Web.Handlers;

public class CustomClaimsPrincipalFactory(
    UserManager<BlazorHeroUser> userManager,
    RoleManager<BlazorHeroRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor,
BlazorHeroContext dbContext)
    : UserClaimsPrincipalFactory<BlazorHeroUser, BlazorHeroRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BlazorHeroUser user)
    {
        var identityOptions = optionsAccessor.Value.ClaimsIdentity;
        var claimsIdentity = new ClaimsIdentity(
            authenticationType: IdentityConstants.ApplicationScheme,
            nameType: identityOptions.UserNameClaimType,
            roleType: identityOptions.RoleClaimType);

        var userRoles = await UserManager.GetRolesAsync(user);

        await AddBaseIdentityClaims(claimsIdentity, user);

        await AddRolesToClaims(claimsIdentity, userRoles, user.Id);

        await AddFestivalIdToClaims(claimsIdentity, userRoles, user.Id);

        return claimsIdentity;
    }

    private async Task AddBaseIdentityClaims(ClaimsIdentity claimsIdentity, BlazorHeroUser user)
    {
        var identity = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };
        claimsIdentity.AddClaims(identity);

        if (UserManager.SupportsUserSecurityStamp)
        {
            var securityStamp = await UserManager.GetSecurityStampAsync(user);
            if (!string.IsNullOrWhiteSpace(securityStamp))
            {
                claimsIdentity.AddClaim(new Claim(
                    optionsAccessor.Value.ClaimsIdentity.SecurityStampClaimType,
                    securityStamp));
            }
        }
    }
    private async Task AddRolesToClaims(
        ClaimsIdentity claimsIdentity,
        IList<string> userRoles,
        string userId)
    {
        var festivalAccess = new Dictionary<int, string[]>();
        var festivalId =await  GetFestivalId(userRoles,userId);

        foreach (var roleName in userRoles)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role == null)
                continue;

            var access = await RoleManager.GetClaimsAsync(role);

            if (role is not { FestivalId: null } && role.Name != RoleConstants.FestivalRole)
            {
                if (role.Name != null) claimsIdentity.AddClaim(new(ApplicationClaimTypes.FestivalRole, role.Name));
                festivalAccess.Add(role.FestivalId.Value, access.Select(p => p.Value).ToArray());
            }

            else if (roleName == RoleConstants.FestivalRole && festivalId!=null)
            {
                if (role.Name != null) claimsIdentity.AddClaim(new(ClaimTypes.Role, role.Name));
                    festivalAccess.Add(festivalId.Value, access.Select(p => p.Value).ToArray());
            }

            else
            {
                //Add Role
                if (role.Name != null) claimsIdentity.AddClaim(new(ClaimTypes.Role, role.Name));
                foreach (var a in access)
                    claimsIdentity.AddClaim(new(ApplicationClaimTypes.Permission, a.Value));
            }
        }

        var accessJson = JsonSerializer.Serialize(festivalAccess);
        claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.FestivalPermission, accessJson));
    }
    private async Task AddFestivalIdToClaims(
        ClaimsIdentity claimsIdentity,
        IList<string> userRoles,
        string userId)
    {
        var fesId = await GetFestivalId(userRoles, userId);

        if (fesId != null)
        {
            claimsIdentity.AddClaim(new(ApplicationClaimTypes.FestivalId, fesId.ToString()!));
            //_claimsIdentity.AddClaim(new(ApplicationClaimTypes.SelectedFestival, fesId.ToString()!));
        }

    }
    private async Task<int?> GetFestivalId(IList<string> roles, string userId)
    {
        if (!roles.Any(role => role == RoleConstants.FestivalRole))
            return null;

        // A festival manager may own multiple festival masters. Use the
        // newest master with a valid active festival instead of relying on
        // insertion order or creating a misleading claim with value 0.
        var festival = await dbContext.FestivalMasters
            .Where(master => master.UserId == userId && master.ActiveId != 0)
            .OrderByDescending(master => master.CreatedOn)
            .SelectMany(master => dbContext.Festivals
                .Where(item => item.FestivalMasterId == master.Id &&
                               item.Id == master.ActiveId)
                .Select(item => new { item.Id }))
            .FirstOrDefaultAsync();

        return festival?.Id;
    }
}

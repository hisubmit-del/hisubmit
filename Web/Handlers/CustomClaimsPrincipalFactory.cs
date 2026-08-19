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
        var festivalAccess = new Dictionary<int, HashSet<string>>();
        var ownedOrMemberFestivalIds = await GetFestivalIds(userId);

        foreach (var roleName in userRoles)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role == null)
                continue;

            var access = await RoleManager.GetClaimsAsync(role);

            if (role.FestivalId is int roleFestivalId)
            {
                if (role.Name != null) claimsIdentity.AddClaim(new(ApplicationClaimTypes.FestivalRole, role.Name));
                AddFestivalAccess(festivalAccess, roleFestivalId, access.Select(p => p.Value));
            }

            else if (roleName == RoleConstants.FestivalRole)
            {
                if (role.Name != null) claimsIdentity.AddClaim(new(ClaimTypes.Role, role.Name));
                foreach (var festivalId in ownedOrMemberFestivalIds)
                    AddFestivalAccess(festivalAccess, festivalId, access.Select(p => p.Value));
            }

            else
            {
                //Add Role
                if (role.Name != null) claimsIdentity.AddClaim(new(ClaimTypes.Role, role.Name));
                foreach (var a in access)
                    claimsIdentity.AddClaim(new(ApplicationClaimTypes.Permission, a.Value));
            }
        }

        var accessJson = JsonSerializer.Serialize(
            festivalAccess.ToDictionary(p => p.Key, p => p.Value.ToArray()));
        claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.FestivalPermission, accessJson));
    }

    private static void AddFestivalAccess(
        IDictionary<int, HashSet<string>> festivalAccess,
        int festivalId,
        IEnumerable<string> permissions)
    {
        if (!festivalAccess.TryGetValue(festivalId, out var existing))
        {
            existing = new HashSet<string>(StringComparer.Ordinal);
            festivalAccess[festivalId] = existing;
        }

        foreach (var permission in permissions)
            existing.Add(permission);
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

        return (await GetFestivalIds(userId)).FirstOrDefault();
    }

    private async Task<List<int>> GetFestivalIds(string userId)
    {
        var ownedFestivalIds = await dbContext.Festivals
            .Where(festival => festival.UserId == userId ||
                               (festival.FestivalMaster != null &&
                                festival.FestivalMaster.UserId == userId))
            .Select(festival => festival.Id)
            .ToListAsync();

        var memberFestivalIds = await dbContext.FestivalSubUser
            .Where(member => member.UserId == userId &&
                               !member.IsRemoved &&
                               member.Festival.IsActive)
            .Select(member => member.FestivalId)
            .ToListAsync();

        return ownedFestivalIds
            .Concat(memberFestivalIds)
            .Where(id => id > 0)
            .Distinct()
            .OrderByDescending(id => id)
            .ToList();
    }
}

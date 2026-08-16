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
    private readonly ClaimsIdentity _claimsIdentity = new(IdentityConstants.ApplicationScheme);
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BlazorHeroUser user)
    {

        var userRoles = await UserManager.GetRolesAsync(user);

        await AddBaseIdentityClaims(user);

        await AddRolesToClaims(userRoles,user.Id);

        await AddFestivalIdToClaims(userRoles, user.Id);

        return _claimsIdentity;
    }

    private Task AddBaseIdentityClaims(BlazorHeroUser user)
    {
        var identity = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };
        _claimsIdentity.AddClaims(identity);
        return Task.CompletedTask;
    }
    private async Task AddRolesToClaims(IList<string> userRoles,string userId)
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
                if (role.Name != null) _claimsIdentity.AddClaim(new(ApplicationClaimTypes.FestivalRole, role.Name));
                festivalAccess.Add(role.FestivalId.Value, access.Select(p => p.Value).ToArray());
            }

            else if (roleName == RoleConstants.FestivalRole && festivalId!=null)
            {
                if (role.Name != null) _claimsIdentity.AddClaim(new(ClaimTypes.Role, role.Name));
                    festivalAccess.Add(festivalId.Value, access.Select(p => p.Value).ToArray());
            }

            else
            {
                //Add Role
                if (role.Name != null) _claimsIdentity.AddClaim(new(ClaimTypes.Role, role.Name));
                foreach (var a in access)
                    _claimsIdentity.AddClaim(new(ApplicationClaimTypes.Permission, a.Value));
            }
        }

        var accessJson = JsonSerializer.Serialize(festivalAccess);
        _claimsIdentity.AddClaim(new Claim(ApplicationClaimTypes.FestivalPermission, accessJson));
    }
    private async Task AddFestivalIdToClaims(IList<string> userRoles, string userId)
    {
        var fesId = await GetFestivalId(userRoles, userId);

        if (fesId != null)
        {
            _claimsIdentity.AddClaim(new(ApplicationClaimTypes.FestivalId, fesId.ToString()!));
            //_claimsIdentity.AddClaim(new(ApplicationClaimTypes.SelectedFestival, fesId.ToString()!));
        }

    }
    private async Task<int?> GetFestivalId(IList<string> roles, string userId)
    {
        int? festivalId = null;

        if (roles.Any(role => role == RoleConstants.FestivalRole))
        {

            var festivalMaster = await dbContext.FestivalMasters
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync();


            festivalId = await dbContext.Festivals
                .Where(p => p.FestivalMasterId == festivalMaster.Id &&
                            p.Id == festivalMaster.ActiveId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();
        }

        return festivalId;
    }
}

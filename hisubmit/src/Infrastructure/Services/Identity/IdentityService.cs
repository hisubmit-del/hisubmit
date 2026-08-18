using HiSubmit.Application.Configurations;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Infrastructure.Contexts;
using HiSubmit.Infrastructure.Models.Identity;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace HiSubmit.Infrastructure.Services.Identity;

public class IdentityService : ITokenService
{
    private const string InvalidErrorMessage = "Invalid email or password.";

    private readonly UserManager<BlazorHeroUser> _userManager;
    private readonly RoleManager<BlazorHeroRole> _roleManager;
    private readonly AppConfiguration _appConfig;
    private readonly SignInManager<BlazorHeroUser> _signInManager;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly BlazorHeroContext _context;

    public IdentityService(
        UserManager<BlazorHeroUser> userManager, RoleManager<BlazorHeroRole> roleManager,
        IOptions<AppConfiguration> appConfig, SignInManager<BlazorHeroUser> signInManager,
        IStringLocalizer<IdentityService> localizer,
        BlazorHeroContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfig = appConfig.Value;
        _signInManager = signInManager;
        _localizer = localizer;
        _context = context;
    }

    public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
        }

        if (!user.IsActive)
        {
            return await Result<TokenResponse>.FailAsync(
                _localizer["User Not Active. Please contact the administrator."]);
        }

        if (!user.EmailConfirmed)
        {
            return await Result<TokenResponse>.FailAsync(_localizer["E-Mail not confirmed."],new TokenResponse()
            {
                GoToVerification = true
            });
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            return await Result<TokenResponse>.FailAsync(_localizer["Invalid Credentials."]);
        }

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            
        await _userManager.UpdateAsync(user);

        var token = await GenerateJwtAsync(user);
        var response = new TokenResponse
        { 
            Token = token,
            RefreshToken = user.RefreshToken,
            UserImageURL = user.ProfilePictureDataUrl,
            TokenExpiryTime = DateTime.Now.AddDays(2),
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
    {
        if (model is null)
        {
            return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
        }

        var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
        if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return await Result<TokenResponse>.FailAsync(_localizer["Refresh Token is Expired."]);
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
            TokenExpiryTime = DateTime.Now.AddDays(2)
        };
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    private async Task<string> GenerateJwtAsync(BlazorHeroUser user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(BlazorHeroUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        var permissionClaims = new List<Claim>();

        foreach (var role in roles)
        {
            var thisRole = await _roleManager.FindByNameAsync(role);
            if (thisRole is null)
                continue;

            var rolePermissionClaims = await _roleManager.GetClaimsAsync(thisRole);

            if (thisRole.FestivalId != null)
            {
                var value = $"{thisRole.FestivalId}-{role}";

                roleClaims.Add(new Claim(ApplicationClaimTypes.FestivalRole, value));

                var festivalPermissionClaims =
                    rolePermissionClaims.Select(permission => new Claim(ApplicationClaimTypes.FestivalPermission,
                            $"{thisRole.FestivalId}-{permission.Value}"))
                        .ToList();

                permissionClaims.AddRange(festivalPermissionClaims);
            }
            else
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                permissionClaims.AddRange(rolePermissionClaims);
            }
        }

        var festivalId = 0;


        if (roles.Any(role => role == RoleConstants.FestivalRole))
        {
            var festivalMaster = await _context.FestivalMasters
                .Where(p => p.UserId == user.Id)
                .Where(p => p.ActiveId != 0)
                .OrderByDescending(p => p.CreatedOn)
                .FirstOrDefaultAsync();

            if (festivalMaster is not null)
            {
                festivalId = await _context.Festivals
                    .Where(p => p.FestivalMasterId == festivalMaster.Id &&
                                p.Id == festivalMaster.ActiveId)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
            }
        }



        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new("FestivalId", festivalId.ToString()),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

        return claims;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(2),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException(_localizer["Invalid token"]);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}

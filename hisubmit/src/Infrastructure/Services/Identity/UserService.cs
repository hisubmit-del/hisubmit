using AutoMapper;
using Hangfire;
using HiSubmit.Application.Enums;
using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Application.Requests.Mail;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Infrastructure.Models.Identity;
using HiSubmit.Infrastructure.Specifications;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using HiSubmit.Domain.Enums;
using HiSubmit.Infrastructure.Contexts;

namespace HiSubmit.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly UserManager<BlazorHeroUser> _userManager;
    private readonly RoleManager<BlazorHeroRole> _roleManager;
    private readonly IMailService _mailService;
    private readonly IStringLocalizer<UserService> _localizer;
    private readonly IExcelService _excelService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly BlazorHeroContext _context;

    public UserService(
        UserManager<BlazorHeroUser> userManager,
        IMapper mapper,
        RoleManager<BlazorHeroRole> roleManager,
        IMailService mailService,
        IStringLocalizer<UserService> localizer,
        IExcelService excelService,
        ICurrentUserService currentUserService,
        BlazorHeroContext context)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _mailService = mailService;
        _localizer = localizer;
        _excelService = excelService;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<UserResponse>>> GetAllAsync(List<string> usersId)
    {
        var query = _userManager.Users;

        if (usersId != null)
        {
            query = query.Where(user => usersId.Any(id => id == user.Id));
        }

        var users = await query.ToListAsync();
        var result = _mapper.Map<List<UserResponse>>(users);
        return await Result<List<UserResponse>>.SuccessAsync(result);
    }

    public async Task<Result<RegisterUserResponse>> RegisterAsync(RegisterRequest request, string origin)
    {
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            return await Result<RegisterUserResponse>.FailAsync(string.Format(_localizer["Email {0} is already taken."],
                request.UserName));
        }

        var user = new BlazorHeroUser
        {
            Email = request.Email,
            LastName = request.LastName,
            UserName = request.UserName,
            FirstName = request.FirstName,
            IsActive = request.ActivateUser,
            VerificationCode = request.VerificationCode
            //EmailConfirmed = true
            // EmailConfirmed = request.AutoConfirmEmail
        };

        //if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        //{
        //    var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
        //    if (userWithSamePhoneNumber != null)
        //    {
        //        return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
        //    }
        //}

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail == null)
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            var user23 = await _userManager.FindByEmailAsync(user.Email);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user23);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleConstants.ArtistRole);
                if (!request.AutoConfirmEmail)
                {
                    return await Result<RegisterUserResponse>.SuccessAsync(new RegisterUserResponse() {UserId = user.Id,VerificationCode = token},
                        string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"],
                            user.UserName));
                }

                return await Result<RegisterUserResponse>.SuccessAsync(new RegisterUserResponse() { UserId = user.Id, VerificationCode = token },
                    string.Format(_localizer["User {0} Registered."], user.UserName));
            }
            else
            {
                return await Result<RegisterUserResponse>.FailAsync(result.Errors
                    .Select(a => _localizer[a.Description].ToString()).ToList());
            }
        }
        else
        {
            return await Result<RegisterUserResponse>.FailAsync(string.Format(_localizer["Email {0} is already registered."],
                request.Email));
        }
    }

    private async Task<string> SendVerificationEmail(BlazorHeroUser user, string origin)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var route = "api/identity/user/confirm-email/";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
        return verificationUri;
    }

    public async Task<IResult<UserResponse>> GetAsync(string userId)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        var result = _mapper.Map<UserResponse>(user);
        return await Result<UserResponse>.SuccessAsync(result);
    }

    public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
        var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
        if (isAdmin)
        {
            return await Result.FailAsync(_localizer["Administrators Profile's Enable cannot be toggled"]);
        }

        if (user != null)
        {
            user.IsActive = request.ActivateUser;
            var identityResult = await _userManager.UpdateAsync(user);
        }

        return await Result.SuccessAsync();
    }

    public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId, int? festivalId = null)
    {
        var viewModel = new List<UserRoleModel>();
        var user = await _userManager.FindByIdAsync(userId);

        var roles = await _roleManager.Roles
            .Where(p => p.FestivalId == festivalId).ToListAsync();

        foreach (var role in roles)
        {
            var userRolesViewModel = new UserRoleModel
            {
                RoleName = role.Name,
                RoleDescription = role.Description
            };
            if (await UserIsInRole(user.Id, role.Name, festivalId))
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }

            viewModel.Add(userRolesViewModel);
        }

        var result = new UserRolesResponse { UserRoles = viewModel };
        return await Result<UserRolesResponse>.SuccessAsync(result);
    }

    public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user.Email == "mukesh@blazorhero.com")
        {
            return await Result.FailAsync(_localizer["Not Allowed."]);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

        var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
        if (!await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
        {
            var tryToAddAdministratorRole = selectedRoles
                .Any(x => x.RoleName == RoleConstants.AdministratorRole);
            var userHasAdministratorRole = roles.Any(x => x == RoleConstants.AdministratorRole);
            if (tryToAddAdministratorRole && !userHasAdministratorRole ||
                !tryToAddAdministratorRole && userHasAdministratorRole)
            {
                return await Result.FailAsync(
                    _localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
            }
        }

        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
        return await Result.SuccessAsync(_localizer["Roles Updated"]);
    }

    public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user.VerificationCode == code)
        {
            user.EmailConfirmed=true;
            var f = await _userManager.UpdateAsync(user);
        }
        else
        {
            return await Result<string>.FailAsync("The entered code is incorrect.");
        }

        return await Result<string>.SuccessAsync("User Confirmed Email");

        //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        //var result = await _userManager.ConfirmEmailAsync(user, code);
        //if (result.Succeeded)
        //{
        //    return await Result<string>.SuccessAsync(user.Id,
        //        string.Format(
        //            _localizer[
        //                "Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT."],
        //            user.Email));
        //}
        //else
        //{
        //    throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
        //}
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return await Result.FailAsync(_localizer["An Error has occurred!"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        var passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        var mailRequest = new MailRequest
        {
            Body = string.Format(_localizer["Please reset your password by <a href='{0}>clicking here</a>."],
                HtmlEncoder.Default.Encode(passwordResetURL)),
            Subject = _localizer["Reset Password"],
            To = request.Email
        };
        BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
        return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return await Result.FailAsync(_localizer["An Error has occured!"]);
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (result.Succeeded)
        {
            return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);
        }
        else
        {
            return await Result.FailAsync(_localizer["An Error has occured!"]);
        }
    }

    public async Task<int> GetCountAsync()
    {
        var count = await _userManager.Users.CountAsync();
        return count;
    }

    public async Task<string> ExportToExcelAsync(string searchString = "")
    {
        var userSpec = new UserFilterSpecification(searchString);
        var users = await _userManager.Users
            .Specify(userSpec)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync();
        var result = await _excelService.ExportAsync(users, sheetName: _localizer["Users"],
            mappers: new Dictionary<string, Func<BlazorHeroUser, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["FirstName"], item => item.FirstName },
                { _localizer["LastName"], item => item.LastName },
                { _localizer["UserName"], item => item.UserName },
                { _localizer["Email"], item => item.Email },
                { _localizer["EmailConfirmed"], item => item.EmailConfirmed },
                { _localizer["PhoneNumber"], item => item.PhoneNumber },
                { _localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                { _localizer["IsActive"], item => item.IsActive },
                {
                    _localizer["CreatedOn (Local)"],
                    item => DateTime.SpecifyKind(item.CreatedOn, DateTimeKind.Utc).ToLocalTime()
                        .ToString("G", CultureInfo.CurrentCulture)
                },
                { _localizer["CreatedOn (UTC)"], item => item.CreatedOn.ToString("G", CultureInfo.CurrentCulture) },
                { _localizer["ProfilePictureDataUrl"], item => item.ProfilePictureDataUrl },
            });

        return result;
    }

    public async Task<IResult> AddToRoleAsync(string userId, List<string> rolesName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return await Result.FailAsync(_localizer["User not found"]);

        var missingRoles = new List<string>();
        foreach (var roleName in rolesName.Where(name => !string.IsNullOrWhiteSpace(name)))
        {
            if (!await _userManager.IsInRoleAsync(user, roleName))
                missingRoles.Add(roleName);
        }

        if (missingRoles.Count == 0)
            return await Result.SuccessAsync(_localizer["Roles are already assigned"]);

        var result = await _userManager.AddToRolesAsync(user, missingRoles);
        if (result.Succeeded)
        {
            return await Result.SuccessAsync(_localizer["Roles Updated"]);
        }

        return await Result.FailAsync(
            result.Errors.Select(error => error.Description).ToList());
    }

    public async Task<IResult> AddToRoleAsync(string userId, string roleId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var result =await  _context.UserRoles.AddAsync(new IdentityUserRole<string>()
        {
            RoleId = roleId,
            UserId = userId,
        });
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Dictionary<string, string>> GetUserName(List<string> ids)
    {
        var users = await _userManager.Users.Where(user => ids.Any(id => id == user.Id))
            .ToListAsync();

        var usersName = new Dictionary<string, string>();
        foreach (var user in users)
        {
            usersName.Add(user.Id, user.FirstName + " " + user.LastName);
        }

        return usersName;
    }

    public async Task<Dictionary<string, UserResponse>> GetUser(List<string> ids)
    {
        var users = await _userManager.Users.Where(user => ids.Any(id => id == user.Id))
            .ToListAsync();

        var usersName = new Dictionary<string, UserResponse>();
        foreach (var user in users)
        {
            usersName.Add(user.Id, new UserResponse()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                VerificationCode = user.VerificationCode
            });

            // user.Id, user.FirstName + " " + user.LastName);
        }

        return usersName;
    }

    public async Task<FeeStatus> GetUserType(string userId)
    {
        var feeStatus = await _userManager.Users.Where(p => p.Id == userId)
            .Select(p => p.FeeStatus)
            .FirstOrDefaultAsync();
        return feeStatus;
    }

    public async Task<string> GetUserByEmailAddress(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);
        return user == null ? string.Empty : user.Id;
    }

    public async Task ChangeAccountStatus(FeeStatus status, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        user.FeeStatus = status;
        await _userManager.UpdateAsync(user);
    }

    public async Task<List<UserResponse>> GetAllAdminUsers()
    {
        var adminRole = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
        var adminUsersId = await _context.UserRoles.Where(p => p.RoleId == adminRole.Id)
            .Select(p => p.UserId).ToListAsync();

        var adminUsers = await _context.Users
            .Where(p => adminUsersId.Any(id => id == p.Id))
            .ProjectTo<UserResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return adminUsers;
    }

    private async Task<bool> UserIsInRole(string userId, string roleName, int? festivalId)
    {
        var roleIds = await _roleManager.Roles
            .Where(p => p.FestivalId == festivalId && p.Name == roleName)
            .Select(p => p.Id)
            .ToListAsync();

        var isInRole = await _context.UserRoles
            .Where(p => roleIds.Any(roleId => roleId == p.RoleId) && p.UserId == userId)
            .AnyAsync();
        return isInRole;
    }

    public async Task AddClaim(string userId, Claim claims)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.AddClaimAsync(user, claims);
    }

    public async Task AddClaims(string userId, List<Claim> claims)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.AddClaimsAsync(user, claims);
    }
}

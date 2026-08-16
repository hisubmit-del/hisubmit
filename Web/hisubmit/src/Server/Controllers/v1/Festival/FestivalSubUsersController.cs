using HiSubmit.Application.Features.SubUsers.Commands.AddEditRoles;
using HiSubmit.Application.Features.SubUsers.GetFestivalRoles;
using HiSubmit.Application.Features.SubUsers.Queries.GetFestivalUsers;
using HiSubmit.Application.Features.Users.Commands.Register;
using HiSubmit.Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Festivals.Commands.RemovedUserFromProject;
using HiSubmit.Application.Features.Festivals.Commands.RemoveUserFromFestival;
using HiSubmit.Application.Features.SubUsers.Commands.AddExistingUserToFestival;
using HiSubmit.Application.Requests.Identity;
using Hisubmit.Client.SharedModels.CustomeAttribute;

namespace HiSubmit.Server.Controllers.v1.Festival;

public class FestivalSubUsersController : BaseFestivalController<FestivalSubUsersController>
{
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    public FestivalSubUsersController(IRoleService roleService,IUserService userService)
    {
        _roleService = roleService;
        _userService = userService;
    }

    /// <summary>
    /// Get ProductFestivalId Roles for sub user in festival .the roles use in special festival 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("FestivalRoles")]
    public async Task<IActionResult> GetFestivalRoles([FromQuery] GetFestivalRolesQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Add Or Edit Role  For a festival ,this role dose not working in another festival or admin panel just working in special festival
    /// take festival id with current user data
    /// </summary>
    /// <returns></returns>
    [HttpPost("SaveRole")]
    public async Task<IActionResult> CreateFestivalRoles(AddEditFestivalRoleCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Add User By ProductFestivalId user (Role and privacy is equal to add user with admin and common register)
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("SaveUser")]
    public async Task<IActionResult> SaveUser(RegisterUserCommand command)
    {
        command.IsFestivalUser = true;
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// get All festival user (submitter user or judg or ...)
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("FestivalUser")]
    public async Task<IActionResult> GetFestivalUsers([FromQuery] GetFestivalSubUserQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

        
    /// <summary>
    /// add existing user to festival (existing user equal to user register to site)
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("AddExistingUser")]
    public async Task<IActionResult> AddExistingUserToFestival(AddExistingUserToFestivalCommand command)
    {
        return Ok(await  Mediator.Send(command));
    }


    /// <summary>
    /// get festival permissions
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpGet("RolePermission")]
    public async Task<IActionResult> GetAllPermissions([FromQuery]string roleId)
    {
        return Ok(await _roleService.GetAllPermissionsAsync(roleId, PermissionType.Festival));
    }
        
    /// <summary>
    /// Edit a Role Claim
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("updatePermission")]
    public async Task<IActionResult> Update(PermissionRequest model)
    {
        var response = await _roleService.UpdatePermissionsAsync(model);
        return Ok(response);
    }
    [HttpGet("UserRoles")]
    public async Task<IActionResult> GetUserRole(string userId, int? fId)
    {
        var response = await _userService.GetRolesAsync(userId, fId);
        return Ok(response);
    }
    [HttpPut("UpdateUserRoles")]
    public async Task<IActionResult> UpdateRolesAsync(UpdateUserRolesRequest request)
    {
        return Ok(await _userService.UpdateRolesAsync(request));
    }
        
    /// <summary>
    /// Removed User From ProductFestivalId if user is referee ;referee removed from project judging
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpPost("RemovedUserFromFestival")]
    public async Task<IActionResult> RemoveFromFestival(RemoveUserFromFestivalCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }
        
    /// <summary>
    /// Removed User From project (referee removed from project judging)
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpPost("RemovedUserFromProject")]
    public async Task<IActionResult> RemoveFromProject
        (RemovedUserFromProjectCommand command, int festivalId)
    {
        return Ok(await Mediator.Send(command));
    }
}
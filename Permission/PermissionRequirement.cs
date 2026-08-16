using Microsoft.AspNetCore.Authorization;

namespace Web.Permission;

internal class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; private set; } = permission;
}
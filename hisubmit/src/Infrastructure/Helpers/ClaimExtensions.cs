using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.CustomeAttribute;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiSubmit.Infrastructure.Helpers
{
    public static class ClaimsHelper
    {
        public static void GetAllPermissions(this List<RoleClaimResponse> allPermissions,
            PermissionType? permissionType)
        {

            var modules = typeof(Permissions).GetNestedTypes();

            if (permissionType != null)
            {
                modules = modules.Where(p =>
                    p.GetCustomAttribute<CostumePermissionAttribute>()?.PermissionType == permissionType).ToArray();
            }

            foreach (var module in modules)
            {
                var fields =
                    module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                var fieldName = string.Empty;
                var attrs = module.GetCustomAttributes(false);

                foreach (var attribute in attrs)
                {
                    if (attribute is DisplayAttribute displayAttribute)
                    {
                        if (!string.IsNullOrWhiteSpace(displayAttribute.Name))
                            fieldName = displayAttribute.Name;
                        else
                            fieldName = module.Name;
                    }
                }

                foreach (FieldInfo fi in fields)
                {
                    var propertyValue = fi.GetValue(null);
                    var attributes = fi.GetCustomAttributes(false);
                    var des = string.Empty;
                    var name = string.Empty;
                    foreach (var attribute in attributes)
                    {
                        if (attribute is DisplayAttribute displayAttribute)
                        {
                            des = displayAttribute.Description;
                            name = displayAttribute.Name;
                        }
                    }

                    if (propertyValue is not null)
                        allPermissions.Add(new RoleClaimResponse
                        {
                            Value = propertyValue.ToString(),
                            Type = ApplicationClaimTypes.Permission,
                            GroupName = fieldName,
                            Group = module.Name,
                            Description = des,
                            Title = name
                        });

                }
            }
        }

        public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<BlazorHeroRole> roleManager,
            BlazorHeroRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            if (!allClaims.Any(a => a.Type == ApplicationClaimTypes.Permission && a.Value == permission))
            {
                return await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
            }

            return IdentityResult.Failed();
        }
    }
}
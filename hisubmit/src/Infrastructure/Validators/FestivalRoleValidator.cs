using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Infrastructure.Validators
{
    public class FestivalRoleValidator:RoleValidator<BlazorHeroRole>
    {
        private IdentityErrorDescriber Describer { get; set; }

        public FestivalRoleValidator() : base()
        {

        }
        public override async Task<IdentityResult> ValidateAsync(RoleManager<BlazorHeroRole> manager, BlazorHeroRole role)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            var errors = new List<IdentityError>();
            await ValidateRoleName(manager, role, errors);
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }
            return IdentityResult.Success;
        }
        private async Task ValidateRoleName(RoleManager<BlazorHeroRole> manager, BlazorHeroRole role,
        ICollection<IdentityError> errors)
        {
            var roleName = await manager.GetRoleNameAsync(role);
            if (string.IsNullOrWhiteSpace(roleName))
            {
                errors.Add(Describer.InvalidRoleName(roleName));
            }
            else
            {
                var owner = await manager.FindByNameAsync(roleName);
                if (owner != null
                    && owner.FestivalId == role.FestivalId
                    && !string.Equals(await manager.GetRoleIdAsync(owner), await manager.GetRoleIdAsync(role)))
                {
                    errors.Add(Describer.DuplicateRoleName(roleName));
                }
            }
        }
    }
}

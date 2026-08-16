using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace HiSubmit.Infrastructure.Models.Identity
{
    public class BlazorHeroRole : IdentityRole, IAuditableEntity<string>
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int? FestivalId { get; set; }
        public Festival Festival { get; set; }

        public virtual ICollection<BlazorHeroRoleClaim> RoleClaims { get; set; }

        public BlazorHeroRole() : base()
        {
            RoleClaims = new HashSet<BlazorHeroRoleClaim>();
        }

        public BlazorHeroRole(string roleName,string roleDescription=null):this(roleName,null,roleDescription)
        {
            
        }
        public BlazorHeroRole(string roleName,int? festivalId, string roleDescription = null) : base(roleName)
        {
            Name = roleName;
            RoleClaims = new HashSet<BlazorHeroRoleClaim>();
            Description = roleDescription;
            FestivalId = festivalId;
        }
    }
}
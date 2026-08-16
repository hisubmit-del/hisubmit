using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<BlazorHeroUser>
    {
        public void Configure(EntityTypeBuilder<BlazorHeroUser> builder)
        {
            builder.HasMany(p => p.FestivalMasters).WithOne().HasForeignKey(p => p.UserId);
            builder.HasMany(p=>p.FestivalSubUsers).WithOne().HasForeignKey(p=>p.UserId);
            builder.HasMany(p=>p.ProjectJudgings).WithOne().HasForeignKey(p=>p.UserId);
            builder.HasMany(p => p.SealedTickets).WithOne().HasForeignKey(p => p.UserId);
        }
    }

    public class RoleConfiguration : IEntityTypeConfiguration<BlazorHeroRole>
    {
        public void Configure(EntityTypeBuilder<BlazorHeroRole> builder)
        {
            builder.HasIndex(p => p.NormalizedName).IsUnique(false);
            
        //    builder.HasIndex(p => new { p.NormalizedName, p.FestivalId }).IsUnique(true);

            builder.HasIndex(p => p.Name).IsUnique(false);
        }
    }
}


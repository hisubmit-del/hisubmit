using HiSubmit.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class FestivalFocusConfiguration : IEntityTypeConfiguration<FestivalFocus>
    {
        public void Configure(EntityTypeBuilder<FestivalFocus> builder)
        {
            builder.HasMany(p => p.FestivalFestivalFoci).WithOne(p => p.FestivalFocus).HasForeignKey(p => p.FestivalFocusId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
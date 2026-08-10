using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class FestivalConfiguration : IEntityTypeConfiguration<Festival>
    {
        public void Configure(EntityTypeBuilder<Festival> builder)
        {
            builder.HasOne(p => p.Address).WithOne(p => p.Festival).HasForeignKey<Address>(p=>p.FestivalId);
            builder.HasOne(p => p.SubmissionAddress).WithOne(p => p.SubmissionFestival).HasForeignKey<Address>(p=>p.SubmissionFestivalId);
            builder.HasMany(p => p.FestivalFestivalFoci).WithOne(p => p.Festival).HasForeignKey(p => p.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.FestivalArtCategories).WithOne(p => p.Festival).HasForeignKey(p => p.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
   
        
    }
}
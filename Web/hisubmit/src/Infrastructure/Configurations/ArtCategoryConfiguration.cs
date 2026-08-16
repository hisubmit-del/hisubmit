using HiSubmit.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class ArtCategoryConfiguration : IEntityTypeConfiguration<ArtCategory>
    {
        public void Configure(EntityTypeBuilder<ArtCategory> builder)
        {
            builder.HasMany(p => p.FestivalArtCategories).WithOne(p => p.ArtCategory).HasForeignKey(p => p.ArtCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
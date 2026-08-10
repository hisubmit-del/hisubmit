using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class VenueConfiguration : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.HasOne(p => p.Festival).WithMany(p => p.Venues).HasForeignKey(p => p.FestivalId);
            builder.HasOne(p => p.Address).WithOne(p => p.Venue).HasForeignKey<Address>(p => p.VenueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasOne(p => p.Address).WithOne(p => p.Project).HasForeignKey<Address>(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.XrVrSpecification).WithOne(p => p.Project).HasForeignKey<XrVrSpecification>(p => p.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.FilmSpecification).WithOne(p => p.Project).HasForeignKey<FilmSpecification>(p => p.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.MusicSpecification).WithOne(p => p.Project).HasForeignKey<MusicSpecification>(p => p.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.ScriptSpecification).WithOne(p => p.Project).HasForeignKey<ScriptSpecification>(p => p.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.PhotographySpecification).WithOne(p => p.Project).HasForeignKey<PhotographySpecification>(p => p.ProjectId)
              .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
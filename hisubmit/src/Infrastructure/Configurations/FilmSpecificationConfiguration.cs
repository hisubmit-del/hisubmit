using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class FilmSpecificationConfiguration : IEntityTypeConfiguration<FilmSpecification>
    {
        public void Configure(EntityTypeBuilder<FilmSpecification> builder)
        {
            builder.HasOne(p => p.OriginCountry).WithMany()
                .HasForeignKey(p => p.OriginCountryId)
                .OnDelete(DeleteBehavior.NoAction);

            // builder.HasOne(p => p.FilmingCountry).WithMany()
            //    .HasForeignKey(p => p.FilmingCountryId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class JudgingFiledAnswerConfiguration : IEntityTypeConfiguration<JudgingFiledAnswered>
    {
        public void Configure(EntityTypeBuilder<JudgingFiledAnswered> builder)
        {
            builder.HasOne(p => p.ProjectJudging)
                .WithMany(p => p.JudgingFiledAnswereds)
                .HasForeignKey(p => p.ProjectJudgingId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
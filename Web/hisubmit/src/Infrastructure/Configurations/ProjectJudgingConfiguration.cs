using HiSubmit.Domain.Entities.Festivals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class ProjectJudgingConfiguration : IEntityTypeConfiguration<ProjectJudging>
    {
        public void Configure(EntityTypeBuilder<ProjectJudging> builder)
        {
            builder.HasOne(p => p.JudgingButton)
                .WithMany()
                .HasForeignKey(p => p.JudgingButtonId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
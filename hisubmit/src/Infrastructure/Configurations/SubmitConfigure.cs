using HiSubmit.Domain.Entities.Submitter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations;

public  class  SubmitConfigure:IEntityTypeConfiguration<Submit>
{
    public void Configure(EntityTypeBuilder<Submit> builder)
    {
        builder.HasMany(p => p.SubmitAnswerQuestions)
            .WithOne(p=>p.Submit)
            .HasForeignKey(p=>p.SubmitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
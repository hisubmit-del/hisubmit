using HiSubmit.Domain.Entities.Festivals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations;

public class SubmissionQuestionEventCategoryConfigure :
    IEntityTypeConfiguration<SubmissionQuestionEventCategory>
{
    public void Configure(EntityTypeBuilder<SubmissionQuestionEventCategory> builder)
    {
        builder.HasOne(p => p.SubmissionQuestion).WithMany(p=>p.SubmissionQuestionEventCategories)
            .HasForeignKey(p => p.SubmissionQuestionId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(p => p.EventCategory).WithMany(p => p.SubmissionQuestionEventCategories)
            .HasForeignKey(p => p.EventCategoryId).OnDelete(DeleteBehavior.NoAction);
    }
}
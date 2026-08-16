using HiSubmit.Domain.Entities.Festivals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class SubmissionQuestionConfigure : IEntityTypeConfiguration<SubmissionQuestion>
    {
        public void Configure(EntityTypeBuilder<SubmissionQuestion> builder)
        {
            builder.HasOne(p => p.Festival).WithMany(p => p.SubmissionQuestions)
                .HasForeignKey(p => p.FestivalId);

            builder.HasMany(p => p.Options).WithOne(p => p.Question)
                .HasForeignKey(p => p.QuestionId);

            builder.HasMany(p => p.SubmissionQuestionEventCategories).WithOne(p => p.SubmissionQuestion)
                .HasForeignKey(p => p.SubmissionQuestionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
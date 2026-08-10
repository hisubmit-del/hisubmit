using HiSubmit.Domain.Entities.Festivals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations
{
    public class DeadLineEventCategoryConfiguration : IEntityTypeConfiguration<DeadlineEventCategory>
    {
        public void Configure(EntityTypeBuilder<DeadlineEventCategory> builder)
        {
            builder.HasOne(p => p.DeadLine).WithMany(p => p.DeadlineEventCategories)
                .HasForeignKey(p => p.DeadLineId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.EventCategory).WithMany(p => p.DeadlineEventCategories)
                .HasForeignKey(p => p.EventCategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
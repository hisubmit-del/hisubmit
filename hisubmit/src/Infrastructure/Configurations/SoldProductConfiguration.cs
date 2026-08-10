using HiSubmit.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiSubmit.Infrastructure.Configurations;

public class SoldProductConfiguration:IEntityTypeConfiguration<ProductSold>
{
    public void Configure(EntityTypeBuilder<ProductSold> builder)
    {
        builder.HasOne(p => p.Product).WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

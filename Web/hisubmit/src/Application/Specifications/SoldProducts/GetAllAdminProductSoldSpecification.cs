using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Specifications.SoldProducts;

internal class GetAllAdminProductSoldSpecification:HeroSpecification<ProductSold>
{
    public GetAllAdminProductSoldSpecification()
    {
        Criteria = productSold => productSold.Status == ProductSoldStatus.Paid;
    }
}
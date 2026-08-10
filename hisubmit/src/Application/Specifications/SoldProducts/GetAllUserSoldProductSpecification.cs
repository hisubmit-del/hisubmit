using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;

namespace  HiSubmit.Application.Specifications.SoldProducts;

internal class GetAllUserSoldProductSpecification:HeroSpecification<ProductSold>
{
    public GetAllUserSoldProductSpecification(string userId)
    {
        Criteria = productSold =>
            productSold.Status==ProductSoldStatus.Paid &&
            productSold.UserId==userId;
    }
}




using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Specifications.SoldProducts;

internal class GetAllFestivalSoldProductSpecification : HeroSpecification<ProductSold>
{
    public GetAllFestivalSoldProductSpecification(int festivalId)
    {
        Criteria = (productSold) =>
            productSold.Status == ProductSoldStatus.Paid &&
            productSold.Product.FestivalId == festivalId;
    }
}
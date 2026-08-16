using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Application.Specifications.Payments;

public class DiscountCodeFilterSpecification : HeroSpecification<DiscountCode>
{
    public DiscountCodeFilterSpecification(DiscountCodeFilter filter)
    {
        Criteria=(discountCode) => (filter.Enable==null || filter.Enable==discountCode.Enable) &&
                                       (filter.FestivalId==discountCode.FestivalId);
    }
}
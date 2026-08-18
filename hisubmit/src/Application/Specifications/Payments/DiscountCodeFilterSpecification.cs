using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Application.Specifications.Payments;

public class DiscountCodeFilterSpecification : HeroSpecification<DiscountCode>
{
    public DiscountCodeFilterSpecification(DiscountCodeFilter filter)
    {
        var search = filter.SearchString?.Trim();

        Criteria = discountCode =>
            (filter.Enable == null || filter.Enable == discountCode.Enable) &&
            (filter.FestivalId == discountCode.FestivalId) &&
            (string.IsNullOrWhiteSpace(search) ||
             (discountCode.Code != null && discountCode.Code.Contains(search)) ||
             (discountCode.Description != null && discountCode.Description.Contains(search)));
    }
}

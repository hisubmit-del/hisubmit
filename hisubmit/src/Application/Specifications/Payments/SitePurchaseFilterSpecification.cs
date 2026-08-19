using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using Hisubmit.Hisubmit.Client.SharedModels.Features.AdminDashboard;

namespace HiSubmit.Application.Specifications.Payments;

public class SitePurchaseFilterSpecification:HeroSpecification<CarTItem>
{
    public SitePurchaseFilterSpecification(GetSitePurchaseRequest filter)
    {
        Criteria = (cartItem) =>
                (cartItem.Cart.Paid) &&
                (filter.DateFilter.GetMinDate()==null || filter.DateFilter.GetMinDateTime().Value <=cartItem.Cart.CartDate) &&
                (filter.DateFilter.GetMaxDate()==null || cartItem.Cart.CartDate <= filter.DateFilter.GetMaxDateTime().Value)
        ;
    }
}

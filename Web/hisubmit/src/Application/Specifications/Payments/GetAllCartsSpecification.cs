using System;
using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Application.Specifications.Payments;

public class GetAllCartsFilterSpecification : HeroSpecification<Cart>
{
    public GetAllCartsFilterSpecification(GetAllCartsFilterDto filter)
    {
        Criteria = (cart) =>
                (filter.Paid == null || filter.Paid == cart.Paid) &&
                ((filter.PriceFilter.Number1 == null) ||
                 (filter.PriceFilter.NumberFilterType == NumberFilterType.Equal &&
                  filter.PriceFilter.Number1 == cart.Price) ||
                 (filter.PriceFilter.NumberFilterType == NumberFilterType.GreaterThan &&
                  filter.PriceFilter.Number1 <= cart.Price) ||
                 (filter.PriceFilter.NumberFilterType == NumberFilterType.LessThan &&
                  filter.PriceFilter.Number1 >= cart.Price) ||
                 (filter.PriceFilter.NumberFilterType == NumberFilterType.Range &&
                  filter.PriceFilter.Number1 <= cart.Price && filter.PriceFilter.Number2 >= cart.Price)) &&
                (filter.PaidDateFilter.Period == null ||
                 (filter.PaidDateFilter.Period == TimePeriod.Weekly &&
                  cart.CartDate >= DateTime.Now.AddDays(-7)) ||
                 (filter.PaidDateFilter.Period == TimePeriod.Monthly &&
                  cart.CartDate >= DateTime.Now.AddMonths(-1)) ||
                 (filter.PaidDateFilter.Period == TimePeriod.Yearly &&
                  cart.CartDate >= DateTime.Now.AddYears(-1)) ||
                 (filter.PaidDateFilter.Period == TimePeriod.Period &&
                  cart.CartDate >= filter.PaidDateFilter.Date1 &&
                  cart.CartDate <= filter.PaidDateFilter.Date2)
                ) &&
                (string.IsNullOrWhiteSpace(filter.PaymentId) || cart.PaymentId.Contains(filter.PaymentId)) &&
                (string.IsNullOrWhiteSpace(filter.PayerId) || cart.PayerId.Contains(filter.PayerId)) &&
                (string.IsNullOrWhiteSpace(filter.PaypalEmail) || cart.Email.Contains(filter.PaypalEmail))
            ;
    }
}

public class GetUserCartsSpecification : HeroSpecification<Cart>
{
    public GetUserCartsSpecification(string userId)
    {
        Criteria = (cart) =>string.IsNullOrWhiteSpace(userId) || cart.UserId == userId;
    }
}

public class AllCartQuickSearchSpecification : HeroSpecification<Cart>
{
    public AllCartQuickSearchSpecification(string searchString)
    {
        Criteria = (cart) =>string.IsNullOrWhiteSpace(searchString) ||
            cart.UserId.Contains(searchString) ||
                             cart.PaymentId.Contains(searchString) ||
                             cart.Email.Contains(searchString) ||
                             cart.PayerId.Contains(searchString);
    }
}
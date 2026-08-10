using HiSubmit.Domain.Enums;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Specifications.Base;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;
using System;

namespace HiSubmit.Application.Specifications.Payments;

public class GetAllFestivalCartItemsSpecification : HeroSpecification<CarTItem>
{
    public GetAllFestivalCartItemsSpecification(int? festivalId,int? festivalMasterId)
    {
        Criteria = (cartItem) => (festivalId == null) || (
            (cartItem.CartItemType == CartItemType.Submit && cartItem.Submit.FestivalId == festivalId) ||
            (cartItem.CartItemType == CartItemType.Ticket &&
             cartItem.SoldTicket.Ticket.Venue.FestivalId == festivalId) ||
            (cartItem.CartItemType == CartItemType.Product && cartItem.ProductSold.Product.FestivalId == festivalId))
         &&((festivalMasterId==null)||
         (cartItem.CartItemType == CartItemType.Submit && cartItem.Submit.Festival.FestivalMasterId == festivalMasterId) ||
         (cartItem.CartItemType == CartItemType.Ticket &&
          cartItem.SoldTicket.Ticket.Venue.Festival.FestivalMasterId == festivalMasterId) ||
         (cartItem.CartItemType == CartItemType.Product && cartItem.ProductSold.Product.Festival.FestivalMasterId == festivalMasterId));
        
    }
    // public GetAllFestivalCartItemsSpecification(int? yearsRunning,int? festivalMasterId)
    // {
    //  Includes.Add((p)=>p.Submit.Festival.FestivalMaster);
    //  Criteria = (cartItem) => (festivalMasterId == null) || (
    //   (yearsRunning==-1 && cartItem.Submit.Festival.FestivalMasterId == festivalMasterId) ||
    //   (yearsRunning==-2 &&cartItem.Submit.Festival.FestivalMasterId == festivalMasterId
    //                     && cartItem.Submit.Festival.FestivalMaster.ActivePeriod ==cartItem.Submit.Festival.YearsRunning ) ||
    //   (cartItem.Submit.Festival.FestivalMasterId==festivalMasterId && cartItem.Submit.Festival.YearsRunning==yearsRunning));
    // }
}

public class GetAllAdminCArtItemFilterSpecification : HeroSpecification<CarTItem>
{
    public GetAllAdminCArtItemFilterSpecification(PaymentFilterDto filter)
    {
        Criteria = (cartItem) => ((filter.FestivalId == null) || (
            (cartItem.CartItemType == CartItemType.Submit && cartItem.Submit.FestivalId == filter.FestivalId) ||
            (cartItem.CartItemType == CartItemType.Ticket &&
             cartItem.SoldTicket.Ticket.Venue.FestivalId == filter.FestivalId) ||
            (cartItem.CartItemType == CartItemType.Product &&
             cartItem.ProductSold.Product.FestivalId == filter.FestivalId) ||
            (cartItem.CartItemType == CartItemType.ServiceFee && cartItem.Submit.FestivalId == filter.FestivalId)) )
         &&((filter.MasterFestivalId==null)||
            (cartItem.CartItemType == CartItemType.Submit && cartItem.Submit.Festival.FestivalMasterId == filter.MasterFestivalId) ||
            (cartItem.CartItemType == CartItemType.Ticket &&
             cartItem.SoldTicket.Ticket.Venue.Festival.FestivalMasterId == filter.MasterFestivalId) ||
            (cartItem.CartItemType == CartItemType.Product && cartItem.ProductSold.Product.Festival.FestivalMasterId == filter.MasterFestivalId)||
         (cartItem.CartItemType == CartItemType.ServiceFee && cartItem.Submit.Festival.FestivalMasterId == filter.MasterFestivalId));

    }
}

public class CartItemFilterSpecification : HeroSpecification<CarTItem>
{
    public CartItemFilterSpecification(PaymentFilterDto filter)
    {
        Includes.Add((p) => p.Cart);

        Criteria = (cartItem) =>
                ((filter.IncomeFilter.Number1 == null) ||
                 (filter.IncomeFilter.NumberFilterType == NumberFilterType.Equal &&
                  filter.IncomeFilter.Number1 == cartItem.Price) ||
                 (filter.IncomeFilter.NumberFilterType == NumberFilterType.GreaterThan &&
                  filter.IncomeFilter.Number1 <= cartItem.Price) ||
                 (filter.IncomeFilter.NumberFilterType == NumberFilterType.LessThan &&
                  filter.IncomeFilter.Number1 >= cartItem.Price) ||
                 (filter.IncomeFilter.NumberFilterType == NumberFilterType.Range &&
                  filter.IncomeFilter.Number1 <= cartItem.Price && filter.IncomeFilter.Number2 >= cartItem.Price)) &&
                (filter.CardDateFilter.Period == null ||
                 (filter.CardDateFilter.Period == TimePeriod.Weekly &&
                  cartItem.Cart.CartDate >= DateTime.Now.AddDays(-7)) ||
                 (filter.CardDateFilter.Period == TimePeriod.Monthly &&
                  cartItem.Cart.CartDate >= DateTime.Now.AddMonths(-1)) ||
                 (filter.CardDateFilter.Period == TimePeriod.Yearly &&
                  cartItem.Cart.CartDate >= DateTime.Now.AddYears(-1)) ||
                 (filter.CardDateFilter.Period == TimePeriod.Period &&
                  cartItem.Cart.CartDate >= filter.CardDateFilter.Date1 &&
                  cartItem.Cart.CartDate <= filter.CardDateFilter.Date2)
                )
                 && (filter.ItemType == null || cartItem.CartItemType == (CartItemType)filter.ItemType.Value)
            ;
    }
}
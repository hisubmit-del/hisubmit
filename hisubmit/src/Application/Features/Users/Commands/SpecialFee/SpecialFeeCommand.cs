using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Interfaces.Carts;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Users.Commands.SpecialFee;

public class SpecialFeeCommand : IRequest<IResult>
{
    public StatusFeePeriod Period { get; set; }
}

public class SpecialFeeCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<SpecialFeeCommandHandler> localize,
    ICurrentUserService currentUserService,
    ICartService cartService)
    : FeatureBaseService<SpecialFeeCommandHandler>(mapper, unitOfWork, localize),
        IRequestHandler<SpecialFeeCommand, IResult>
{
    public async Task<IResult> Handle(SpecialFeeCommand request, CancellationToken cancellationToken)
    {
        var siteCommission = await _unitOfWork.Repository<SiteCommission>()
            .Entities.FirstOrDefaultAsync(cancellationToken);

        var userId = currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            return await Result.FailAsync(_localize["You must be signed in to buy Gold."]);

        var hasPendingOrActivePlan = await _unitOfWork.Repository<UserSpecialPeriod>()
            .Entities
            .AnyAsync(p => p.UserId == userId &&
                           (p.Status == UserSpecialAccountStatus.DontPaid ||
                            (p.Status == UserSpecialAccountStatus.Open &&
                             p.CloseDateTime > DateTime.Now)), cancellationToken);
        if (hasPendingOrActivePlan)
            return await Result.FailAsync(_localize["You already have a pending or active Gold plan."]);

        var cost = TakeCost(siteCommission, request.Period);

        var statusAccount = new UserSpecialPeriod()
        {
            UserId = userId,
            Cost = (decimal)cost,
            Status = UserSpecialAccountStatus.DontPaid,
            Period = request.Period
        };

        await _unitOfWork.Repository<UserSpecialPeriod>().AddAsync(statusAccount);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await cartService.AddToCard(new AddToCartRequest
        {
            Price = statusAccount.Cost,
            Title = "Special Account",
            CartItemType = CartItemType.SpecialAccount,
            ItemId = statusAccount.Id.ToString(),
            
        }, cancellationToken);
        return await Result.SuccessAsync(_localize["Your plan added to the shopping card"]);
    }


    private static double TakeCost(SiteCommission siteCommission, StatusFeePeriod feePeriod)
    {
        double cost = 0;
        switch (feePeriod)
        {
            case StatusFeePeriod.Monthly:
                cost = siteCommission.MonthlySpecialUserFee;
                break;
            case StatusFeePeriod.ThreeMonth:
                cost = siteCommission.ThreeMonthlySpecialUserFee;
                break;
            case StatusFeePeriod.Yearly:
                cost = siteCommission.YearlySpecialUserFee;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(feePeriod), feePeriod, null);
        }

        return cost;
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.SpecialAccounts.Queries;

public class GetUserAccountTypeQuery : IRequest<IResult<GetUserAccountTypeResponse>>
{
    public string UserId { get; set; }
}

public class GetUserAccountType : IRequestHandler<GetUserAccountTypeQuery,
    IResult<GetUserAccountTypeResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetUserAccountType(IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IResult<GetUserAccountTypeResponse>> Handle(GetUserAccountTypeQuery request,
        CancellationToken cancellationToken)
    {
        var userId = string.IsNullOrWhiteSpace(request.UserId)
            ? _currentUserService.UserId
            : request.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return await Result<GetUserAccountTypeResponse>.SuccessAsync(new GetUserAccountTypeResponse());

        var special = await _unitOfWork.Repository<UserSpecialPeriod>()
            .Entities
            .Where(p => p.UserId == userId &&
                        p.Status == UserSpecialAccountStatus.Open &&
                        p.OpenDateTime <= DateTime.Now &&
                        p.CloseDateTime > DateTime.Now)
            .OrderByDescending(p => p.CloseDateTime)
            .FirstOrDefaultAsync(cancellationToken);
        if (special != null)
        {
            var result = new GetUserAccountTypeResponse
            {
                FeeStatus = FeeStatus.Special,
                CloseDate = special.CloseDateTime,
                OpenDate = special.OpenDateTime,
                Id = special.Id
            };
        return await Result<GetUserAccountTypeResponse>.SuccessAsync(result);
        }

        return await Result<GetUserAccountTypeResponse>.SuccessAsync(new GetUserAccountTypeResponse() );
    }
}

public class GetUserAccountTypeResponse
{
    public int Id { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }
    public FeeStatus FeeStatus { get; set; }
}

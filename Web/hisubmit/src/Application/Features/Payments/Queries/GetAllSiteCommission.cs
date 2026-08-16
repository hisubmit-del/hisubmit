using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Payments.Queries;

public class GetSiteCommissionQuery:IRequest<IResult<GetSiteCommissionResponse>>
{
    
}

public class GetSubmissionCommissionQueryHandler:FeatureBaseService<GetSubmissionCommissionQueryHandler>, IRequestHandler<GetSiteCommissionQuery,IResult<GetSiteCommissionResponse>>
{
    public GetSubmissionCommissionQueryHandler
        (IMapper mapper,IStringLocalizer<GetSubmissionCommissionQueryHandler>localize,IUnitOfWork<int> unitOfWork)
        :base(mapper,unitOfWork,localize)
    {
        
    }
    public async Task<IResult<GetSiteCommissionResponse>> Handle(GetSiteCommissionQuery request, CancellationToken cancellationToken)
    {
        var siteCommission =await _unitOfWork.Repository<SiteCommission>()
            .Entities.ProjectTo<GetSiteCommissionResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        return await Result<GetSiteCommissionResponse>.SuccessAsync(siteCommission);
    }
}

public class GetSiteCommissionResponse
{
    public double SubmitServiceFee { get; set; }
    public double MinimumServiceFee { get; set; }
    public double MaximumServiceFee { get; set; }

    public double UsualFestivalCommission { get; set; }
    public double SpecialFestivalCommission { get; set; }

    public double TicketSalesCommission { get; set; }
    public double ProductSalesCommission { get; set; }
    
    
    public double  MonthlySpecialUserFee { get; set; } 
    public double ThreeMonthlySpecialUserFee { get; set; }
    public  double YearlySpecialUserFee { get; set; }
}
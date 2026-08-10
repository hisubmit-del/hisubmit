using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Payments.Commands.EditSiteCommission;

public class EditSiteCommissionCommand : IRequest<IResult>
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
    public double YearlySpecialUserFee { get; set; }
}

public class EditSiteCommissionCommandHandler : IRequestHandler<EditSiteCommissionCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<EditSiteCommissionCommandHandler> _localizer;

    public EditSiteCommissionCommandHandler
    (IUnitOfWork<int> unitOfWork, IMapper mapper,
        IStringLocalizer<EditSiteCommissionCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IResult> Handle(EditSiteCommissionCommand request, CancellationToken cancellationToken)
    {
        var dbCommission = await _unitOfWork.Repository<SiteCommission>()
            .Entities.FirstOrDefaultAsync(cancellationToken);

        var updateCommission = _mapper.Map(request, dbCommission);
        await _unitOfWork.Repository<SiteCommission>().UpdateAsync(updateCommission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["Site Commission Updated"]);
    }
}

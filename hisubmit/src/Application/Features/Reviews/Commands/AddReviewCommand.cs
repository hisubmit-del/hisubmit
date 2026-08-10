using System.Linq;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Festivals.ViolationReportField;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Reviews.Commands;

public class AddReviewCommand : IRequest<IResult>
{
    public string Text { get; set; }
    public string UserId { get; set; }
   
    public int Rate { get; set; }
    public CommentType Type { get; set; }
    public int FestivalId { get; set; }
}

public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddReviewCommandHandler> _localize;
    private readonly ICurrentUserService _currentUserService;

    public AddReviewCommandHandler
    (
        IMapper mapper,
        IMediator mediator,
        IUnitOfWork<int> unitOfWork,
        ICurrentUserService currentUserService,
        IStringLocalizer<AddReviewCommandHandler> localize)
    {
        _mapper = mapper;
        _localize = localize;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IResult> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {                
        //To do Add IpAddress checker
        if (request.Type != CommentType.ViolationReport && !string.IsNullOrWhiteSpace(_currentUserService.UserIP))
        {
            var dbReview = await _unitOfWork.Repository<Review>()
                .Entities
                .AnyAsync(p =>  p.ClientIp == _currentUserService.UserIP || _currentUserService.UserId == p.UserId, cancellationToken: cancellationToken);
            if (dbReview)
                return await Result.FailAsync(_localize["You have already registered your comment"]);
        }
            
        var model = _mapper.Map<Review>(request);
        model.UserId = _currentUserService.UserId;
        model.ClientIp = _currentUserService.UserIP;
        await _unitOfWork.Repository<Review>().AddAsync(model);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var message = request.Type == CommentType.Review 
            ? "Review Send" : "Report Send";

        if (request.Type == CommentType.ViolationReport)
            await _mediator.Publish(new ViolationReportFieldEvent
            {
                UserId = model.UserId,
                FestivalId = request.FestivalId,
            }, cancellationToken);
        return await Result.SuccessAsync(_localize[message]);
    }
}
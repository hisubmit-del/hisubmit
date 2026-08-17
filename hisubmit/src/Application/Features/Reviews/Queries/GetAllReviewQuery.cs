using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests;
using HiSubmit.Application.Specifications.Reviews;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Reviews.Queries;

public class GetAllReviewQuery : PagedRequest, IRequest<PaginatedResult<GetAllReviewResponse>>
{
    public int? FestivalId { get; set; }
    public new string SearchString { get; set; }
    public string UserId { get; set; }
    public  CommentType Type { get; set; }
}

public class GetAllReviewQueryHandler : IRequestHandler<GetAllReviewQuery, PaginatedResult<GetAllReviewResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<GetAllReviewQueryHandler> _localize;
    private readonly IUserService _userService;

    public GetAllReviewQueryHandler
    (IMapper mapper, IUnitOfWork<int> unitOfWork, IUserService userService,
        IStringLocalizer<GetAllReviewQueryHandler> localize)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localize = localize;
        _userService = userService;
    }

    public async Task<PaginatedResult<GetAllReviewResponse>> Handle(GetAllReviewQuery request,
        CancellationToken cancellationToken)
    {
        var festivalSpecify = new FestivalReviewSpecification(request.FestivalId);
        var userSpecify = new UserReviewSpecification(request.UserId);
        var response = await _unitOfWork.Repository<Review>()
            .Entities
            .Specify(userSpecify)
            .Specify(festivalSpecify)
            .Where(p => p.Type == request.Type &&
                        (request.Type != CommentType.Review ||
                         !string.IsNullOrWhiteSpace(p.Text)))
            .ProjectTo<GetAllReviewResponse>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        var userIds = response.Data
            .Select(p => p.UserId)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .ToList();
        var users = await _userService.GetAllAsync(userIds);
        foreach (var review in response.Data)
        {
            var user = users.Data.FirstOrDefault(p => p.Id == review.UserId);
            if (user == null) continue;
            review.UserFullName = user.FullName;
            review.UserImages = user.ProfilePictureDataUrl;
        }
        return response;
    }
}

public class GetAllReviewResponse
{
    public int Id { get; set; }
    public string UserFullName { get; set; }
    public string UserImages { get; set; }
    public int Rate { get; set; }
    public string Text { get; set; }
    public int FestivalId { get; set; }
    public  string FestivalName { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedOn { get; set; }
}

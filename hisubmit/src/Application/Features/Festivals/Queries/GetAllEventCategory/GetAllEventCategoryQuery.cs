using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllEventCategory
{
    public class GetAllEventCategoryQuery : IRequest<Result<List<GetAllEventCategoryResponse>>>
    {
        public int FestivalId { get; set; }
    }
    public class GetAllEventCategoryQueryHandler : IRequestHandler<GetAllEventCategoryQuery, Result<List<GetAllEventCategoryResponse>>>
    {
        private readonly IStringLocalizer<GetAllEventCategoryQueryHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _appCache;
        public GetAllEventCategoryQueryHandler(
            IStringLocalizer<GetAllEventCategoryQueryHandler> localizer,
            IUnitOfWork<int> unitOfWork, IMapper mapper,
            IAppCache appCache)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appCache = appCache;
        }
        public async Task<Result<List<GetAllEventCategoryResponse>>> Handle(GetAllEventCategoryQuery request, CancellationToken cancellationToken)
        {
            var getAllEventCategories = await _unitOfWork.Repository<EventCategory>().Entities
                .Include(p => p.DeadlineEventCategories)
            .Where(p => p.FestivalId == request.FestivalId).ToListAsync();
            //  var categories = await _appCache.GetOrAddAsync(ApplicationConstants.Cache.GetAllEventCategoryCacheKefy, getAllEventCategories);
            var mappedCategories = _mapper.Map<List<GetAllEventCategoryResponse>>(getAllEventCategories);
            return await Result<List<GetAllEventCategoryResponse>>.SuccessAsync(mappedCategories, _localizer["Success Doing"] + request.FestivalId.ToString());
        }
    }
}

using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.FestivalFocs.Queries.GetAllFestivalFocus
{
    public class GetAllFestivalFocusQuery:IRequest<Result<List<GetAllFestivalFocusResponse>>>
    {
    }
    internal class GetAllFestivalFocusQueryHandler : IRequestHandler<GetAllFestivalFocusQuery, Result<List<GetAllFestivalFocusResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllFestivalFocusQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllFestivalFocusResponse>>> Handle(GetAllFestivalFocusQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<FestivalFocus>>> getAllFestivalFocus = () => _unitOfWork.Repository<FestivalFocus>().GetAllAsync();
            var FestivalFocusList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllFestivalFocusCacheKey, getAllFestivalFocus);
            var mappedBrands = _mapper.Map<List<GetAllFestivalFocusResponse>>(FestivalFocusList);
            return await Result<List<GetAllFestivalFocusResponse>>.SuccessAsync(mappedBrands);
        }
    }
}

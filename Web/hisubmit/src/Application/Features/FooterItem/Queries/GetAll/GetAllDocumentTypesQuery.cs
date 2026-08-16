using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Misc;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Domain.Entities.Content;

namespace HiSubmit.Application.Features.FooterItems.Queries.GetAll
{
    public class GetAllFooterItemQuery : IRequest<Result<List<FooterItemDto>>>
    {
        public GetAllFooterItemQuery()
        {
        }
    }

    internal class GetAllFooterItemQueryHandler : IRequestHandler<GetAllFooterItemQuery, Result<List<FooterItemDto>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllFooterItemQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<FooterItemDto>>> Handle(GetAllFooterItemQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<MenuItem>>> getAllFooterItems = () => _unitOfWork.Repository<MenuItem>().GetAllAsync();
            var documentTypeList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllFooterItem, getAllFooterItems);
            var mappedDocumentTypes = _mapper.Map<List<FooterItemDto>>(documentTypeList);
            return await Result<List<FooterItemDto>>.SuccessAsync(mappedDocumentTypes);
        }
    }
}
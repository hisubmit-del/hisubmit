using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllFestivalFile
{
    public class GetAllFestivalFileQuery:IRequest<Result<List<GetAllFestivalFileResponse>>>
    {
        public int FestivalId { get; set; }
    }
    public class GetAllFestivalFileQueryHandler : IRequestHandler<GetAllFestivalFileQuery, Result<List<GetAllFestivalFileResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<GetAllFestivalFileQueryHandler> _localizer;
        public GetAllFestivalFileQueryHandler(
            IMapper mapper, IUnitOfWork<int> unitOfWork, 
            IStringLocalizer<GetAllFestivalFileQueryHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<List<GetAllFestivalFileResponse>>> Handle(GetAllFestivalFileQuery request, CancellationToken cancellationToken)
        {
            var allFiles =await _unitOfWork.Repository<FestivalFile>().Entities
                .Where(p => p.FestivalId == request.FestivalId)
                .ProjectTo<GetAllFestivalFileResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return await Result<List<GetAllFestivalFileResponse>>.SuccessAsync(allFiles);
        }

    }
}

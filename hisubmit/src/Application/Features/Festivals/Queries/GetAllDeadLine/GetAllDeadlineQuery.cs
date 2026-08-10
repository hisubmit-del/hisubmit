using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLine
{
    public class GetAllDeadlineQuery : IRequest<Result<List<GetAllDeadLineResponse>>>
    {
        public int FestivalId { get; set; }
        public bool? ApplyToAllCategory { get; set; }
    }

    public class GetAllDeadLineQueryHandler : IRequestHandler<GetAllDeadlineQuery, Result<List<GetAllDeadLineResponse>>>
    {
        private readonly IStringLocalizer<GetAllDeadLineQueryHandler> _localize;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _appCache;
        public GetAllDeadLineQueryHandler(
            IStringLocalizer<GetAllDeadLineQueryHandler> localize,
            IUnitOfWork<int> unitOfWork, IMapper mapper,
            IAppCache appCache)
        {
            _localize = localize;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appCache = appCache;
        }
        public async Task<Result<List<GetAllDeadLineResponse>>> Handle(GetAllDeadlineQuery request, CancellationToken cancellationToken)
        {
            IQueryable<DeadLine> query;
            if (request.ApplyToAllCategory == null)
            {
                query = _unitOfWork.Repository<DeadLine>().Entities
               .Where(p => p.FestivalId == request.FestivalId);
            }
            else
            {
                query = _unitOfWork.Repository<DeadLine>().Entities
              .Where(p => p.FestivalId == request.FestivalId && p.ApplyToAllCategory == request.ApplyToAllCategory);
            }
            var getallDeadLine = await query.ToListAsync();

            var mappedDeadLine = _mapper.Map<List<GetAllDeadLineResponse>>(getallDeadLine);
            return await Result<List<GetAllDeadLineResponse>>.SuccessAsync(mappedDeadLine, _localize["Success Doing"]);
        }
    }
}

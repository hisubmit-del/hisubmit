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
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace HiSubmit.Application.Features.Festivals.Queries.GetDetailById
{
    public class GetFestivalDetailByIdQuery : IRequest<Result<GetFestivalDetailResponse>>
    {
        public int FestivalId { get; set; }
        public bool WithInclude { get; set; }
        public  string FestivalUrl { get; set; }
    }
    public class GetFestivalDetailByIdQueryHandler :
        IRequestHandler<GetFestivalDetailByIdQuery, Result<GetFestivalDetailResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IAppCache _appCache;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IRepositoryAsync<Festival, int> _repository;
        private readonly IStringLocalizer<GetFestivalDetailByIdQueryHandler> _stringLocalizer;
        public GetFestivalDetailByIdQueryHandler(IMapper mapper,
            IAppCache appCache, IUnitOfWork<int> unitOfWork,
            IRepositoryAsync<Festival, int> repository,
            IStringLocalizer<GetFestivalDetailByIdQueryHandler> localizer)
        {
            _mapper = mapper;
            _appCache = appCache;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _stringLocalizer = localizer;
        }

        public async Task<Result<GetFestivalDetailResponse>> Handle(GetFestivalDetailByIdQuery request, CancellationToken cancellationToken)
        {

                var query = _repository.Entities;
                if (!string.IsNullOrWhiteSpace(request.FestivalUrl))
                {
                    query = query.Where(p => p.URL == request.FestivalUrl);
                }else if (request.FestivalId != 0)
                {
                    query = query.Where(p => p.Id == request.FestivalId);
                }
                else
                {
                    return await Result<GetFestivalDetailResponse>.FailAsync(_stringLocalizer["Festival Not Found"]);
                }

                if (request.WithInclude)
                {
                    query = query
                        .Include(p => p.Address)
                        .Include(p => p.SubmissionAddress)
                        .Include(p => p.FestivalArtCategories)
                        .Include(p => p.FestivalFestivalFoci)
                        .Include(p => p.FestivalFestivalQualifyings);
                }

                // var qualiId = await _unitOfWork.Repository<FestivalFestivalQualifying>()
                //     .Entities
                //     .Where(p => p.ProductFestivalId == request.ProductFestivalId)
                //     .Select(p => p.FestivalQualifyingId)
                //     .ToListAsync(cancellationToken);
                
                var result =
                    await query.ProjectTo<GetFestivalDetailResponse>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(cancellationToken);
               // result.QualifyersId = qualiId;
                
                return await Result<GetFestivalDetailResponse>
                    .SuccessAsync(result);
                }
    }
}

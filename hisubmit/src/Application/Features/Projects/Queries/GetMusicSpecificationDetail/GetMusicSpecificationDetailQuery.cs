using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
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

namespace HiSubmit.Application.Features.Projects.Queries.GetMusicSpecificationDetail
{
    public class GetMusicSpecificationDetailQuery:IRequest<Result<GetMusicSpecificationDetailResponse>>
    {
        public int ProjectId { get; set; }
    }

    public class GetMusicSpecificationDetailQueryHandler:IRequestHandler<GetMusicSpecificationDetailQuery, Result<GetMusicSpecificationDetailResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetMusicSpecificationDetailQueryHandler> _localizer;

        public GetMusicSpecificationDetailQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper,
            IStringLocalizer<GetMusicSpecificationDetailQueryHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<GetMusicSpecificationDetailResponse>> Handle(GetMusicSpecificationDetailQuery request, CancellationToken cancellationToken)
        {
            var specification = await _unitOfWork.Repository<MusicSpecification>().Entities
              .Where(p => p.ProjectId == request.ProjectId)
              .Include(p => p.ProjectType)
              .ProjectTo<GetMusicSpecificationDetailResponse>(_mapper.ConfigurationProvider)
              .FirstOrDefaultAsync();

            if (specification != null)
            {
                return await Result<GetMusicSpecificationDetailResponse>.SuccessAsync(specification);
            }
            else
            {
                var newSpec = new GetMusicSpecificationDetailResponse() { SubProjectTypeIds = new List<int>() };
                return await Result<GetMusicSpecificationDetailResponse>.SuccessAsync(newSpec);
            }
        }
    }
    public class GetMusicSpecificationDetailResponse
    {
        public int Id { get; set; }
        public List<int> SubProjectTypeIds { get; set; }
        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }
        public DateTime CompletionDate { get; set; }
        public int OriginCountryId { get; set; }

        public string Language { get; set; }

        public bool StudentProject { get; set; }

        //navigation Property
        public int ProjectId { get; set; }
        public string OriginCountryName { get; set; }

        public GetMusicSpecificationDetailResponse()
        {
            CompletionDate = DateTime.Today;
        }
    }
}

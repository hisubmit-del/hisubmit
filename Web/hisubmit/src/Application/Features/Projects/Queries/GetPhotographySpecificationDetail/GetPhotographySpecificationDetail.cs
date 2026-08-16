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
    public class GetPhotographySpecificationDetailQuery : IRequest<Result<GetPhotographySpecificationDetailResponse>>
    {
        public int ProjectId { get; set; }
    }

    public class GetPhotographySpecificationDetailQueryHelper : IRequestHandler<GetPhotographySpecificationDetailQuery, Result<GetPhotographySpecificationDetailResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetMusicSpecificationDetailQueryHandler> _localizer;

        public GetPhotographySpecificationDetailQueryHelper(IUnitOfWork<int> unitOfWork, IMapper mapper,
            IStringLocalizer<GetMusicSpecificationDetailQueryHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<GetPhotographySpecificationDetailResponse>> Handle(GetPhotographySpecificationDetailQuery request, CancellationToken cancellationToken)
        {
            var specification = await _unitOfWork.Repository<PhotographySpecification>().Entities
              .Where(p => p.ProjectId == request.ProjectId)
              .ProjectTo<GetPhotographySpecificationDetailResponse>(_mapper.ConfigurationProvider)
              .FirstOrDefaultAsync();

            if (specification != null)
            {
                return await Result<GetPhotographySpecificationDetailResponse>.SuccessAsync(specification);
            }
            else
            {
                var newSpec = new GetPhotographySpecificationDetailResponse();
                return await Result<GetPhotographySpecificationDetailResponse>.SuccessAsync(newSpec);
            }
        }
    }
    public class GetPhotographySpecificationDetailResponse
    {
        public int Id { get; set; }

        public string Genre { get; set; }
        public DateTime TakenDate { get; set; }
        public int OriginCountryId { get; set; }
        public string Camera { get; set; }
        public string Lens { get; set; }
        public string FocalLength { get; set; }
        public string ShutterSpeed { get; set; }
        public string Aperture { get; set; }
        public string Iso_Film { get; set; }
        public bool StudentProject { get; set; }

        //navigation propety
        public int ProjectId { get; set; }

        public List<int> SubProjectTypeIds { get; set; }
        public string OriginCountryName { get; set; }

        public GetPhotographySpecificationDetailResponse()
        {
            TakenDate = DateTime.Today;
        }
    }
}

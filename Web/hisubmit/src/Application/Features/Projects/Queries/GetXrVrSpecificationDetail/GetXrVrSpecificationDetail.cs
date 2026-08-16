using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Projects.Queries.GetVrXrSpecificationDetail
{
    public class GetVrXrSpecificationDetailQuery : IRequest<Result<GetVrXrSpecificationDetailResponse>>
    {
        public int ProjectId { get; set; }
    }

    public class GetVrXrSpecificationDetailQueryHandler : IRequestHandler<GetVrXrSpecificationDetailQuery, Result<GetVrXrSpecificationDetailResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetVrXrSpecificationDetailQueryHandler> _localizer;

        public GetVrXrSpecificationDetailQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper,
            IStringLocalizer<GetVrXrSpecificationDetailQueryHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<GetVrXrSpecificationDetailResponse>> Handle(GetVrXrSpecificationDetailQuery request, CancellationToken cancellationToken)
        {
            var specification = await _unitOfWork.Repository<XrVrSpecification>().Entities
                .Where(p => p.ProjectId == request.ProjectId)
                .Include(p => p.ProjectType)
                .ProjectTo<GetVrXrSpecificationDetailResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (specification != null)
            {
                return await Result<GetVrXrSpecificationDetailResponse>.SuccessAsync(specification);
            }
            else
            {
                var newSpec = new GetVrXrSpecificationDetailResponse() { SubProjectTypeIds = new List<int>() };
                return await Result<GetVrXrSpecificationDetailResponse>.SuccessAsync(newSpec);
            }
        }
    }

    public class GetVrXrSpecificationDetailResponse
    {
        public int Id { get; set; }
        public List<int> SubProjectTypeIds { get; set; }

        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }

        public bool VariableRunTime { get; set; }
        public string DescriptionRunTime { get; set; }
        public int MinRunTimeHours { get; set; }
        public int MinRunTimeMinutes { get; set; }
        public int MinRunTimeSecounds { get; set; }
        public int MaxTimeHours { get; set; }
        public int MaxTimeMinutes { get; set; }
        public int MaxTimeSecounds { get; set; }
        public int AvgTimeHours { get; set; }
        public int AvgTimeMinutes { get; set; }
        public int AvgTimeSecounds { get; set; }

        public DateTime CompletionDate { get; set; }
        public int ProductionBudget { get; set; }
        public int OriginCountryId { get; set; }

        public string Language { get; set; }
        public bool StudentProject { get; set; }


        //navigationProperty
        public int ProjectId { get; set; }
        public GetVrXrSpecificationDetailResponse()
        {
            CompletionDate = DateTime.Today;
        }
    }
}

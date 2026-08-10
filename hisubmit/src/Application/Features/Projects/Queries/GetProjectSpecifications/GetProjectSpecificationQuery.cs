using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Projects.Queries.GetProjectSpecifications
{
    public class GetProjectSpecificationQuery:IRequest<Result<GetProjectSpecificationResponse>>
    {
        public int Id { get; set; }
    }

    public class GetProjectSpecificationResponse
    {
        public bool StudentProject { get; set; }
        public ProjectType ProjectType { get; set; }
        public  int Size { get; set; }
    }

    public class GetProjectSpecificationsQueryHandler : IRequestHandler<GetProjectSpecificationQuery, Result<GetProjectSpecificationResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<GetProjectSpecificationsQueryHandler> _localizer;
        private readonly IMapper _mapper;
        public GetProjectSpecificationsQueryHandler(
            IUnitOfWork<int> unitOfWork, 
            IStringLocalizer<GetProjectSpecificationsQueryHandler> localizer,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _mapper = mapper;
        }

        public async Task<Result<GetProjectSpecificationResponse>> Handle(GetProjectSpecificationQuery request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(request.Id);

            var prjectSpecification = _mapper.Map<GetProjectSpecificationResponse>(project);
            return await Result<GetProjectSpecificationResponse>.SuccessAsync(prjectSpecification);
        }
    }
}

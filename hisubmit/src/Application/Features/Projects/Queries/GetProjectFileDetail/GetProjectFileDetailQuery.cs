using AutoMapper;
using HiSubmit.Application.Features.Products.Queries.GetProductImage;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Projects.Queries.GetProjectFileDetail
{
    public class GetProjectFileDetailQuery:IRequest<Result<GetProjectFileDetailResponse>>
    {
        public int Id { get; set; }
    }

    public class GetProjectFileDetailResponse
    {
        public int Id { get; set; }
    }

    public class GetProjectFileDetailQueryHandler : IRequestHandler<GetProjectFileDetailQuery, Result<GetProjectFileDetailResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitofWork;
        private readonly IStringLocalizer<GetProjectFileDetailQueryHandler> _localizer;
        public GetProjectFileDetailQueryHandler(
            IMapper mapper,
            IUnitOfWork<int> unitofWork,
            IStringLocalizer<GetProjectFileDetailQueryHandler> localizer)
        {
            _mapper = mapper;
            _unitofWork = unitofWork;
            _localizer = localizer;
        }

        public async Task<Result<GetProjectFileDetailResponse>> Handle(GetProjectFileDetailQuery request, CancellationToken cancellationToken)
        {
            var projectfile = await _unitofWork.Repository<ProjectFile>().GetByIdAsync(request.Id);
            if(projectfile != null)
            {
                var mappedProjectFile = _mapper.Map<GetProjectFileDetailResponse>(projectfile);
                return await Result<GetProjectFileDetailResponse>.SuccessAsync(mappedProjectFile);
            }
            else
            {
                return await Result<GetProjectFileDetailResponse>.FailAsync(_localizer["File not found"]);
            }
        }
    }
}

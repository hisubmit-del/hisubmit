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

namespace HiSubmit.Application.Features.Projects.Queries.GetFilmSpecificationDetail;

public class GetScriptSpecificationDetailQuery : IRequest<Result<GetScriptSpecificationDetailResponse>>
{
    public int ProjectId { get; set; }
}

public class GetScriptSpecificationDetailQueryHelper : IRequestHandler<GetScriptSpecificationDetailQuery, Result<GetScriptSpecificationDetailResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetFilmSpecificationDetailQueryHandler> _localizer;

    public GetScriptSpecificationDetailQueryHelper(IUnitOfWork<int> unitOfWork, IMapper mapper,
        IStringLocalizer<GetFilmSpecificationDetailQueryHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<Result<GetScriptSpecificationDetailResponse>> 
        Handle(GetScriptSpecificationDetailQuery request, CancellationToken cancellationToken)
    {
        var specification = await _unitOfWork.Repository<ScriptSpecification>().Entities
            .Where(p => p.ProjectId == request.ProjectId)
            .Include(p => p.ProjectTypes)
            .ProjectTo<GetScriptSpecificationDetailResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (specification != null)
        {
            return await Result<GetScriptSpecificationDetailResponse>.SuccessAsync(specification);
        }
        else
        {
            var newSpec = new GetScriptSpecificationDetailResponse
            {
                SubProjectTypeIds = new List<int>()
            };
            return await Result<GetScriptSpecificationDetailResponse>.SuccessAsync(newSpec);
        }
    }
}

public class GetScriptSpecificationDetailResponse
{
    public int Id { get; set; }
    public List<int> SubProjectTypeIds { get; set; }
    public string Genre { get; set; }
    public int NumberOfPage { get; set; }
    public int OriginCountryId { get; set; }
    public string Language { get; set; }
    public bool StudentProject { get; set; }
    public bool FirstTimeScreenWrite { get; set; }

    //navigation property
    public int ProjectId { get; set; }
    public string OriginCountryName { get; set; }
}
#nullable enable
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Catalog;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Projects.Queries.GetAllSubProjectType;

namespace HiSubmit.Application.Features.SubProjectTypes.Queries.GetAll;

public class GetAllSubProjectTypeQuery : IRequest<Result<List<GetAllSubProjectTypeResponse>>>
{
    public ProjectType? ProjectType { get; set; }
    public string? SubIdString { get; set; }
}

public class GetAllSubProjectTypeQueryHandler : IRequestHandler<GetAllSubProjectTypeQuery,
        Result<List<GetAllSubProjectTypeResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSubProjectTypeQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<GetAllSubProjectTypeResponse>>> Handle(GetAllSubProjectTypeQuery request,
        CancellationToken cancellationToken)
    {
        var subIds = request.SubIdString == null
            ? new List<int>()
            : request.SubIdString.Split("-").Select(int.Parse).ToList();
        
        var spec = new SubProjectTypeFilterSpecification(request.ProjectType, subIds);
        var types = await _unitOfWork.Repository<SubProjectType>()
            .Entities
            .Specify(spec)
            .ProjectTo<GetAllSubProjectTypeResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return await Result<List<GetAllSubProjectTypeResponse>>.SuccessAsync(types);
    }
}
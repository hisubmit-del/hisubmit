using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Projects;
using HiSubmit.Application.Specifications.Submits;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Projects.Queries.GetAllSubProjectType;

public class GetAllSubProjectSelectedTypeQuery:IRequest<IResult<List<GetAllSubProjectTypeResponse>>>
{
    public int ProjectId { get; set; }
    public  ProjectType ProjectType { get; set; }
}

public  class  GetAllSubProjectTypeQueryHandler:IRequestHandler<GetAllSubProjectSelectedTypeQuery,IResult<List<GetAllSubProjectTypeResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSubProjectTypeQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IResult<List<GetAllSubProjectTypeResponse>>> Handle(GetAllSubProjectSelectedTypeQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // IQueryable<List<SubProjectSpecficationDto>> command;
        // switch (request.ProjectType)
        // {
        //     case ProjectType.Film:
        //         command = _unitOfWork.Repository<FilmSpecification>().Entities
        //             .Where(p => p.ProjectId == request.ProjectId)
        //             .Select(p => p.ProjectTypes)
        //             .ProjectTo<List<SubProjectSpecficationDto>>(_mapper.ConfigurationProvider);
        //         break;
        //     case ProjectType.Photography:
        //         command = _unitOfWork.Repository<PhotographySpecification>().Entities
        //             .Where(p => p.ProjectId == request.ProjectId)
        //             .Select(p => p.PhotographySpecificationSubProjectTypes)
        //             .ProjectTo<List<SubProjectSpecficationDto>>(_mapper.ConfigurationProvider);
        //         break;
        //     case ProjectType.Music:
        //         command = _unitOfWork.Repository<MusicSpecification>().Entities
        //             .Where(p => p.ProjectId == request.ProjectId)
        //             .Select(p => p.ProjectType)
        //             .ProjectTo<List<SubProjectSpecficationDto>>(_mapper.ConfigurationProvider);
        //         break;
        //     case ProjectType.Script_ScreenWriting:
        //         command = _unitOfWork.Repository<ScriptSpecification>().Entities
        //             .Where(p => p.ProjectId == request.ProjectId)
        //             .Select(p => p.ProjectTypes) 
        //             .ProjectTo<List<SubProjectSpecficationDto>>(_mapper.ConfigurationProvider);
        //         break;
        //     case ProjectType.VR_XR:
        //         command = _unitOfWork.Repository<XrVrSpecification>().Entities
        //             .Where(p => p.ProjectId == request.ProjectId)
        //             .Select(p => p.ProjectType)
        //             .ProjectTo<List<SubProjectSpecficationDto>>(_mapper.ConfigurationProvider);
        //             ;
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
        //
        // var f =(await command.FirstOrDefaultAsync(cancellationToken)).Select(p=>p.ProjectTypeId).ToList();
        //
        // List<GetAllSubProjectTypeResponse> response = new();
        // response = await _unitOfWork.Repository<SubProjectType>()
        //         .Entities.Where(p => f.Any(k => k == p.Id))
        //         .ProjectTo<GetAllSubProjectTypeResponse>(_mapper.ConfigurationProvider)
        //         .ToListAsync(cancellationToken);
        //
        // return await Result<List<GetAllSubProjectTypeResponse>>.SuccessAsync(response);
    }
}

public class GetAllSubProjectTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class SubProjectSpecficationDto
{
    public int ProjectTypeId { get; set; }
}

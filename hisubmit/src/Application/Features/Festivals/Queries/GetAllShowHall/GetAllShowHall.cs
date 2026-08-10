using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Festivals.Commands.AddEditShowHall;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllShowHall;

public class GetAllShowHallQuery:IRequest<IResult<List<GetAllShowHallResponse>>>
{
    public int VenueId { get; set; }
}

public class GetAllShowHallQueryHandler : IRequestHandler<GetAllShowHallQuery, IResult<List<GetAllShowHallResponse>>>
{
    private readonly IMapper _mapper;
    private IUnitOfWork<int> _unitOfWork;

    public GetAllShowHallQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult<List<GetAllShowHallResponse>>> Handle(GetAllShowHallQuery request, CancellationToken cancellationToken)
    {
        var showHalls = await _unitOfWork.Repository<ShowHall>()
            .Entities.Where(p => p.VenueId == request.VenueId)
            .Include(p=>p.ShowTimes)
            .ProjectTo<GetAllShowHallResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<List<GetAllShowHallResponse>>.SuccessAsync(showHalls);
    }
}

public class GetAllShowHallResponse
{
    public  int Id { get; set; }
    public  string Title { get; set; }
    public  int Capacity { get; set; }
    public  int AvailableCapacity { get; set; }
        
    public  int VenueId { get; set; }
        
    public  List<ShowTimeDto> ShowTimes { get; set; }

    public GetAllShowHallResponse()
    {
        ShowTimes = new List<ShowTimeDto>();
    }
}

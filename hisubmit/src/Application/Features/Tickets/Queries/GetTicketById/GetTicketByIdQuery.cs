using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetTicketById;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQuery:IRequest<Result<GetTicketByIdResponse>>
{
    public  int Id { get; set; }
    public  int FestivalId { get; set; }
}


public  class GetTicketByIdQueryHandler:IRequestHandler<GetTicketByIdQuery,Result<GetTicketByIdResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<GetTicketByIdQueryHandler> _localizer;

    public GetTicketByIdQueryHandler
        (IMapper mapper, IUnitOfWork<int> unitOfWork, IStringLocalizer<GetTicketByIdQueryHandler> localizer)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }
    public async Task<Result<GetTicketByIdResponse>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Repository<Ticket>()
            .Entities
            .Where(p => p.Id == request.Id &&
                        p.Venue != null &&
                        p.Venue.FestivalId == request.FestivalId)
            .Include(p => p.ShowTimeTickets)
            .ProjectTo<GetTicketByIdResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (ticket != null)
        {
            return await Result<GetTicketByIdResponse>.SuccessAsync(ticket);
        }

        return await Result<GetTicketByIdResponse>.FailAsync(_localizer["The  ticket not found"]);
    }
}


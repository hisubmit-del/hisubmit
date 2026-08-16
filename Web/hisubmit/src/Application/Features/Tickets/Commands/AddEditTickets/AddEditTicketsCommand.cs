using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.Tickets.AddTickets;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VenueType = HiSubmit.Domain.Enums.VenueType;

namespace HiSubmit.Application.Features.Tickets.Commands.AddEditTickets;

public class AddEditTicketsCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public int FestivalId { get; set; }
    public string Title { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public bool AddManagerPercentage { get; set; }
    public int Cost { get; set; }

    public List<AddEditSubmissionQuestionCommand> SubmissionQuestions { get; set; }


    public DateTime? EventDate { get; set; }

    //Capacity
    public int Capacity { get; set; }

    public string Description { get; set; }


    public int VenueId { get; set; }

    public List<int> ShowHallId { get; set; }

    public HashSet<int> ShowTimesId { get; set; }



    public AddEditTicketsCommand()
    {
        SubmissionQuestions = new List<AddEditSubmissionQuestionCommand>();
        ShowHallId = new List<int>();
        ShowTimesId = new HashSet<int>();
    }
}

public class AddEditTicketsCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IMediator mediator,
    ICurrentUserService currentUserService,
    IStringLocalizer<AddEditTicketsCommandHandler> localizer)
    : IRequestHandler<AddEditTicketsCommand, IResult>
{
    public async Task<IResult> Handle(AddEditTicketsCommand request, CancellationToken cancellationToken)
    {
        var userInRoleAdmin = currentUserService.IsInRole(RoleConstants.AdministratorRole);
        var venue = await unitOfWork.Repository<Venue>().GetByIdAsync(request.VenueId);
        if (venue == null)
        {
            return await Result.FailAsync(localizer["Venue not found"]);
        }

        var festival = await unitOfWork.Repository<Festival>().GetByIdAsync(venue.FestivalId);

        if (festival.EventEndDate < request.CloseDate)
        {
            return await Result.FailAsync("The ticket sale dates must fall within the festival duration.");
        }
        if (request.Id == 0)
        {
            var festivalName = await unitOfWork.Repository<Festival>()
                .Entities
                .Where(p => p.Id == venue.FestivalId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(cancellationToken);
            var mappedTicket = mapper.Map<Ticket>(request);
            mappedTicket.TicketType = venue.VenueType == VenueType.ShowLocation ? TicketType.Ticket : TicketType.Badge;
            mappedTicket.ShowTimeTickets = request.ShowTimesId
                .Select(id => new ShowTimeTicket() { ShowTimeId = id })
                .ToList();
            mappedTicket.AvailableCapacity = request.Capacity;
            mappedTicket.IsEnable = userInRoleAdmin;
            await unitOfWork.Repository<Ticket>().AddAsync(mappedTicket);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await mediator.Publish(new AddedTicketEvent
            {
                FestivalId = venue.FestivalId,
                FestivalName = festivalName,
                TicketId = mappedTicket.Id
            }, cancellationToken);
            return await Result.SuccessAsync(localizer["The ticket was created successfully"]);
        }

        var dbTicket = await unitOfWork.Repository<Ticket>().GetByIdAsync(request.Id);
        if (dbTicket == null) return await Result.FailAsync(localizer["Ticket not found"]);
        var updatedTicket = mapper.Map(request, dbTicket);
        updatedTicket.TicketType = venue.VenueType == VenueType.ShowLocation ? TicketType.Ticket : TicketType.Badge;
        updatedTicket.AvailableCapacity = request.Capacity;
        updatedTicket.IsEnable = userInRoleAdmin;
        await unitOfWork.Repository<Ticket>().UpdateAsync(updatedTicket);
        await UpdateShowTimes(request.ShowTimesId, request.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localizer["The ticket has been successfully updated"]);
    }

    private async Task UpdateShowTimes(IReadOnlyCollection<int> clientShowTimesId, int ticketId)
    {
        var dbShowTimes = await unitOfWork.Repository<ShowTimeTicket>()
            .Entities.Where(p => p.TicketId == ticketId)
            .ToListAsync();
        var dbShowTimesId = dbShowTimes.Select(p => p.Id);
        var addedIds = clientShowTimesId.Where(clId => dbShowTimesId.All(dbId => dbId != clId));
        var deletedTicketShowTimes = dbShowTimes.Where(dbShoT => clientShowTimesId.All(clId => clId != dbShoT.Id));
        foreach (var id in addedIds)
        {
            await unitOfWork.Repository<ShowTimeTicket>()
                 .AddAsync(new ShowTimeTicket() { ShowTimeId = id, TicketId = ticketId });
        }

        foreach (var showTimeTicket in deletedTicketShowTimes)
        {
            await unitOfWork.Repository<ShowTimeTicket>()
                .DeleteAsync(showTimeTicket);
        }
    }
}

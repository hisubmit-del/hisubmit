using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Validators.Features.Tickets;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SoldTicketStatus = HiSubmit.Domain.Enums.SoldTicketStatus;

namespace HiSubmit.Application.Features.SoldTickets.Queries;

public class GetAllSoldTicketQuery:PagedRequest,IRequest<PaginatedResult<GetAllSoldTicketResponse>>
{
    public  string UserId { get; set; }
    public  int? FestivalId { get; set; }
    public  int? TicketId { get; set; }
    public  int? VenueId { get; set; }
    public  string SearchString { get; set; }
    public  SoldTicketStatus? SoldTicketStatus { get; set; }
}

public class GetAllSoldTicketQueryHandler(
        IUnitOfWork<int> unitOfWork,
        IMapper mapper,
        IStringLocalizer<GetAllSoldTicketQueryHandler> localizer,
        IUserService userService)
    : IRequestHandler<GetAllSoldTicketQuery, PaginatedResult<GetAllSoldTicketResponse>>
{
    private readonly IStringLocalizer<GetAllSoldTicketQueryHandler> _localizer = localizer;

    public async Task<PaginatedResult<GetAllSoldTicketResponse>> Handle(GetAllSoldTicketQuery request, CancellationToken cancellationToken)
    {
        var specification =
            new GetAllSoldTicketFilter(request.FestivalId, request.VenueId, request.TicketId, request.UserId,request.SearchString,request.SoldTicketStatus);

        var response = await unitOfWork.Repository<SoldTicket>()
            .Entities
            .Include(p=>p.Ticket).ThenInclude(p=>p.Venue)
            .Specify(specification)
            .ProjectTo<GetAllSoldTicketResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        var users =await userService
            .GetAllAsync(response.Data.Select(p=>p.UserId).ToList());

        foreach (var soldTicket in response.Data)
        {
            var buyerUser = users.Data.FirstOrDefault(p => p.Id == soldTicket.UserId);
            soldTicket.BuyerFullNameName = buyerUser.FullName;
            soldTicket.BuyerEmail = buyerUser.Email;
        }
        return response;
    }
}

public class GetAllSoldTicketResponse
{
    public  int Id { get; set; }
    public string TicketTitle { get; set; }
    public  TicketType TicketType { get; set; }
    public decimal Cost { get; set; }
    public  string BuyerFullNameName { get; set; }
    public  string BuyerEmail { get; set; }
    public  int Count { get; set; }
    public decimal ShareFestivalIncome { get; set; }
    public  string UserId { get; set; }
    public  int TicketId { get; set; }
    public DateTime? CreatedOn { get; set; }
    public  int? ShowTimeId { get; set; }
    public  bool ForOtherUser { get; set; }
    public  string OtherUserEmail { get; set; }
    public  int? ChairNumber { get; set; }
    public  SoldTicketStatus SoldTicketStatus { get; set; }
}
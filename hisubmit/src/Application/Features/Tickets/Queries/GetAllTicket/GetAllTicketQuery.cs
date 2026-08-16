using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Tickets;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VenueType = HiSubmit.Domain.Enums.VenueType;

namespace HiSubmit.Application.Features.Tickets.Queries.GetAllTicket;

public class GetAllTicketQuery:PagedRequest, IRequest<PaginatedResult<GetAllTicketResponse>>
{
    public int? FestivalId { get; set; }
    public  TicketType? TicketType { get; set; }
    public  string SearchString { get; set; }
    public  bool? GetActiveTicket { get; set; }
    public  bool? IsEnable { get; set; }
}

public class GetAllTicketQueryHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IStringLocalizer<GetAllTicketQueryHandler> localizer)
    : IRequestHandler<GetAllTicketQuery, PaginatedResult<GetAllTicketResponse>>
{
    private readonly IStringLocalizer<GetAllTicketQueryHandler> _localizer = localizer;

    public async Task<PaginatedResult<GetAllTicketResponse>> Handle(GetAllTicketQuery request, CancellationToken cancellationToken)
    {
        //var specificationFestivalTicket = new GetAllFestivalTicketSpecification(request.ProductFestivalId);
        var specificationGetAllFilter
            = new GetAllTicketFilterSpecification
                (request.GetActiveTicket,request.FestivalId,request.IsEnable,request.TicketType);
        var result =await unitOfWork.Repository<Ticket>()
            .Entities
            .Specify(specificationGetAllFilter)
            .ProjectTo<GetAllTicketResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        var venueId = (result.Data ?? Enumerable.Empty<GetAllTicketResponse>())
            .Where(p => p.VenueId.HasValue)
            .Select(p => p.VenueId.Value)
            .ToList();
        var addresses = unitOfWork.Repository<Address>()
            .Entities.Where(p => p.VenueId.HasValue && venueId.Contains(p.VenueId.Value))
            .Include(p => p.Country);

        foreach (var ticket in result.Data ?? Enumerable.Empty<GetAllTicketResponse>())
        {
            var address = addresses.FirstOrDefault(p => p.VenueId == ticket.VenueId);
            if (address != null)
            {
                ticket.VenueAddress = string.Join("-", new[]
                {
                    address.Country?.Name,
                    address.State,
                    address.City,
                    address.Text
                }.Where(value => !string.IsNullOrWhiteSpace(value)));
            }
        }

        return result;
    }
}

public class GetAllTicketResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public  string Description { get; set; }
    public DateTime OpenDate { get; set; }
    public  DateTime CloseDate { get; set; }
    public  bool AddManagerPercentage { get; set; }
    public  int Cost { get; set; }
    
    public  DateTime EventDate { get; set; }
    
    public  bool IsEnable { get; set; }
    public  int? VenueId { get; set; }
    public  string VenueName { get; set; }
    public  VenueType VenueVenueType { get; set; }
    
    public  string VenueAddress { get; set; }
    
    public  TicketType TicketType { get; set; }
    public  int AvailableCapacity { get; set; }
}

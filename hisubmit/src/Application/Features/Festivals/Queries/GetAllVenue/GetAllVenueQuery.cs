using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllVenue
{
    public class GetAllVenueQuery : PagedRequest, IRequest<PaginatedResult<GetAllVenueResponse>>
    {
        public int FestivalId { get; set; }

        public string SearchString { get; set; }
    }

    public class GetAllVenueQueryHandler(
        IStringLocalizer<GetAllVenueQueryHandler> localizer,
        IUnitOfWork<int> unitOfWork,
        IMapper mapper)
        : IRequestHandler<GetAllVenueQuery, PaginatedResult<GetAllVenueResponse>>
    {
        private readonly IStringLocalizer<GetAllVenueQueryHandler> _localizer = localizer;

        public async Task<PaginatedResult<GetAllVenueResponse>> Handle(GetAllVenueQuery request,
            CancellationToken cancellationToken)
        {
            var venues = await unitOfWork.Repository<Venue>().Entities
                .Include(p => p.Address)
                .Where(p => p.FestivalId == request.FestivalId)
                .ProjectTo<GetAllVenueResponse>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request);

            return venues;
        }
    }
}
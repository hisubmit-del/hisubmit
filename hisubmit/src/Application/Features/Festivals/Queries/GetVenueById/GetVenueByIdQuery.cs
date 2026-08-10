using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Queries.GetVenueById
{
    public class GetVenueByIdQuery : IRequest<Result<GetVenueByIdResponse>>
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
    }

    public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, Result<GetVenueByIdResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetVenueByIdQueryHandler> _localizer;
        private readonly IRepositoryAsync<Venue, int> _repository;

        public GetVenueByIdQueryHandler
        (IMapper mapper,
            IStringLocalizer<GetVenueByIdQueryHandler> localizer,
            IRepositoryAsync<Venue, int> repositoryAsync)
        {
            _mapper = mapper;
            _localizer = localizer;
            _repository = repositoryAsync;
        }

        public async Task<Result<GetVenueByIdResponse>> Handle(GetVenueByIdQuery request,
            CancellationToken cancellationToken)
        {
            var venue = await _repository.Entities.Include(p => p.Address)
                .Include(p => p.ShowHalls).ThenInclude(p => p.ShowTimes)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (venue == null) return await Result<GetVenueByIdResponse>.FailAsync("Venue not found");
            
            var mappedVenue = _mapper.Map<GetVenueByIdResponse>(venue);
            return await Result<GetVenueByIdResponse>.SuccessAsync(mappedVenue);
        }
    }
}
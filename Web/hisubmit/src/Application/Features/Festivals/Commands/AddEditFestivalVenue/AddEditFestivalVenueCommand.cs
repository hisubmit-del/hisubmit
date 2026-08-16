using AutoMapper;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
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
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalVenue
{
    public class AddEditFestivalVenueCommand : IRequest<Result<int>>
    {
        public int FestivalId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public VenueType VenueType { get; set; }
        public AddEditAddressCommand Address { get; set; }

        public AddEditFestivalVenueCommand()
        {
            Address = new AddEditAddressCommand();
        }
    }

    public class AddEditFestivalVenueCommandHandler : IRequestHandler<AddEditFestivalVenueCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditFestivalVenueCommandHandler> _localizer;
        private readonly IMapper _mapper;
        public AddEditFestivalVenueCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditFestivalVenueCommandHandler> localizer, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditFestivalVenueCommand request, CancellationToken cancellationToken)
        {
            var foundFestival = await _unitOfWork.Repository<Festival>().Entities.AnyAsync(p => p.Id == request.FestivalId);
            if (foundFestival)
            {
                if (request.Id == 0)
                {
                    var venue = _mapper.Map<Venue>(request);
                    await _unitOfWork.Repository<Venue>().AddAsync(venue);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllVenueCacheKey);
                    return await Result<int>.SuccessAsync(venue.Id, _localizer["Venue Added"]); ;
                }
                else
                {
                    var venue = await _unitOfWork.Repository<Venue>().GetByIdAsync(request.Id);
                    if (venue != null)
                    {
                        var updatedVenue = _mapper.Map(request, venue);
                        await _unitOfWork.Repository<Venue>().UpdateAsync(updatedVenue);
                        await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllVenueCacheKey);
                        return await Result<int>.SuccessAsync(venue.Id, _localizer["Venue Updated"]); ;
                    }
                    else
                    {
                        return await Result<int>.FailAsync(_localizer["Venue  Not Found"]);
                    }

                }
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Festival Not Found"]);
            }

        }
    }
}

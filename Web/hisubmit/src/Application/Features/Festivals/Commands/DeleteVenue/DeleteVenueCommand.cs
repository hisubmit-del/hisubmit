using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Locations;
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

namespace HiSubmit.Application.Features.Festivals.Commands.DeleteVenue
{
    public class DeleteVenueCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
    }
    public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteVenueCommandHandler> _localizer;
        public DeleteVenueCommandHandler(IUnitOfWork<int> unitOfWork,
            IStringLocalizer<DeleteVenueCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
        {
            var venue = await _unitOfWork.Repository<Venue>().GetByIdAsync(request.Id);
            if (venue != null)
            {
                await _unitOfWork.Repository<Venue>().DeleteAsync(venue);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllVenueCacheKey);
                return await Result<int>.SuccessAsync(venue.Id, _localizer["Venue Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Venue Not Found!"]);
            }
        }
    }
}

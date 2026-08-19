using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Festivals.FestivalReleasedRequests;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Festivals.Commands.ReleaseFestival
{
    public class ReleaseFestivalCommand : IRequest<IResult>
    {
        public int FestivalId { get; set; }
    }

    public class ReleaseFestivalCommandHandler : IRequestHandler<ReleaseFestivalCommand, IResult>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<ReleaseFestivalCommandHandler> _localize;

        public ReleaseFestivalCommandHandler
        (IUnitOfWork<int> unitOfWork, IMediator mediator,
            IStringLocalizer<ReleaseFestivalCommandHandler> localize)
        {
            _mediator = mediator;
            _localize = localize;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(ReleaseFestivalCommand request, CancellationToken cancellationToken)
        {
            var festival = await _unitOfWork.Repository<Festival>()
                .Entities.Include(p => p.EventOrginizers)
                .Include(p => p.EventCategories)
                    .ThenInclude(p => p.DeadlineEventCategories)
                .Include(p => p.Images)
                .Include(p => p.DeadLines)
                .Include(p => p.Venues)
                .FirstOrDefaultAsync(p => p.Id == request.FestivalId, cancellationToken);

            if (festival == null)
            {
                return await Result.FailAsync(_localize["Festival not found"]);
            }

            // if (festival.FestivalStatus == FestivalStatus.UnderInvestigation ||
            //     DateTime.Now.Between(festival.EventStartDate, festival.EventEndDate))
            // {
            //     return await Result.FailAsync(_localize["This function is  unavailable in this time "]);
            // };
            var messages = new List<string>();
            if (string.IsNullOrWhiteSpace(festival.LogoURL))
                messages.Add(_localize["To publish the festival, you must upload a logo"]);
            if (string.IsNullOrWhiteSpace(festival.Description))
                messages.Add(_localize["To publish the festival, you must enter a description"]);
            if (string.IsNullOrWhiteSpace(festival.Rules))
                messages.Add(_localize["To publish the festival, you must provide the rules"]);
            if (string.IsNullOrWhiteSpace(festival.Email))
                messages.Add(_localize["To publish the festival, you must enter a email"]);
            if (string.IsNullOrWhiteSpace(festival.WebSite))
                messages.Add(_localize["To publish the festival, you must enter a website"]);
            if (festival.OpeningDate == null)
                messages.Add(_localize["To publish the festival, you must enter the registration opening date"]);
            if (festival.EventStartDate == null || festival.EventEndDate == null)
                messages.Add(_localize["To publish the festival, you must enter the event start and end dates"]);
            if (string.IsNullOrWhiteSpace(festival.Prefix))
                messages.Add("To publish the festival, you must enter a prefix for submition code");
            if (festival.EventOrginizers == null || !festival.EventOrginizers.Any())
                messages.Add(_localize[" Event Organizer  must have at least one member"]);            
            if (festival.NotificationDate == null)
                messages.Add(_localize["The deadlines section has not been completed"]);            
            if (festival.DeadLines == null || !festival.DeadLines.Any())
                messages.Add(_localize["At least one submission deadline is required"]);
            if (festival.EventCategories == null || !festival.EventCategories.Any())
                messages.Add(_localize["The Event Categories section has not been completed"]);            
            else
            {
                if (festival.EventCategories.Any(category =>
                        category.DeadlineEventCategories == null ||
                        !category.DeadlineEventCategories.Any()))
                {
                    messages.Add(_localize[
                        "Each category must be assigned to at least one submission deadline"]);
                }

                if (festival.EventCategories
                    .Where(category => category.DeadlineEventCategories != null)
                    .SelectMany(category => category.DeadlineEventCategories)
                    .Any(deadlineCategory => deadlineCategory.StandardFee == null))
                {
                    messages.Add(_localize[
                        "Every category deadline must have a standard fee. Use 0 for a free submission"]);
                }
            }
            if (!festival.OnlineEvent && (festival.Venues == null || !festival.Venues.Any()))
                messages.Add(_localize["At least one event venue is required for an offline festival"]);
            if (festival.Images.All(p => p.ImageType != ImageType.Cover))
                messages.Add(_localize["Your festival dont have a Cover"]);
            
            if (messages.Any())
                return await Result.FailAsync(messages);

            festival.FestivalStatus = FestivalStatus.UnderInvestigation;
            await _unitOfWork.Repository<Festival>().UpdateAsync(festival);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _mediator.Publish(new FestivalRequestedReleased() { FestivalId = festival.Id }, cancellationToken);
            return await Result.SuccessAsync(_localize["Your festival is being reviewed"]);
        }
    }
}

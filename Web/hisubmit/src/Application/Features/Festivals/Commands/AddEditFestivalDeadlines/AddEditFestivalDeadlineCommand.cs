using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.FestivalChangeDeadLine;
using HiSubmit.Application.Events.FestivalChangeDeadLine.Handlers;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalDeadlines
{
    public class AddEditFestivalDeadlineCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? NotificationDate { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; } 
        public  FestivalStatus FestivalStatus { get; set; }
        public bool ChangesNotAllowed { get; set; }
        public AddEditFestivalDeadlineCommand()
        {
            OpeningDate = DateTime.Now;
        }
    }
    public class AddEditFestivalDeadLineCommandHandler : IRequestHandler<AddEditFestivalDeadlineCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<AddEditFestivalDeadLineCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        
        public AddEditFestivalDeadLineCommandHandler(
            IMapper mapper, IMediator mediator,
            IStringLocalizer<AddEditFestivalDeadLineCommandHandler> localizer,
            IUnitOfWork<int> unitOfWork)
        {
            _mapper = mapper;
            _mediator = mediator;
            _localizer = localizer;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<int>> Handle(AddEditFestivalDeadlineCommand request, CancellationToken cancellationToken)
        {
            var festival = await _unitOfWork.Repository<Festival>().GetByIdAsync(request.Id);
            if (festival != null)
            {
                var updatedFestival = _mapper.Map(request, festival);
               
                await _unitOfWork.Repository<Festival>().UpdateAsync(updatedFestival);
                await _mediator.Publish(new FestivalChangeDeadlineEvent(){Festival = updatedFestival},cancellationToken);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalCacheKey);
                
                return await Result<int>.SuccessAsync(updatedFestival.Id, _localizer["Festival Updated"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Festival Not Found"]);
            }
        }
    }
}

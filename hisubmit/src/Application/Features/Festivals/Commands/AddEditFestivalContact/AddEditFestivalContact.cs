using AutoMapper;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
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
using HiSubmit.Application.Events.Festivals.FestivalReleasedRequests;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalContact
{
    public class AddEditFestivalContactCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        //Contact
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public AddEditAddressCommand Address { get; set; }

        //Social Media
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string WhatsAppNumber { get; set; }
        public string Telegram { get; set; }
        public string Youtube { get; set; }

        //Submission Address
        public bool SeparateSubmissiionAddress { get; set; }
        public AddEditAddressCommand SubmissionAddress { get; set; }

        public bool OnlineEvent { get; set; }

        public bool ChangesNotAllowed { get; set; }
        public  FestivalStatus FestivalStatus { get; set; }

        public AddEditFestivalContactCommand()
        {
            Address = new AddEditAddressCommand();
            SubmissionAddress = new AddEditAddressCommand();
        }
    }

    public class AddEditFestivalContactCommandHandler(
        IMapper mapper,
        IMediator mediator,
        IStringLocalizer<AddEditFestivalContactCommandHandler> localizer,
        IUnitOfWork<int> unitOfWork)
        : IRequestHandler<AddEditFestivalContactCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(AddEditFestivalContactCommand request, CancellationToken cancellationToken)
        {
            var festival = await unitOfWork.Repository<Festival>().GetByIdAsync(request.Id);
            if (festival != null)
            {
                var updatedFestival = mapper.Map(request,festival);
                if (!request.SeparateSubmissiionAddress)
                {
                    updatedFestival.SubmissionAddress = null;
                }
                
                //check potential edit
                if (festival.FestivalStatus == FestivalStatus.Confirmed
                    && (festival.WebSite != request.WebSite || festival.Email != request.Email ))
                {
                    festival.FestivalStatus = FestivalStatus.UnderInvestigation;
                    await mediator.Publish(new FestivalRequestedReleased()
                    {
                        FestivalId = festival.Id
                    },cancellationToken:cancellationToken);
                }
                
                await unitOfWork.Repository<Festival>().UpdateAsync(updatedFestival);
                await unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalCacheKey);
            
                return await Result<int>.SuccessAsync(updatedFestival.Id, localizer["Festival Updated"]);
            }
            else
            {
                return await Result<int>.FailAsync(localizer["Festival Not Found"]);
            }
        }
    }
}

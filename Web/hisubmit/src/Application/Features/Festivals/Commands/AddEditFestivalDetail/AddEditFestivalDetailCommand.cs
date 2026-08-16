using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Festivals.FestivalReleasedRequests;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Application.Features.Festivals.Commands.CreateFestival
{
    public class AddEditFestivalDetailCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        public int YearsRunning { get; set; }
        public List<EventType> EventTypes { get; set; }
        public string Description { get; set; }
        public string LogoURL { get; set; }
        public string Rewards { get; set; }
        public string RewardsName { get; set; }
        public string RewardLogoURL { get; set; }
        public string Rules { get; set; }
        public int AudienceAttendence { get; set; }
        public int EstimatedSubmissions { get; set; }
        public int ProjectsSelected { get; set; }
        public int AwardsPresented { get; set; }

        public bool FilmFestival { get; set; }
        public bool ScreenWritingWriter { get; set; }
        public bool MusicContest { get; set; }
        public bool PhotographicContest { get; set; }
        public bool OnlineFestival { get; set; }
        public bool ArtFestival { get; set; }

        public List<string> QualifyersId { get; set; }

        public UploadRequest UploadRequest { get; set; }
        public UploadRequest RewardLogoUploadRequest { get; set; }

        public FestivalStatus FestivalStatus { get; set; }

        public bool ChangesNotAllowed { get; set; }

        public UploadRequest ApprovedLicenseUploadRequest { get; set; }

        public string ApprovedLicenseURL { get; set; }

        public AddEditFestivalDetailCommand()
        {
            QualifyersId = new List<string>();
        }
    }

    public class CreateFestivalCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IMapper mapper,
        IMediator mediator,
        IStringLocalizer<CreateFestivalCommandHandler> localizer,
        ICurrentUserService currentUserService,
        IUploadService uploadService)
        : IRequestHandler<AddEditFestivalDetailCommand, Result<int>>
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Result<int>> Handle(AddEditFestivalDetailCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                var festival = mapper.Map<Festival>(request);
                if (request.UploadRequest.Data != null)
                {
                    festival.LogoURL = uploadService.UploadAsync(request.UploadRequest);
                }

                await unitOfWork.Repository<Festival>().AddAsync(festival);
                await unitOfWork.CommitAndRemoveCache(cancellationToken,
                    ApplicationConstants.Cache.GetAllFestivalCacheKey);
                return await Result<int>.SuccessAsync(festival.Id, localizer["Festival Saved"]);
            }
            else
            {
                var festival = await unitOfWork.Repository<Festival>().GetByIdAsync(request.Id);
                if (festival != null)
                {
                    if (string.IsNullOrWhiteSpace(festival.URL))
                    {
                        festival.URL = $"{festival.Name.Trim()}";
                    }

                   


                    var newFestival = mapper.Map(request, festival);
                    newFestival.LogoURL = UpdateLogoUrl
                        (festival.LogoURL, request.LogoURL, request.UploadRequest);

                    newFestival.RewardLogoURL = UpdateRewardLogoURL
                        (festival.RewardLogoURL, request.RewardLogoURL, request.RewardLogoUploadRequest);
                    newFestival.ApprovedLicenseURL = UpdateApprovedLicenseUrl(festival.ApprovedLicenseURL,
                        request.ApprovedLicenseURL, request.ApprovedLicenseUploadRequest);
                    
                    //check potential edit
                    if (festival.FestivalStatus == FestivalStatus.Confirmed
                        && (festival.Description != request.Description || festival.LogoURL != request.LogoURL ||
                            festival.Rules != request.Rules))
                    {
                        festival.FestivalStatus = FestivalStatus.UnderInvestigation;
                        await mediator.Publish(new FestivalRequestedReleased()
                        {
                            FestivalId = festival.Id
                        });
                    }
                    
                    await UpdateQualifiers(request.QualifyersId, request.Id);
                    await unitOfWork.Repository<Festival>().UpdateAsync(newFestival);
                    await unitOfWork.CommitAndRemoveCache(cancellationToken,
                        ApplicationConstants.Cache.GetAllFestivalCacheKey);
                    return await Result<int>.SuccessAsync(festival.Id, localizer["Festival Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(localizer["Event not found"]);
                }
            }
        }

        private string UpdateLogoUrl(string dbLogoUrl, string clientLogoUrl, UploadRequest uploadRequest)
        {
            var updatedLogoUrl = dbLogoUrl;
            if (string.IsNullOrWhiteSpace(clientLogoUrl))
            {
                TryDeleteOldLogoFile(dbLogoUrl);
                updatedLogoUrl = string.Empty;
            }

            if (uploadRequest != null && uploadRequest.Data.Any())
            {
                TryDeleteOldLogoFile(dbLogoUrl);
                updatedLogoUrl = uploadService.UploadAsync(uploadRequest);
            }

            return updatedLogoUrl;
        }

        private string UpdateApprovedLicenseUrl
            (string dbLogoUrl, string clientLogoUrl, UploadRequest uploadRequest)
        {
            var updatedLogoUrl = dbLogoUrl;
            if (string.IsNullOrWhiteSpace(clientLogoUrl))
            {
                TryDeleteOldApprovedLicenseFile(dbLogoUrl);
                updatedLogoUrl = string.Empty;
            }

            if (uploadRequest != null && uploadRequest.Data.Any())
            {
                TryDeleteOldApprovedLicenseFile(dbLogoUrl);
                updatedLogoUrl = uploadService.UploadAsync(uploadRequest);
            }

            return updatedLogoUrl;
        }

        private void TryDeleteOldApprovedLicenseFile(string dbLogoUrl)
        {
            if (!string.IsNullOrWhiteSpace(dbLogoUrl))
            {
                uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
            }
        }

        private void TryDeleteOldLogoFile(string dbLogoUrl)
        {
            if (!string.IsNullOrWhiteSpace(dbLogoUrl))
            {
                uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
            }
        }

        private string UpdateRewardLogoURL(string dbRewardLogoUrl, string clientRewardLogoUrl,
            UploadRequest uploadRequest)
        {
            var updatedRewardLogoUrl = dbRewardLogoUrl;
            if (string.IsNullOrWhiteSpace(clientRewardLogoUrl))
            {
                TryDeleteOldRewardLogoFile(dbRewardLogoUrl);
                updatedRewardLogoUrl = string.Empty;
            }

            if (uploadRequest != null && uploadRequest.Data.Any())
            {
                TryDeleteOldRewardLogoFile(dbRewardLogoUrl);
                updatedRewardLogoUrl = uploadService.UploadAsync(uploadRequest);
            }

            return updatedRewardLogoUrl;
        }

        private void TryDeleteOldRewardLogoFile(string dbLogoUrl)
        {
            if (!string.IsNullOrWhiteSpace(dbLogoUrl))
            {
                uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
            }
        }

        private async Task UpdateQualifiers(List<string> qualifiersStringId, int festivalId)
        {
            var qualifiersId = new List<int>();
            foreach (var item in qualifiersStringId)
            {
                qualifiersId.Add(int.Parse(item));
            }

            var dbFestivalQualifiers = unitOfWork.Repository<FestivalFestivalQualifying>().Entities
                .Where(p => p.FestivalId == festivalId);

            var deletedFestivalQualifiers = dbFestivalQualifiers
                .Where(festivalQualifier => qualifiersId.All(id => id != festivalQualifier.FestivalQualifyingId))
                .ToList();

            var addedFestivalQualifier = qualifiersId
                .Where(id => !dbFestivalQualifiers.Any(qualifier => qualifier.FestivalQualifyingId == id))
                .ToList();

            if (deletedFestivalQualifiers != null)
            {
                foreach (var item in deletedFestivalQualifiers)
                {
                    await unitOfWork.Repository<FestivalFestivalQualifying>().DeleteAsync(item);
                }
            }

            if (addedFestivalQualifier != null)
            {
                foreach (var item in addedFestivalQualifier)
                {
                    await unitOfWork.Repository<FestivalFestivalQualifying>().AddAsync(new FestivalFestivalQualifying()
                    {
                        FestivalId = festivalId,
                        FestivalQualifyingId = item
                    });
                }
            }
        }
    }
}
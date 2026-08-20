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
using HiSubmit.Application.Services.Text;

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
            request.QualifyersId ??= new List<string>();
            request.Description = HtmlTextSanitizer.SanitizeWithoutLinks(request.Description);
            request.Rewards = HtmlTextSanitizer.SanitizeWithoutLinks(request.Rewards);
            request.Rules = HtmlTextSanitizer.SanitizeWithoutLinks(request.Rules);

            if (request.Id == 0)
            {
                var festival = mapper.Map<Festival>(request);
                if (request.UploadRequest?.Data is { Length: > 0 })
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
                    // AutoMapper maps request values into the tracked entity. Preserve
                    // the stored file paths before mapping so a data URL from the
                    // browser cannot be mistaken for an old filesystem path.
                    var storedLogoUrl = festival.LogoURL;
                    var storedRewardLogoUrl = festival.RewardLogoURL;
                    var storedApprovedLicenseUrl = festival.ApprovedLicenseURL;

                    if (string.IsNullOrWhiteSpace(festival.URL))
                    {
                        festival.URL = $"{festival.Name.Trim()}";
                    }

                   


                    var newFestival = mapper.Map(request, festival);
                    newFestival.LogoURL = UpdateLogoUrl
                        (storedLogoUrl, request.LogoURL, request.UploadRequest);

                    newFestival.RewardLogoURL = UpdateRewardLogoURL
                        (storedRewardLogoUrl, request.RewardLogoURL, request.RewardLogoUploadRequest);
                    newFestival.ApprovedLicenseURL = UpdateApprovedLicenseUrl(storedApprovedLicenseUrl,
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
            var hasNewUpload = uploadRequest?.Data is { Length: > 0 };

            if (string.IsNullOrWhiteSpace(clientLogoUrl))
            {
                TryDeleteOldLogoFile(dbLogoUrl);
                updatedLogoUrl = string.Empty;
            }

            if (hasNewUpload)
            {
                TryDeleteOldLogoFile(dbLogoUrl);
                updatedLogoUrl = uploadService.UploadAsync(uploadRequest);
            }
            else if (IsEmbeddedDataUrl(clientLogoUrl))
            {
                // The browser preview is not a persisted file path.
                updatedLogoUrl = dbLogoUrl;
            }

            return updatedLogoUrl;
        }

        private string UpdateApprovedLicenseUrl
            (string dbLogoUrl, string clientLogoUrl, UploadRequest uploadRequest)
        {
            var updatedLogoUrl = dbLogoUrl;
            var hasNewUpload = uploadRequest?.Data is { Length: > 0 };

            if (string.IsNullOrWhiteSpace(clientLogoUrl))
            {
                TryDeleteOldApprovedLicenseFile(dbLogoUrl);
                updatedLogoUrl = string.Empty;
            }

            if (hasNewUpload)
            {
                TryDeleteOldApprovedLicenseFile(dbLogoUrl);
                updatedLogoUrl = uploadService.UploadAsync(uploadRequest);
            }
            else if (IsEmbeddedDataUrl(clientLogoUrl))
            {
                updatedLogoUrl = dbLogoUrl;
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
            var hasNewUpload = uploadRequest?.Data is { Length: > 0 };

            if (string.IsNullOrWhiteSpace(clientRewardLogoUrl))
            {
                TryDeleteOldRewardLogoFile(dbRewardLogoUrl);
                updatedRewardLogoUrl = string.Empty;
            }

            if (hasNewUpload)
            {
                TryDeleteOldRewardLogoFile(dbRewardLogoUrl);
                updatedRewardLogoUrl = uploadService.UploadAsync(uploadRequest);
            }
            else if (IsEmbeddedDataUrl(clientRewardLogoUrl))
            {
                updatedRewardLogoUrl = dbRewardLogoUrl;
            }

            return updatedRewardLogoUrl;
        }

        private static bool IsEmbeddedDataUrl(string value) =>
            value.StartsWith("data:", System.StringComparison.OrdinalIgnoreCase);

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
            foreach (var item in qualifiersStringId ?? [])
            {
                if (int.TryParse(item, out var qualifierId) && qualifierId > 0)
                    qualifiersId.Add(qualifierId);
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

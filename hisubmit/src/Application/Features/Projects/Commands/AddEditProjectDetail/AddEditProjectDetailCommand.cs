using AutoMapper;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Application.Features.Projects.Commands.AddEditProjectDetail;

public class AddEditProjectDetailCommand : IRequest<Result<int>>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string WebSite { get; set; }
    public string Twitter { get; set; }
    public string Youtube { get; set; }
    public string Telegram { get; set; }
    public string WhatsApp { get; set; }
    public string Instagram { get; set; }
    public string SubTitle { get; set; }
    public string OriginalTitle { get; set; }
    public ProjectType ProjectType { get; set; }
    public bool HasNoneEnglishTitle { get; set; }
    public string EnglishBriefSynopsis { get; set; }
    public string OriginalBriefSynopsis { get; set; }

    //Submitter
    public bool UseCurrentUserInformation { get; set; }

    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public AddEditAddressCommand Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }

    public string FileURl { get; set; }
    public UploadRequest FileUrlUploadRequest { get; set; }
    public string URL { get; set; }

    public UploadRequest UploadRequest { get; set; }

    //student project
    public bool StudentProject { get; set; }
    public string UniversityName { get; set; }
    public string StudentPhotoCard { get; set; }

    public AddEditProjectDetailCommand()
    {
        Address = new AddEditAddressCommand();
    }
}

public class AddEditProjectDetailCommandHandler : IRequestHandler<AddEditProjectDetailCommand, Result<int>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUploadService _uploadService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICheckPermission _checkPermission;
    private readonly IStringLocalizer<AddEditProjectDetailCommandHandler> _localize;

    public AddEditProjectDetailCommandHandler(
        IMapper mapper, IUnitOfWork<int> unitOfWork,
        IStringLocalizer<AddEditProjectDetailCommandHandler> localize,
        ICurrentUserService currentUserService,
        ICheckPermission checkPermission,
        IUploadService uploadService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localize = localize;
        _uploadService = uploadService;
        _checkPermission = checkPermission;
    }

    public async Task<Result<int>> Handle(AddEditProjectDetailCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == 0)
        {
            var project = _mapper.Map<Project>(request);
            project.UserId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(request.URL))
            {
                request.URL = $"{request.Title}{request.LastName}";
            }

            if (project.Address.CountryId == 0)
            {
                project.Address = null;
            }

            project.StudentPhotoCard =
                UpdateUniversityCard(string.Empty, request.StudentPhotoCard, request.UploadRequest);
            project.FileURl = UpdateUProjectImage(string.Empty, request.FileURl, request.FileUrlUploadRequest);
            await _unitOfWork.Repository<Project>().AddAsync(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(project.Id, _localize["Project Added"]);
        }
        else
        {
            var dbProject = await _unitOfWork.Repository<Project>().GetByIdAsync(request.Id);
            if (dbProject != null)
            {
                await _checkPermission.CheckWrightProjectPermission(dbProject.UserId);

                // Keep the persisted file paths before AutoMapper copies the browser
                // preview data URLs into the tracked entity.
                var storedStudentPhotoCard = dbProject.StudentPhotoCard;
                var storedProjectFileUrl = dbProject.FileURl;
                var updatedProject = _mapper.Map(request, dbProject);
                if (request.StudentProject)
                    updatedProject.StudentPhotoCard = UpdateUniversityCard(storedStudentPhotoCard,
                        request.StudentPhotoCard, request.UploadRequest);

                updatedProject.FileURl = UpdateUProjectImage(storedProjectFileUrl, request.FileURl, request.FileUrlUploadRequest);
                if (updatedProject.Address.CountryId == 0)
                {
                    updatedProject.Address = null;
                }

                await _unitOfWork.Repository<Project>().UpdateAsync(updatedProject);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(updatedProject.Id, _localize["Project updated"]);
            }
            else
            {
                return await Result<int>.FailAsync("Project not found");
            }
        }
    }

    private string UpdateUniversityCard(string dbRewardLogoUrl, string clientRewardLogoUrl,
        UploadRequest uploadRequest)
    {
        var updatedRewardLogoUrl = dbRewardLogoUrl;
        if (string.IsNullOrWhiteSpace(clientRewardLogoUrl))
        {
            TryDeleteOldRewardLogoFile(dbRewardLogoUrl);
            updatedRewardLogoUrl = string.Empty;
        }

        if (uploadRequest?.Data is { Length: > 0 })
        {
            TryDeleteOldRewardLogoFile(dbRewardLogoUrl);
            updatedRewardLogoUrl = _uploadService.UploadAsync(uploadRequest);
        }
        else if (IsEmbeddedDataUrl(clientRewardLogoUrl))
        {
            // A data URL is only a browser preview, never a persisted file path.
            updatedRewardLogoUrl = dbRewardLogoUrl;
        }

        return updatedRewardLogoUrl;
    }

    private void TryDeleteOldRewardLogoFile(string dbLogoUrl)
    {
        if (!string.IsNullOrWhiteSpace(dbLogoUrl))
        {
            _uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
        }
    }
        
    private string UpdateUProjectImage(string dbProjectCoverUrl, string clientRewardLogoUrl,
        UploadRequest uploadRequest)
    {
        var updatedRewardLogoUrl = dbProjectCoverUrl;
        if (string.IsNullOrWhiteSpace(clientRewardLogoUrl))
        {
            TryDeleteOldProjectCoverFile(dbProjectCoverUrl);
            updatedRewardLogoUrl = string.Empty;
        }

        if (uploadRequest?.Data is { Length: > 0 })
        {
            TryDeleteOldProjectCoverFile(dbProjectCoverUrl);
            updatedRewardLogoUrl = _uploadService.UploadAsync(uploadRequest);
        }
        else if (IsEmbeddedDataUrl(clientRewardLogoUrl))
        {
            // A data URL is only a browser preview, never a persisted file path.
            updatedRewardLogoUrl = dbProjectCoverUrl;
        }

        return updatedRewardLogoUrl;
    }

    private void TryDeleteOldProjectCoverFile(string dbLogoUrl)
    {
        if (!string.IsNullOrWhiteSpace(dbLogoUrl))
        {
            _uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
        }
    }

    private static bool IsEmbeddedDataUrl(string? value) =>
        value?.StartsWith("data:", StringComparison.OrdinalIgnoreCase) == true;
}

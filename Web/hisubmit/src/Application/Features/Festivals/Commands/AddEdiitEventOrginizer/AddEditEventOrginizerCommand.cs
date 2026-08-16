using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.Extensions.Localization;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEdiitEventOrginizer;

public class AddEditEventOrginizerCommand:IRequest<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public int FestivalId { get; set; }
    public string ImageName { get; set; }
    public UploadRequest Image { get; set; }
}

public class AddEditEventOrganizerCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IUploadService uploadService,
    IStringLocalizer<AddEditEventOrganizerCommandHandler> localizer)
    : IRequestHandler<AddEditEventOrginizerCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AddEditEventOrginizerCommand request, CancellationToken cancellationToken)
    {
        
        if(request.Id == 0)
        {
            var organizer = mapper.Map<EventOrginizer>(request);
            if (request.Image.Data != null)
            {
                request.Image.UploadType = UploadType.Organizer;
                organizer.ImageName = uploadService.UploadAsync(request.Image);
            }
            await unitOfWork.Repository<EventOrginizer>().AddAsync(organizer);
            await unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllEventOrginizerKey);
            return await Result<int>.SuccessAsync(organizer.Id, localizer["Organizer Saved"]);
        }
        else
        {
            var organizer = await unitOfWork.Repository<EventOrginizer>().GetByIdAsync(request.Id);
            if(organizer != null)
            {
                var newOrganizer = mapper.Map(request,organizer);
                if (request.Image != null)
                {
                    request.Image.UploadType = UploadType.Organizer;
                }
                organizer.ImageName = UpdateLogoUrl(organizer.ImageName, request.ImageName, request.Image);

                await unitOfWork.Repository<EventOrginizer>().UpdateAsync(newOrganizer);
                await unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllEventOrginizerKey);
                return await Result<int>.SuccessAsync(organizer.Id, localizer["Organizer Updated"]);
            }
            else
            {
                return await Result<int>.FailAsync("organizer not found");
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
    private void TryDeleteOldLogoFile(string dbLogoUrl)
    {
        if (!string.IsNullOrWhiteSpace(dbLogoUrl))
        {
            uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
        } 
    }
}
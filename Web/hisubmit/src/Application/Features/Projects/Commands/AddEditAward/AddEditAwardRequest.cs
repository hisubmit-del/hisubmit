using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Application.Features.Projects.Commands.AddEditAward;

public class UpdateAwardCommand :UpdateAwardRequest, IRequest<IResult>;

public class UpdateAwardCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IUploadService uploadService,
    ICheckPermission checkPermission,
    IStringLocalizer<UpdateAwardCommandHandler> localizer)
    : IRequestHandler<UpdateAwardCommand, IResult>
{
    public async Task<IResult> Handle(UpdateAwardCommand request, CancellationToken cancellationToken)
    {
        var project = await  unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);
        await checkPermission.CheckWrightProjectPermission(project.UserId);
            
        var clientIds = request.Awards.Select(p => p.Id);
        var deletedAward = unitOfWork.Repository<Award>().Entities
            .Where(p=> clientIds.All(id => id != p.Id));

        foreach (var item in deletedAward)
        {
            uploadService.DeleteAsync(new DeleteFileRequest() { RelativeDirectory = item.ImageUrl });
            await unitOfWork.Repository<Award>().DeleteAsync(item);              
        }

        foreach (var writer in request.Awards)
        {
            if (writer.Id == 0)
            {
                
                var mappedWriter = mapper.Map<Award>(writer);
                if (writer.UploadRequest.Data != null)
                {
                    writer.UploadRequest.UploadType = UploadType.Awards;
                    mappedWriter.ImageUrl =  uploadService.UploadAsync(writer.UploadRequest);
                }
                await unitOfWork.Repository<Award>().AddAsync(mappedWriter);
            }
            else
            {
                var dbWriter = await unitOfWork.Repository<Award>().GetByIdAsync(writer.Id);
                if (dbWriter == null)
                {
                    return await Result.FailAsync(localizer["award updated has error"]);
                }
                
                var updatedWriter = mapper.Map(writer, dbWriter);
                updatedWriter.ImageUrl = UpdateLogoUrl(dbWriter.ImageUrl, writer.ImageUrl, writer.UploadRequest);
                await unitOfWork.Repository<Award>().UpdateAsync(updatedWriter);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localizer["award updated"]);
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
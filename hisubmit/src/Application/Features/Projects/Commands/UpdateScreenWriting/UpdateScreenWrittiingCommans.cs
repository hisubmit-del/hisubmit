using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Application.Features.Projects.Commands.UpdateScreenWritings;

public class UpdateScreenWritingCommand : UpdateScreenWritingRequest, IRequest<IResult>;

internal class UpdateScreenWritingCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    ICheckPermission checkPermission,
    IUploadService uploadService,
    IStringLocalizer<UpdateScreenWritingCommandHandler> localize)
    : IRequestHandler<UpdateScreenWritingCommand, IResult>
{
    public async Task<IResult> Handle(UpdateScreenWritingCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);
        await checkPermission.CheckWrightProjectPermission(project.UserId);

        var clientIds = request.ScreenWritings.Select(p => p.Id);
        var deletedAward = unitOfWork.Repository<ScreeningAward>().Entities
            .Where(p => clientIds.All(id => id != p.Id));
        foreach (var item in deletedAward)
        {
            uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = item.ImageUrl });

            await unitOfWork.Repository<ScreeningAward>().DeleteAsync(item);
        }

        foreach (var writer in request.ScreenWritings)
        {
            if (writer.Id == 0)
            {
                var mappedWriter = mapper.Map<ScreeningAward>(writer)
                                   ?? throw new ArgumentNullException("mapper.Map<ScreeningAward>(writer)");
                if (writer.UploadRequest?.Data is { Length: > 0 })
                {
                    writer.UploadRequest.UploadType = UploadType.Awards;
                    mappedWriter.ImageUrl = uploadService.UploadAsync(writer.UploadRequest);
                }

                await unitOfWork.Repository<ScreeningAward>().AddAsync(mappedWriter);
            }
            else
            {
                var dbWriter = await unitOfWork.Repository<ScreeningAward>().GetByIdAsync(writer.Id);
                if (dbWriter == null)
                {
                    return await Result.FailAsync(localize["screen award updated has error"]);
                }

                if (writer.UploadRequest != null)
                {
                    writer.UploadRequest.UploadType = UploadType.Awards;
                }

                var storedImageUrl = dbWriter.ImageUrl;
                var updatedWriter = mapper.Map(writer, dbWriter);

                updatedWriter.ImageUrl = UpdateLogoUrl(storedImageUrl, writer.ImageUrl, writer.UploadRequest);
                await unitOfWork.Repository<ScreeningAward>().UpdateAsync(updatedWriter);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localize["award updated"]);
    }

    private string UpdateLogoUrl(string dbLogoUrl, string clientLogoUrl, UploadRequest uploadRequest)
    {
        var updatedLogoUrl = dbLogoUrl;
        if (string.IsNullOrWhiteSpace(clientLogoUrl))
        {
            TryDeleteOldLogoFile(dbLogoUrl);
            updatedLogoUrl = string.Empty;
        }

        if (uploadRequest?.Data is { Length: > 0 })
        {
            TryDeleteOldLogoFile(dbLogoUrl);
            updatedLogoUrl = uploadService.UploadAsync(uploadRequest);
        }
        else if (clientLogoUrl?.StartsWith("data:", StringComparison.OrdinalIgnoreCase) == true)
        {
            updatedLogoUrl = dbLogoUrl;
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

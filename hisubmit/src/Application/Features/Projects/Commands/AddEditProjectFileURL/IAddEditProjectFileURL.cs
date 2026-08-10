using System.Collections.Generic;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Projects.Commands.DeleteProjectFiles;
using HiSubmit.Application.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using ProjectFilePosition = Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile.ProjectFilePosition;

namespace HiSubmit.Application.Features.Projects.Commands.AddEditProjectFileURL;

public class AddEditProjectFileUrlRequest : AddEditProjectFileURLRequest, IRequest<IResult<AddEditFileUrlResponse>>;


public class AddEditProjectFileUrlCommandHandler(
    IMapper mapper,
    IMediator mediator,
    IUnitOfWork<int> unitOfWork,
    ICheckPermission checkPermission,
    IStringLocalizer<AddEditProjectFileUrlCommandHandler> localizer)
    : IRequestHandler<AddEditProjectFileUrlRequest, IResult<AddEditFileUrlResponse>>
{
    public async Task<IResult<AddEditFileUrlResponse>> Handle(AddEditProjectFileUrlRequest request,
        CancellationToken cancellationToken)
    {
        var project = await unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);

        var files = await unitOfWork.Repository<ProjectFile>()
            .Entities
            .Where(p => p.ProjectId == request.ProjectId)
            .OrderBy(p => p.Order)
            .ToListAsync(cancellationToken);

        var headerFiles = files
            .Where(p => p.Position == Domain.Entities.Projects.ProjectFilePosition.Header).ToList();

        await checkPermission.CheckWrightProjectPermission(project.UserId);

     //   await unitOfWork.BeginTransaction();

        if (request.Id == 0)
        {
            var lastOrder = 0;

            if (files.Any())
                lastOrder = files.Last().Order + 1;
         
           
            if (headerFiles.Any() && request.Position == ProjectFilePosition.Header)
            {
                var positionCheck = await CheckAndUpdateHeaderFiles(request, headerFiles);
                if (!positionCheck.Succeeded)
                    return positionCheck;
            }

            var file = mapper.Map<ProjectFile>(request);
            file.Order = lastOrder;

            await unitOfWork.Repository<ProjectFile>().AddAsync(file);
            await unitOfWork.SaveChangesAsync(cancellationToken);
          //  await unitOfWork.CommitTransaction();
            return await Result<AddEditFileUrlResponse>.SuccessAsync(new AddEditFileUrlResponse()
            {
                FileId = file.Id,
                HasConflictFile = false
            });
        }

        var dbFile = await unitOfWork.Repository<ProjectFile>().GetByIdAsync(request.Id);
        if (dbFile == null) return await Result<AddEditFileUrlResponse>.FailAsync("File not found");
       
        if (request.Position == ProjectFilePosition.Header &&
            dbFile.Position != Domain.Entities.Projects.ProjectFilePosition.Header)
        {
            var positionCheck = await CheckAndUpdateHeaderFiles(request, headerFiles);
            if (!positionCheck.Succeeded)
                return positionCheck;
        }

        var updatedFile = mapper.Map(request, dbFile);
       
        await unitOfWork.Repository<ProjectFile>().UpdateAsync(updatedFile);
        await unitOfWork.SaveChangesAsync(cancellationToken);
      //  await unitOfWork.CommitTransaction();
        return await Result<AddEditFileUrlResponse>.SuccessAsync(new AddEditFileUrlResponse()
        {
            FileId =dbFile.Id,
        }, localizer["File Updated"]);
    }

    private async Task<Result<AddEditFileUrlResponse>> 
        CheckAndUpdateHeaderFiles(AddEditProjectFileUrlRequest request, List<ProjectFile> headerFiles)
    {
        if (request.ConflictWays == ConflictWays.Default)
        {
            if (request.Position != ProjectFilePosition.Header)
                return await Result<AddEditFileUrlResponse>.SuccessAsync();

            if (headerFiles.Any())
            {
                if (headerFiles.First().FileFormat != request.FileFormat)
                    return await Result<AddEditFileUrlResponse>
                        .FailAsync("The file you selected for this position conflicts with other files. Please choose a method to resolve the conflict",new AddEditFileUrlResponse()
                        {
                            HasConflictFile = true
                        });
                if (request.FileFormat is
                    Hisubmit.Client.SharedModels.Enums.FileFormat.Video
                    or Hisubmit.Client.SharedModels.Enums.FileFormat.Music)
                {
                    return await Result<AddEditFileUrlResponse>
                        .FailAsync("The file you selected for this position conflicts with other files. Please choose a method to resolve the conflict",
                            new AddEditFileUrlResponse(){HasConflictFile = true});
                }
            }
        }

        else if (headerFiles.Any())
        {
            switch (request.ConflictWays)
            {
                case ConflictWays.DeleteFiles:
                {
                    foreach (var hf in headerFiles)
                    {
                        await mediator.Send(new DeleteProjectFilesCommand()
                        {
                            Id = hf.Id
                        });
                    }

                    break;
                }
                case ConflictWays.MoveToFiles:
                {
                    foreach (var hf in headerFiles)
                    {
                        hf.Position=Domain.Entities.Projects.ProjectFilePosition.SideBarFile;
                        await unitOfWork.Repository<ProjectFile>().UpdateAsync(hf);
                    }

                    break;
                }
                case ConflictWays.MoveToGallery:
                {
                    foreach (var hf in headerFiles)
                    {
                        hf.Position=Domain.Entities.Projects.ProjectFilePosition.Gallery;
                        await unitOfWork.Repository<ProjectFile>().UpdateAsync(hf);
                    }

                    break;
                }
            }

            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }


        return await Result<AddEditFileUrlResponse>.SuccessAsync();
    }
}


using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Http;
using HiSubmit.Application.Enums;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Requests;
using HiSubmit.Domain.Entities.Projects;
using Microsoft.Extensions.Localization;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Enums;

namespace HiSubmit.Application.Features.Projects.Commands.UploadProjectFile;

public class UploadProjectFileCommand : IRequest<IResult>
{
    public int ProjectId { get; set; }
    public int Fragment { get; set; }
    public IFormFile FormFile { get; set; }
}

public class UploadProjectFileCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IUploadService uploadService,
    ICheckPermission checkPermission,
    IStringLocalizer<UploadProjectFileCommandHandler> localize)
    :
        FeatureBaseService<UploadProjectFileCommandHandler>(mapper, unitOfWork, localize),
        IRequestHandler<UploadProjectFileCommand, IResult>
{
    public async Task<IResult> Handle(UploadProjectFileCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);
        await checkPermission.CheckWrightProjectPermission(project.UserId);
            
        var existFile = uploadService.ExistAsync(new ExistFileRequest()
        {
            Name = request.FormFile.Name,
            UploadType = UploadType.ProjectFile
        });

        if (request.Fragment == 0 && existFile)
        {
            uploadService.DeleteAsync(new DeleteFileWithUploadTypeRequest
                { UploadType = UploadType.ProjectFile, Name = request.FormFile.Name });
        }

        await uploadService.AppendAsync(new AppendFileRequest
        {
            File = request.FormFile,
            UploadType = UploadType.ProjectFile
        });

        return await Result.SuccessAsync(_localize["Project updated"]);
    }
}

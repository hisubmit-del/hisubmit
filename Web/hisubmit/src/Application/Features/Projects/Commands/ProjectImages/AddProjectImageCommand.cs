using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Enums;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Projects.Commands.ProjectImages;

public class AddProjectImageCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string State { get; set; }
    public int ProjectId { get; set; }

    public UploadRequest UploadRequest { get; set; }

    public AddProjectImageCommand()
    {
        UploadRequest = new UploadRequest
        {
            UploadType = UploadType.ProjectFile
        };
    }
}

public class AddProjectImageCommandHandler : IRequestHandler<AddProjectImageCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUploadService _uploadService;
    private readonly ICheckPermission _checkPermission;
    private readonly IStringLocalizer<AddProjectImageCommandHandler> _localize;

    public AddProjectImageCommandHandler
    (IMapper mapper, IUnitOfWork<int> unitOfWork,
        ICheckPermission checkPermission,
        IUploadService uploadService, IStringLocalizer<AddProjectImageCommandHandler> localize)
    {
        _mapper = mapper;
        _localize = localize;
        _unitOfWork = unitOfWork;
        _uploadService = uploadService;
        _checkPermission = checkPermission;
    }

    public async Task<IResult> Handle(AddProjectImageCommand request, CancellationToken cancellationToken)
    {
        var projectImage = _mapper.Map<ProjectImage>(request);
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(projectImage.ProjectId);
        await _checkPermission.CheckWrightProjectPermission(project.UserId);
        projectImage.Url = _uploadService.UploadAsync(request.UploadRequest);
        await _unitOfWork.Repository<ProjectImage>().AddAsync(projectImage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localize["Image Added"]);
    }
}
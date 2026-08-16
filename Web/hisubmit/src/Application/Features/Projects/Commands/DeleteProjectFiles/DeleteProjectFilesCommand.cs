using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using HiSubmit.Domain.Entities.Projects;
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

namespace HiSubmit.Application.Features.Projects.Commands.DeleteProjectFiles
{
    public class DeleteProjectFilesCommand : IRequest<IResult>
    {
        public int Id { get; set; }
    }

    public class DeleteProjectFileCommandHandler : IRequestHandler<DeleteProjectFilesCommand, IResult>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteProjectFileCommandHandler> _localizer;
        private readonly IUploadService _uploadService;
        private readonly ICheckPermission _checkPermission;

        public DeleteProjectFileCommandHandler
            (IMapper mapper, IUnitOfWork<int> unitOfWork, 
                IStringLocalizer<DeleteProjectFileCommandHandler> localizer, 
                IUploadService uploadService, ICheckPermission checkPermission)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _uploadService = uploadService;
            _checkPermission = checkPermission;
        }

        public async Task<IResult> Handle(DeleteProjectFilesCommand request, CancellationToken cancellationToken)
        {
            var file = await _unitOfWork.Repository<ProjectFile>().GetByIdAsync(request.Id);
            if (file != null)
            {
                var project = await _unitOfWork.Repository<Project>().GetByIdAsync(file.ProjectId);
                await _checkPermission.CheckWrightProjectPermission(project.UserId);
                if (file.IsLocalFile)
                {
                    var deleteResult = _uploadService.DeleteAsync(new DeleteFileRequest()
                    {
                        RelativeDirectory = file.LocalFileURL
                    });
                    if (!deleteResult)
                    {
                        return await Result.FailAsync("local file not found");
                    }
                }
                await _unitOfWork.Repository<ProjectFile>().DeleteAsync(file);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result.SuccessAsync(_localizer["File Deleted"]);
            }
            else
            {
                return await Result.FailAsync(_localizer["File not found"]);
            }
        }
    }
}

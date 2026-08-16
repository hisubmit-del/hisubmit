using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Commands.DeleteFestivalFile
{
    public class DeleteFestivalFileCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteFestivalFileCommandHandler : IRequestHandler<DeleteFestivalFileCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteFestivalFileCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        public DeleteFestivalFileCommandHandler(
            IStringLocalizer<DeleteFestivalFileCommandHandler> localizer, 
            IUnitOfWork<int> unitOfWork, IUploadService uploadService)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
        }

        public async Task<Result<int>> Handle(DeleteFestivalFileCommand request, CancellationToken cancellationToken)
        {
            var file =await _unitOfWork.Repository<FestivalFile>().GetByIdAsync(request.Id);
            if(file != null)
            {
                TryDeleteFile(file.FileURL);
                await _unitOfWork.Repository<FestivalFile>().DeleteAsync(file);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken,ApplicationConstants.Cache.GetAllFestivalCacheKey);
                return await Result<int>.SuccessAsync(request.Id, _localizer["File Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["file not found"]);
            }
        }

        private void TryDeleteFile(string dbLogoUrl)
        {
            if (!string.IsNullOrWhiteSpace(dbLogoUrl))
            {
                _uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
            }
        }
    }
}

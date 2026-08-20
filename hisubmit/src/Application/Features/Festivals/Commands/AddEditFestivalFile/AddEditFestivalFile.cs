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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalFile
{
    public class AddEditFestivalFileCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileURL { get; set; }
        public FileFormat FileFormat { get; set; }

        public string Description { get; set; }
        public int FestivalId { get; set; }
        public UploadRequest UploadFileRequest { get; set; }
    }

    public class AddEditFestivalFileCommandHandler : IRequestHandler<AddEditFestivalFileCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditFestivalFileCommandHandler> _localizer;
        public AddEditFestivalFileCommandHandler(
            IMapper mapper, IUnitOfWork<int> unitOfWork, 
            IStringLocalizer<AddEditFestivalFileCommandHandler> localizer,
            IUploadService uploadService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _uploadService = uploadService;
        }


        public async Task<Result<int>> Handle(AddEditFestivalFileCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return await Result<int>.FailAsync(_localizer["File name is required"]);

            if(request.Id == 0)
            {
                if (request.UploadFileRequest?.Data is not { Length: > 0 })
                    return await Result<int>.FailAsync(_localizer["Please select a file to upload"]);

                var file = _mapper.Map<FestivalFile>(request);
                if (request.UploadFileRequest != null)
                {
                    request.UploadFileRequest.FileName = $"D-{Guid.NewGuid()}{request.UploadFileRequest.Extension}";
                }
                file.FileURL = _uploadService.UploadAsync(request.UploadFileRequest);
                await _unitOfWork.Repository<FestivalFile>().AddAsync(file);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalCacheKey);
                return await Result<int>.SuccessAsync(file.Id, _localizer["File Added"]);
            }
            else
            {
                var dbFile =await _unitOfWork.Repository<FestivalFile>().GetByIdAsync(request.Id);
                if(dbFile != null)
                {
                    var updatedFile = _mapper.Map(request,dbFile);
                    updatedFile.FileURL = UpdateFile(dbFile.FileURL,request.FileURL,request.UploadFileRequest);
                    await _unitOfWork.Repository<FestivalFile>().UpdateAsync(updatedFile);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalCacheKey);
                    return await Result<int>.SuccessAsync(updatedFile.Id, _localizer["File Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync("File Not  Found");
                }
            }
        }
        private string UpdateFile(string dbFileURl, string clientFileUrl, UploadRequest uploadRequest)
        {
            var updatedRewardLogoUrl = dbFileURl;
            if (string.IsNullOrWhiteSpace(clientFileUrl))
            {
                TryDeleteFile(dbFileURl);
                updatedRewardLogoUrl = string.Empty;
            }
            if (uploadRequest != null && uploadRequest.Data.Any())
            {
                TryDeleteFile(dbFileURl);
                updatedRewardLogoUrl = _uploadService.UploadAsync(uploadRequest);
            }
            return updatedRewardLogoUrl;
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

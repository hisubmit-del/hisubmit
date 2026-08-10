using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalImages;

public class AddEditFestivalImageCommand:IRequest<IResult>
{
    public  int FestivalId { get; set; }
    public List<FestivalImageDto> Images { get; set; } = new();
}


public class AddEditFestivalImageCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IUploadService uploadService,
    IStringLocalizer<AddEditFestivalImageCommandHandler> localizer)
    : IRequestHandler<AddEditFestivalImageCommand, IResult>
{
    public async Task<IResult> Handle(AddEditFestivalImageCommand request, CancellationToken cancellationToken)
    {
        var dbImages = await unitOfWork.Repository<Image>()
            .Entities
            .Where(p => p.FestivalId == request.FestivalId && (p.ImageType==ImageType.Cover ||p.ImageType==ImageType.Images)).ToListAsync(cancellationToken);
        
        
        foreach (var image in request.Images)
        {
            image.FestivalId = request.FestivalId;
        }

        var addedImages = request.Images.Where(p => p.Id == 0);
        var deletedImages = dbImages.Where(p => request.Images.All(img => img.Id != p.Id));
        var updatedImages = request.Images.Where(img=>dbImages.Any(dbImg=>dbImg.Id==img.Id));

        foreach (var addedImage in addedImages)
        {
            var url=  uploadService.UploadAsync(addedImage.UploadRequest);
            var image = mapper.Map<Image>(addedImage);
            image.Url = url;
            await unitOfWork.Repository<Image>().AddAsync(image);
        }

        foreach (var deletedImage in deletedImages)
        {
             uploadService.DeleteAsync(new DeleteFileRequest(){RelativeDirectory = deletedImage.Url});
             await  unitOfWork.Repository<Image>().DeleteAsync(deletedImage);
        }

        foreach (var updatedImage in updatedImages)
        {
            var dbImage = dbImages.First(p => p.Id == updatedImage.Id);
            if (dbImage.Url != updatedImage.Url)
            {
                dbImage.Url = UpdateImage(dbImage.Url,updatedImage.Url,updatedImage.UploadRequest);
            }

            await unitOfWork.Repository<Image>().UpdateAsync(dbImage);
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localizer["Images saved"]);
    }
    
    private string UpdateImage
        (string dbLogoUrl, string clientLogoUrl, UploadRequest uploadRequest)
    {
        var updatedLogoUrl = dbLogoUrl;
        if (string.IsNullOrWhiteSpace(clientLogoUrl))
        {
            TryDeleteImage(dbLogoUrl);
            updatedLogoUrl = string.Empty;
        }

        if (uploadRequest != null && uploadRequest.Data.Any())
        {
            TryDeleteImage(dbLogoUrl);
            updatedLogoUrl = uploadService.UploadAsync(uploadRequest);
        }

        return updatedLogoUrl;
    }
        
    private void TryDeleteImage(string dbLogoUrl)
    {
        if (!string.IsNullOrWhiteSpace(dbLogoUrl))
        {
            uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
        } 
    }
}

public class FestivalImageDto
{
    public int Id { get; set; }
    public int FestivalId { get; set; }
    public  string Title { get; set; }
    public  ImageType ImageType { get; set; }
    public  string Url { get; set; }
    public  UploadRequest UploadRequest { get; set; }

    public FestivalImageDto()
    {
        UploadRequest = new UploadRequest();
    }
}

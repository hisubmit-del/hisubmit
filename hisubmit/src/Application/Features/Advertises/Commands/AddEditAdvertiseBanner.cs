using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Requests;
using Microsoft.Extensions.Localization;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;

namespace HiSubmit.Application.Features.Advertises.Commands;

public class AddEditAdvertiseBannerCommand :AddEditAdvertiseBannerRequest, IRequest<IResult>;

public class AddEditAdvertiseCommandHandler(
    IMapper mapper,
    IUploadService uploadService,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<AddEditAdvertiseCommandHandler> localize)
    : IRequestHandler<AddEditAdvertiseBannerCommand, IResult>
{
    public async Task<IResult> Handle(AddEditAdvertiseBannerCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == 0)
        {
            var banner = mapper.Map<AdvertiseBanner>(request);
            request.UploadRequest.FileName = Guid.NewGuid() + request.UploadRequest.FileName;
            banner.Url = uploadService.UploadAsync(request.UploadRequest);
            await unitOfWork.Repository<AdvertiseBanner>().AddAsync(banner);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(localize["Banner Added"]);
        }

        var dbBanner = await unitOfWork.Repository<AdvertiseBanner>()
            .GetByIdAsync(request.Id);
        if (dbBanner == null)
            return await Result.FailAsync(localize["Banner Not found"]);

        var updatedBanner = mapper.Map(request, dbBanner);
        updatedBanner.Url = UpdateFile(dbBanner.Url,request.UploadRequest);
        await unitOfWork.Repository<AdvertiseBanner>()
            .UpdateAsync(updatedBanner);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localize["Banner Updated"]);
    }

    private string UpdateFile(string fileUrl, UploadRequest request)
    {
        var url = string.Empty;
        if (!string.IsNullOrWhiteSpace(fileUrl))
            uploadService.DeleteAsync(new DeleteFileRequest
            {
                RelativeDirectory = fileUrl
            });

        if (! string.IsNullOrWhiteSpace(request.FileName))
        {
            request.FileName=Guid.NewGuid() + request.FileName;
            url = uploadService.UploadAsync(request);
        }

        return url;
    }
}

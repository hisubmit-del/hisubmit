using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Advertises.Commands;

public class DeleteAdvertiseBannerCommand:DeleteAdvertiseBannerRequest, IRequest<IResult>;

public class DeleteAdvertiseBannerCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IUploadService uploadService,
    IStringLocalizer<DeleteAdvertiseBannerCommandHandler> localize)
    : IRequestHandler<DeleteAdvertiseBannerCommand, IResult>
{
    public async Task<IResult> Handle(DeleteAdvertiseBannerCommand request, CancellationToken cancellationToken)
    {
        var banner = await unitOfWork
            .Repository<AdvertiseBanner>().GetByIdAsync(request.Id);
        if (banner == null)
            return await Result.FailAsync(localize["banner not found"]);
        uploadService.DeleteAsync(new DeleteFileRequest(){RelativeDirectory = banner.Url});
        await unitOfWork.Repository<AdvertiseBanner>().DeleteAsync(banner);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("The banner has been removed");
    }
}
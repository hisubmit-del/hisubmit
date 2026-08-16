using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Enums;
using HiSubmit.Application.Events.Advertises.AddedAdvertise;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Files;
using HiSubmit.Domain.Enums;
using HiSubmit.Domain.Enums.Advertises;

namespace HiSubmit.Application.Features.Advertises.Commands;

public class AddAdvertiseCommand :AddAdvertiseRequest,IRequest<IResult>;

public class AddAdvertiseCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IUploadService uploadService,
    ICurrentUserService currentUserService)
    : IRequestHandler<AddAdvertiseCommand, IResult>
{
    public async Task<IResult> Handle(AddAdvertiseCommand request, CancellationToken cancellationToken)
    {
        var advertiseRequest = mapper.Map<AdvertiseRequest>(request);
        if (!string.IsNullOrWhiteSpace(currentUserService.UserId))
            advertiseRequest.UserId = currentUserService.UserId;
        
        await unitOfWork.Repository<AdvertiseRequest>()
            .AddAsync(advertiseRequest);
        foreach (var i in request.Images)
        {
            var image = mapper.Map<Image>(i);
            image.Url = uploadService.UploadAsync(i.UploadRequest);
            advertiseRequest.Images.Add(image);
        }

        foreach (var f in request.Files)
        {
            var file = mapper.Map<AttachFile>(f);
            file.Url = uploadService.UploadAsync(f.UploadRequest);
            advertiseRequest.Files.Add(file);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new AddedAdvertiseEvent(),cancellationToken);

        return await Result.SuccessAsync("Your request has been registered");
    }
}

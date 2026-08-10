using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Entities.Files;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Advertises.Queries;

public class GetDetailAdvertiseQuery :GetDetailAdvertiseRequest, IRequest<IResult<GetDetailAdvertiseResponse>>;

public class GetDetailAdvertiseQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetDetailAdvertiseQuery, IResult<GetDetailAdvertiseResponse>>
{
    public async Task<IResult<GetDetailAdvertiseResponse>> Handle(GetDetailAdvertiseQuery request,
        CancellationToken cancellationToken)
    {
        var response = await unitOfWork.Repository<AdvertiseRequest>()
            .Entities
            .Where(p=>p.Id==request.Id)

            .FirstOrDefaultAsync(cancellationToken);
       
        var files = await unitOfWork.Repository<AttachFile>()
            .Entities
            .Where(p => p.AdvertiseRequestId == request.Id)
            .ToListAsync(cancellationToken);
        
        var mappedResponse = mapper.Map<GetDetailAdvertiseResponse>(response);
        mappedResponse.Files =mapper.Map<List<AttachFileDto>>(files);
        
        return await Result<GetDetailAdvertiseResponse>.SuccessAsync(mappedResponse);
    }
}

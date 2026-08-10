using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllImages;

public class GetAllFestivalImageQuery:PagedRequest,IRequest<PaginatedResult<GetAllFestivalImageResponse>>
{
    public int FestivalId { get; set; }
}

public class GetAllFestivalImageQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    : IRequestHandler<GetAllFestivalImageQuery, PaginatedResult<GetAllFestivalImageResponse>>
{
    public Task<PaginatedResult<GetAllFestivalImageResponse>> Handle(GetAllFestivalImageQuery request,
        CancellationToken cancellationToken)
    {
        var images = unitOfWork.Repository<Image>()
            .Entities.Where(p => p.FestivalId == request.FestivalId)
            .ProjectTo<GetAllFestivalImageResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
        return images;
    }
}

public class GetAllFestivalImageResponse
{
    public  int Id { get; set; }
    public string Title { get; set; }
    public  string Url { get; set; }
    
    public  int FestivalId { get; set; }
    public ImageType ImageType { get; set; }
}


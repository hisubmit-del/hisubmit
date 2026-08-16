using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Seo;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Seo;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.News.Queries;

public class GetDetailNewQuery:IRequest<IResult<GetDetailNewResponse>>
{
    public int Id { get; set; }
}

public class GetDetailNewQueryHandler : IRequestHandler<GetDetailNewQuery, IResult<GetDetailNewResponse>>
{
    private readonly IMapper _Mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetDetailNewQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _Mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<GetDetailNewResponse>> Handle(GetDetailNewQuery request, CancellationToken cancellationToken)
    {
        var newDb = await _unitOfWork.Repository<New>().GetByIdAsync(request.Id);
        if (newDb == null) return await Result<GetDetailNewResponse>.FailAsync("new not found");
        
        
        var mappedNew = _Mapper.Map<GetDetailNewResponse>(newDb);
        mappedNew.SeoTag = await _unitOfWork.Repository<MetaTag>()
            .Entities
            .Where(p => p.PageId == mappedNew.Id.ToString() && p.Type == PageType.News)
            .ProjectTo<AddEditSeoTagRequest>(_Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        
        return await Result<GetDetailNewResponse>.SuccessAsync(mappedNew);
    }
}

public class GetDetailNewResponse
{
    public  int Id { get; set; }
    public  string Title { get; set; }
    public  string BannerUrl { get; set; }
    public  string Description { get; set; }
    public  bool IsEnable { get; set; }
    public string ShortDescription { get; set; }
    
    public AddEditSeoTagRequest SeoTag { get; set; }

}
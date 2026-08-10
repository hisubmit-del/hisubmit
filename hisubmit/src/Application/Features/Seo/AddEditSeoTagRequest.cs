using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Seo;

namespace HiSubmit.Application.Features.Seo;

public class AddEditSeoTagCommand : AddEditSeoTagRequest, IRequest<IResult>;


public class AddEditSeoTagCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    : IRequestHandler<AddEditSeoTagCommand, IResult>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork<int> _unitOfWork = unitOfWork;


    public async Task<IResult> Handle
        (AddEditSeoTagCommand request, CancellationToken cancellationToken)
    {
        var mappedEntity = _mapper.Map<MetaTag>(request);
        mappedEntity.Type = (PageType)request.Type;
        mappedEntity.PageId = request.PageId;
        mappedEntity.PageTitle = request.PageTitle;
        await _unitOfWork.Repository<MetaTag>().AddAsync(mappedEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}

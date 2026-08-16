using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Misc;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Domain.Entities.Content;

namespace HiSubmit.Application.Features.FooterItems.Queries.GetById
{
    public class GetFooterItemByIdQuery : IRequest<Result<FooterItemDto>>
    {
        public int Id { get; set; }
    }

    internal class GetFooterItemByIdQueryHandler : IRequestHandler<GetFooterItemByIdQuery, Result<FooterItemDto>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetFooterItemByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FooterItemDto>> Handle(GetFooterItemByIdQuery query, CancellationToken cancellationToken)
        {
            var documentType = await _unitOfWork.Repository<MenuItem>().GetByIdAsync(query.Id);
            var mappedDocumentType = _mapper.Map<FooterItemDto>(documentType);
            return await Result<FooterItemDto>.SuccessAsync(mappedDocumentType);
        }
    }
}
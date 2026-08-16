using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.FestivalFocs.Queries.GetFestivalFocusDetail
{
    public class GetFestivalFocusDeailQuery:IRequest<Result<GetFestivalFocusDetailResponse>>
    {
        public int Id { get; set; }
    }
    public  class GetFestivalFocusDetailQueryHandler : IRequestHandler<GetFestivalFocusDeailQuery, Result<GetFestivalFocusDetailResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetFestivalFocusDetailQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetFestivalFocusDetailResponse>> Handle(GetFestivalFocusDeailQuery query, CancellationToken cancellationToken)
        {
            var focus = await _unitOfWork.Repository<FestivalFocus>().GetByIdAsync(query.Id);
            var mappedFocua = _mapper.Map<GetFestivalFocusDetailResponse>(focus);
            return await Result<GetFestivalFocusDetailResponse>.SuccessAsync(mappedFocua);
        }
    }
}

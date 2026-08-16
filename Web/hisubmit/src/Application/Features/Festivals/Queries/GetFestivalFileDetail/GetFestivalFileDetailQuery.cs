using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Queries.GetFestivalFileDetail
{
    public class GetFestivalFileDetailQuery:IRequest<Result<GetFestivalFileDetailResponse>>
    {
        public int Id { get; set; }
    }

    public class GetFestivalFileDetailQueryHandler : IRequestHandler<GetFestivalFileDetailQuery, Result<GetFestivalFileDetailResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<GetFestivalFileDetailQueryHandler> _localizer;
        public GetFestivalFileDetailQueryHandler(
            IMapper mapper, IUnitOfWork<int> unitOfWork,
            IStringLocalizer<GetFestivalFileDetailQueryHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<GetFestivalFileDetailResponse>> Handle(GetFestivalFileDetailQuery request, CancellationToken cancellationToken)
        {
            var file =await _unitOfWork.Repository<FestivalFile>()
                .GetByIdAsync(request.Id);
            if(file != null)
            {
                var mappedFile = _mapper.Map<GetFestivalFileDetailResponse>(file);
                return await Result<GetFestivalFileDetailResponse>.SuccessAsync(mappedFile);
            }
            else
            {
                return await Result<GetFestivalFileDetailResponse>.FailAsync(_localizer["file not found"]);
            }
        }
    }
}

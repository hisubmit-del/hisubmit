using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Queries.GetEventCateoryById
{
    public class GetEventCategoryByIdQuery:IRequest<Result<GetEventCategoryByIdResponse>>
    {
        public int Id { get; set; }
    }
    internal class GetEventCategoryByIdQueryHandler : IRequestHandler<GetEventCategoryByIdQuery, Result<GetEventCategoryByIdResponse>>
    {
        private readonly IRepositoryAsync<EventCategory,int> _repository;
        private readonly IStringLocalizer<GetEventCategoryByIdQueryHandler> _localizer;
        private readonly IMapper _mapper;
        public GetEventCategoryByIdQueryHandler(IRepositoryAsync<EventCategory, int> repository,
            IStringLocalizer<GetEventCategoryByIdQueryHandler> localizer,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<GetEventCategoryByIdResponse>> Handle(GetEventCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category =await  _repository.Entities.Where(p => p.Id == request.Id)
                .Include(p => p.DeadlineEventCategories).ThenInclude(p=>p.DeadLine)
                .ProjectTo<GetEventCategoryByIdResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if(category != null)
            {
                return await Result<GetEventCategoryByIdResponse>.SuccessAsync(category);
            }
            else
            {
                return await Result<GetEventCategoryByIdResponse>.FailAsync(_localizer["Event Category not found"]);
            }
                
        }
    }
}

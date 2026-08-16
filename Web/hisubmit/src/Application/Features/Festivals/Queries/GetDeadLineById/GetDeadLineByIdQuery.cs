using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Queries.GetDeadLineById
{
    public class GetDeadLineByIdQuery:IRequest<Result<GetDeadLineByIdResponse>>
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
    }

    public class GetDeadLineByIdQueryHandler : IRequestHandler<GetDeadLineByIdQuery, Result<GetDeadLineByIdResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetDeadLineByIdResponse> _localizer;
        private readonly IRepositoryAsync<DeadLine, int> _repository;

        public GetDeadLineByIdQueryHandler(
            IMapper mapper, IStringLocalizer<GetDeadLineByIdResponse> localizer, 
            IRepositoryAsync<DeadLine, int> repository)
        {
            _mapper = mapper;
            _localizer = localizer;
            _repository = repository;
        }

        public async Task<Result<GetDeadLineByIdResponse>> Handle(GetDeadLineByIdQuery request, CancellationToken cancellationToken)
        {
            var deadLine =await _repository.Entities
                .ProjectTo<GetDeadLineByIdResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p=>p.Id==request.Id);

            if(deadLine != null)
            {
                return await Result<GetDeadLineByIdResponse>.SuccessAsync(deadLine,"Success doing");
            }
            else
            {
                return await Result<GetDeadLineByIdResponse>.FailAsync("deadline not found");
            }
        }
    }
}

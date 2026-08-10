using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Projects.Queries.GetProjectCreditDetail
{
    public class GetProjectCreditDetailQuery : IRequest<Result<GetProjectCreditDetailResponse>>
    {
        public int Id { get; set; }
    }

    public class GetProjectCreditDetailQueryHandler : IRequestHandler<GetProjectCreditDetailQuery, Result<GetProjectCreditDetailResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfwork;
        private readonly IStringLocalizer<GetProjectCreditDetailQueryHandler> _localizer;
        public GetProjectCreditDetailQueryHandler(
            IMapper mapper, IUnitOfWork<int> unitOfwork,
            IStringLocalizer<GetProjectCreditDetailQueryHandler> localizer)
        {
            _mapper = mapper;
            _unitOfwork = unitOfwork;
            _localizer = localizer;
        }


        public async Task<Result<GetProjectCreditDetailResponse>> Handle(GetProjectCreditDetailQuery request, CancellationToken cancellationToken)
        {
            var credit = await _unitOfwork.Repository<ProjectCredit>()
                .Entities
                .Include(p => p.ProjectItemPeople)
                .ProjectTo<GetProjectCreditDetailResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            if (credit != null)
            {
                return await Result<GetProjectCreditDetailResponse>.SuccessAsync(credit);
            }
            else
            {
                return await Result<GetProjectCreditDetailResponse>.FailAsync(_localizer["credit not found"]);
            }

        }
    }
}

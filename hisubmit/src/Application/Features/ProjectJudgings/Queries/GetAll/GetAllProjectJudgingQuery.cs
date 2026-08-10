using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Specifications.ProjectJudgings;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.ProjectJudgings.Queries.GetAll
{
    public class GetAllProjectJudgingQuery : PagedRequest, IRequest<PaginatedResult<GetAllProjectJudgingResponse>>
    {
        public int? SubmitId { get; set; }
        public string UserId { get; set; }
        public int? FestivalId { get; set; }
        public bool GetCurrentUser { get; set; }
        public string SearchString { get; set; }
    }


    public class GetAllProjectJudgingQueryHandler(
        IUnitOfWork<int> unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IUserService userService)
        : IRequestHandler<GetAllProjectJudgingQuery, PaginatedResult<GetAllProjectJudgingResponse>>
    {
        public async Task<PaginatedResult<GetAllProjectJudgingResponse>> Handle(GetAllProjectJudgingQuery request, CancellationToken cancellationToken)
        {
            if (request.GetCurrentUser)
            {
                request.UserId = currentUserService.UserId;
            }
            var specification = new GetAllProjectJudgingFilterSpecification(request);
            var projectJudging = await unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Include(p=>p.Submit).ThenInclude(p=>p.Project)
            .Include(p=>p.Submit).ThenInclude(p=>p.Festival)
            .Specify(specification)
            .ProjectTo<GetAllProjectJudgingResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

            var userFullNames =await userService
                .GetAllAsync(projectJudging.Data.Select(p => p.UserId).ToList());

            if (projectJudging.Data != null)
                foreach (var pj in projectJudging.Data)
                    pj.UserFullName = userFullNames.Data.Where(p => p.Id == pj.UserId)
                        .Select(p => p.FirstName + " " + p.LastName).FirstOrDefault();
            
            return projectJudging;
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Projects;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;

namespace HiSubmit.Application.Features.Projects.Queries.GetAllProjectCredits
{
    public class GetAllProjectCreditQuery:IRequest<Result<List<GetAllProjectCreditResponse>>>
    {
        public int ProjectId { get; set; }
        public bool WithInclude { get; set; }
    }


    public class GetAllProjectCreditQueryHandler(
        IMapper mapper,
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<GetAllProjectCreditQueryHandler> localizer)
        : IRequestHandler<GetAllProjectCreditQuery, Result<List<GetAllProjectCreditResponse>>>
    {
        public async Task<Result<List<GetAllProjectCreditResponse>>> 
            Handle(GetAllProjectCreditQuery request, CancellationToken cancellationToken)
        {
            var projectCreditSpec = new ProjectCreditFilterSpecification(request.ProjectId);
            var query = unitOfWork.Repository<ProjectCredit>()
                .Entities
                .Specify(projectCreditSpec);

            if (request.WithInclude)
            {
                query = query.Include(p => p.ProjectItemPeople);
            }
            var credits  =await query
                .ProjectTo<GetAllProjectCreditResponse>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return await Result<List<GetAllProjectCreditResponse>>.SuccessAsync(credits);
        }
    }

    public class GetAllProjectCreditResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<AddEditProjectCreditItemCommand> ProjectItemPeople { get; set; } = new();
        public int ProjectId { get; set; }
    }
}

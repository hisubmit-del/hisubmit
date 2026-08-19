using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Projects;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using HiSubmit.Application.Filters;

namespace HiSubmit.Application.Features.Projects.Queries.GetAllProjectFiles
{
    public class GetAllProjectFilesQuery : IRequest<Result<List<GetAllProjectFileResponse>>>
    {
        public int ProjectId { get; set; }
    }

    

    public class
        GetAllProjectFileQueryHandler(
            IMapper mapper,
            IUnitOfWork<int> unitOfWork,
            ICheckPermission checkPermission)
        : IRequestHandler<GetAllProjectFilesQuery,
            Result<List<GetAllProjectFileResponse>>>
    {
        public async Task<Result<List<GetAllProjectFileResponse>>> Handle(GetAllProjectFilesQuery request,
            CancellationToken cancellationToken)
        {
            var projectOwnerId = await unitOfWork.Repository<Project>()
                .Entities
                .AsNoTracking()
                .Where(p => p.Id == request.ProjectId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(projectOwnerId) ||
                !await checkPermission.CheckReadProjectPermission(request.ProjectId, projectOwnerId))
            {
                return await Result<List<GetAllProjectFileResponse>>
                    .FailAsync("You do not have permission to view this project's files.");
            }

            var specification = new GetAllProjectFilesSpecification(request.ProjectId);
            var files = await unitOfWork.Repository<ProjectFile>()
                .Entities
                .Specify(specification)
                .ProjectTo<GetAllProjectFileResponse>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken);

            return await Result<List<GetAllProjectFileResponse>>.SuccessAsync(files);
        }
    }
}

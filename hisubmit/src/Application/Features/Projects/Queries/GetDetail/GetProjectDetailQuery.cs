using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Filters;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Exceptions;
using HiSubmit.Domain.Entities.Projects;
using Microsoft.Extensions.Localization;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;

namespace HiSubmit.Application.Features.Projects.Queries.GetDetail;

public class GetProjectDetailQuery:IRequest<Result<GetProjectDetailResponse>>
{
    public int Id { get; set; }
    public string URL { get; set; }
}
public class GetProjectDetailQueryHandler(
    IMapper mapper,
    IUserService userService,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<GetProjectDetailQueryHandler> localizer,
    ICheckPermission checkPermission)
    : IRequestHandler<GetProjectDetailQuery, Result<GetProjectDetailResponse>>
{
    private readonly IStringLocalizer<GetProjectDetailQueryHandler> _localizer = localizer;

    public async Task<Result<GetProjectDetailResponse>> Handle(GetProjectDetailQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Project> query;
        if (!string.IsNullOrWhiteSpace(request.URL))
        {
            query = unitOfWork.Repository<Project>().Entities.Where(p => p.URL == request.URL);
        }
        else
        {
            query = unitOfWork.Repository<Project>().Entities.Where(p => p.Id == request.Id);
        }

        var project =await query
            .ProjectTo<GetProjectDetailResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);


        var userAccount = await userService.GetAsync(project.UserId);

        project.UserImageUrl = userAccount.Data.ProfilePictureDataUrl;
        project.UserFullName = userAccount.Data.FullName;
        if(! await checkPermission.CheckReadProjectPermission(project.Id,project.UserId))
            throw new DontPermissionException();
        

        return await Result<GetProjectDetailResponse>.SuccessAsync(project);
    }
}
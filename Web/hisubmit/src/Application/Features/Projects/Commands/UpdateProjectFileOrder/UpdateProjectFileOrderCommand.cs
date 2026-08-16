using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Projects.Commands.UpdateProjectFileOrder;

public class UpdateProjectFileOrderCommand:IRequest<IResult>
{
    public Dictionary<int,int> FilesOrders { get; set; }
}

public class UpdateProjectFileOrderCommandHandler(IUnitOfWork<int> unitOfWork)
    : IRequestHandler<UpdateProjectFileOrderCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProjectFileOrderCommand request, CancellationToken cancellationToken)
    {
        foreach (var file in request.FilesOrders)
        {
            var dbFiles = await unitOfWork.Repository<ProjectFile>().GetByIdAsync(file.Key);
            dbFiles.Order = file.Value;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;

namespace HiSubmit.Application.Features.Projects.Commands.DeleteProjectCredit
{
    public class DeleteProjectCreditCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteProjectCreditCommandHandler : IRequestHandler<DeleteProjectCreditCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICheckPermission _checkPermission;
        private readonly IStringLocalizer<DeleteProjectCreditCommandHandler> _localizer;
        public DeleteProjectCreditCommandHandler(
            IStringLocalizer<DeleteProjectCreditCommandHandler> localizer, 
            IUnitOfWork<int> unitOfWork,ICheckPermission checkPermission)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _checkPermission = checkPermission;
        }

        public async Task<Result<int>> Handle(DeleteProjectCreditCommand request, CancellationToken cancellationToken)
        {
            var credit = await _unitOfWork.Repository<ProjectCredit>()
                .GetByIdAsync(request.Id);
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(credit.ProjectId);
            await _checkPermission.CheckWrightProjectPermission(project.UserId);
            if(credit != null)
            {
                await _unitOfWork.Repository<ProjectCredit>().DeleteAsync(credit);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(credit.Id, _localizer["Credit deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["credit not found"]);
            }
        }
    }
}

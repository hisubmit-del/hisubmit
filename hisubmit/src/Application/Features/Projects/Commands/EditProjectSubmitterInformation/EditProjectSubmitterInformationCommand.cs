using AutoMapper;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;

namespace HiSubmit.Application.Features.Projects.Commands.EditProjectSubmitterInformation
{
    public class EditProjectSubmitterInformationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public AddEditAddressCommand Address { get; set; }

        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
    }

    internal class EditProjectSubmitterInformationCommandHandler : IRequestHandler<EditProjectSubmitterInformationCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICheckPermission _checkPermission;
        private readonly IStringLocalizer<EditProjectSubmitterInformationCommand> _localizer;

        public EditProjectSubmitterInformationCommandHandler
            (IMapper mapper, IUnitOfWork<int> unitOfWork, 
                ICheckPermission checkPermission, 
                IStringLocalizer<EditProjectSubmitterInformationCommand> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _checkPermission = checkPermission;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(EditProjectSubmitterInformationCommand request, CancellationToken cancellationToken)
        {
            if (request.Id != 0)
            {
                var dbProject = await _unitOfWork.Repository<Project>()
                    .Entities
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

                var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.Id);
                await _checkPermission.CheckWrightProjectPermission(project.UserId);
                if (dbProject != null)
                {
                    var updatedProject = _mapper.Map(request, dbProject);
                    await _unitOfWork.Repository<Project>().UpdateAsync(updatedProject);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return await Result<int>.SuccessAsync(dbProject.Id, _localizer["Project updated"]);
                }
            }
            return await Result<int>.FailAsync(_localizer["project not found"]);
        }
    }
}

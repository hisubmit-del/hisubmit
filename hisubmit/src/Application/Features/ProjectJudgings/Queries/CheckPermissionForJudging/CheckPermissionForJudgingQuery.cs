using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.ProjectJudgings.Queries.CheckPermissionForJudging
{
    public record CheckPermissionForJudgingQuery(string projectURL):IRequest<Result<CheckPermissionResponse>>
    {

    }


    public class CheckPermissionForJudgingQueryHandler : IRequestHandler<CheckPermissionForJudgingQuery, Result<CheckPermissionResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        public CheckPermissionForJudgingQueryHandler
            (IUnitOfWork<int> unitOfWork,ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<CheckPermissionResponse>> Handle(CheckPermissionForJudgingQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var judgings =await  _unitOfWork.Repository<ProjectJudging>()
                   .Entities
                   .Include(p=>p.Submit).ThenInclude(p=>p.Project)
                   .Where(p => p.Submit.Project.URL == request.projectURL && p.UserId == currentUserId)
                   .ProjectTo<ProjectJudgingDto>(_mapper.ConfigurationProvider)
                   .ToListAsync();

            return await Result<CheckPermissionResponse>.SuccessAsync(new CheckPermissionResponse() { Judgings=judgings});          
        }
    }


    public class CheckPermissionResponse
    {

        public bool Allowed
        {
            get
            {
                return Judgings.Any();
            }
        }
        public List<ProjectJudgingDto> Judgings { get; set; }
    }
    public class ProjectJudgingDto
    {
        public string FestivalName { get; set; }
        public int FestivalId { get; set; }
        public int SubmitId { get; set; }
        public int Id { get; set; }
    }
         
}

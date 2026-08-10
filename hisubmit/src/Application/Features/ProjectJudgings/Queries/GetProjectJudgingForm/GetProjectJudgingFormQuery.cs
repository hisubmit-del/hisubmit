using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.ProjectJudgings.Queries.GetProjectJudgingForm
{
    public class GetProjectJudgingFormQuery : IRequest<Result<GetProjectJudgingFormResponse>>
    {
        public int SubmitId { get; set; }
    }

    public class GetProjectJudgingFormQueryHandler : IRequestHandler<GetProjectJudgingFormQuery, Result<GetProjectJudgingFormResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetProjectJudgingFormQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetProjectJudgingFormResponse>> Handle(GetProjectJudgingFormQuery request, CancellationToken cancellationToken)
        {
            var FestivalId_ProjectType =await _unitOfWork.Repository<Submit>()
                 .Entities.Where(p => p.Id == request.SubmitId)
                 .Select(p => new
                 {
                     ProjectType = p.Project.ProjectType,
                     FestivalId = p.FestivalId
                 })
                 .FirstOrDefaultAsync() ;

            var judging = await _unitOfWork.Repository<Judging>().Entities
                .Where(p => p.FestivalId == FestivalId_ProjectType.FestivalId && p.ProjectType == FestivalId_ProjectType.ProjectType)
                .FirstOrDefaultAsync();
            if(judging == null)
            {
                throw new ApiException();
            }

            throw new NotImplementedException();

        }
    }

    public class GetProjectJudgingFormResponse
    {

    }
}

using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Judgings.Commands.AddEditJudgiingButton
{
    public class AddEditJudgingFiledCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int JudgingId { get; set; }
    }
    public class AddEditJudgingFiledCommandHandler : IRequestHandler<AddEditJudgingFiledCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditJudgingFiledCommandHandler> _localizer;
        public AddEditJudgingFiledCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork,
            IStringLocalizer<AddEditJudgingFiledCommandHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditJudgingFiledCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                var Filed = _mapper.Map<JudgingFiled>(request);
                await _unitOfWork.Repository<JudgingFiled>().AddAsync(Filed);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(Filed.Id, _localizer["Filed Added"]);
            }
            else
            {
                var dbFiled = await _unitOfWork.Repository<JudgingFiled>().GetByIdAsync(request.Id);
                if (dbFiled != null)
                {
                    var updatedFiled = _mapper.Map(request, dbFiled);
                    await _unitOfWork.Repository<JudgingFiled>().UpdateAsync(updatedFiled);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(dbFiled.Id, _localizer["Filed updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Filed not found"]);
                }
            }
        }
    }
}

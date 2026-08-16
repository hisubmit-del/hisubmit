using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Judgings.Commands.AddEditJudgingButton
{
    public class AddEditJudgingButtonCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int JudgingId { get; set; }
    }
    public class AddEditJudgingButtonCommandHandler : IRequestHandler<AddEditJudgingButtonCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditJudgingButtonCommandHandler> _localizer;
        public AddEditJudgingButtonCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork, 
            IStringLocalizer<AddEditJudgingButtonCommandHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditJudgingButtonCommand request, CancellationToken cancellationToken)
        {
            if(request.Id == 0)
            {
                var button = _mapper.Map<JudgingButton>(request);
                await _unitOfWork.Repository<JudgingButton>().AddAsync(button);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(button.Id, _localizer["button Added"]);
            }
            else
            {
                var dbButton =await _unitOfWork.Repository<JudgingButton>().GetByIdAsync(request.Id);
                if(dbButton != null)
                {
                    var updatedButton = _mapper.Map(request, dbButton);
                    await _unitOfWork.Repository<JudgingButton>().UpdateAsync(updatedButton);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(dbButton.Id, _localizer["button updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["button not found"]);
                }
            }
        }
    }
}

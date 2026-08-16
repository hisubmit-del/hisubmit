using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.FestivalFocs.Commands.AddEditFestivalFocus
{
    public class AddEditFestivalFocusCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class AddEditFestivalFocusCommandHandler : IRequestHandler<AddEditFestivalFocusCommand, Result<int>>
    {
        private readonly IStringLocalizer<AddEditFestivalFocusCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        public AddEditFestivalFocusCommandHandler(
            IStringLocalizer<AddEditFestivalFocusCommandHandler> localizer,
            IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditFestivalFocusCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                var festivalFocus = _mapper.Map<FestivalFocus>(request);
                await _unitOfWork.Repository<FestivalFocus>().AddAsync(festivalFocus);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalFocusCacheKey);
                return await Result<int>.SuccessAsync(festivalFocus.Id, _localizer["Festival focus Added"]);
            }
            else
            {
                var festival = await _unitOfWork.Repository<FestivalFocus>().GetByIdAsync(request.Id);
                if (festival != null)
                {
                    var updatedFocus = _mapper.Map(request, festival);
                    await _unitOfWork.Repository<FestivalFocus>().UpdateAsync(updatedFocus);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalFocusCacheKey);
                    return await Result<int>.SuccessAsync(updatedFocus.Id, _localizer["Festival focus Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["festival focus not found"]);
                }
            }
        }
    }
}

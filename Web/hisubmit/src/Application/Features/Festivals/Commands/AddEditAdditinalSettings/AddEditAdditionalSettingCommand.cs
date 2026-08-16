using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
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
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditAdditinalSettings
{
    public class AddEditAdditionalSettingCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public List<int> FestivalFestivalFociId { get; set; } = new();
        public List<int> FestivalArtCategoriesId { get; set; } = new();
        public bool Public { get; set; }
        public string SearchTerms { get; set; }

        public bool AllLenghtAccepted { get; set; }
        public int? MinimomLenght { get; set; }
        public int? MaximomLenght { get; set; }
        public string URL { get; set; }

        //Tracking Sequence
        public int StartingNumber { get; set; }
        public string Prefix { get; set; }
        
        public  FestivalStatus FestivalStatus { get; set; }
        
        public bool ChangesNotAllowed { get; set; }
    }

    public class AddEditAdditionalSettingCommandHandler : IRequestHandler<AddEditAdditionalSettingCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditAdditionalSettingCommandHandler> _loclizer;
        public AddEditAdditionalSettingCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditAdditionalSettingCommandHandler> loclizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _loclizer = loclizer;
        }

        public async Task<Result<int>> Handle(AddEditAdditionalSettingCommand request, CancellationToken cancellationToken)
        {
            var festivalDb = await _unitOfWork.Repository<Festival>().GetByIdAsync(request.Id);
            if (festivalDb != null)
            {
                var updatedFestival = _mapper.Map(request, festivalDb);
                if (string.IsNullOrWhiteSpace(request.URL))
                {
                    request.URL = $"{festivalDb.Name.Trim()}";
                }
                await UpdateFestivalArtCategory(request.FestivalArtCategoriesId,request.Id);
                await UpdateFestivalFestivalfocus(request.FestivalFestivalFociId, request.Id);
                await _unitOfWork.Repository<Festival>().UpdateAsync(updatedFestival);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalCacheKey);
                return await Result<int>.SuccessAsync(_loclizer["Festival Updated"]);
            }
            else
            {
                return await Result<int>.FailAsync(_loclizer["Festival Not Found"]);
            }
        }

        private async Task UpdateFestivalFestivalfocus(List<int> festivalFoci,int festivalId)
        {
            var dbffFocus = _unitOfWork.Repository<FestivalFestivalFocus>().Entities
                .Where(p => p.FestivalId == festivalId);

            var deletedFFocus = dbffFocus.Where(deadlneCat => !festivalFoci.Any(id => id == deadlneCat.Id))
               .ToList();

            var addedFestivalFocus = festivalFoci.Where(id => !dbffFocus.Any(focus => focus.Id == id))
                .ToList();

            if (deletedFFocus != null)
            {
                foreach (var item in deletedFFocus)
                {
                    await _unitOfWork.Repository<FestivalFestivalFocus>().DeleteAsync(item);
                }
            }
            if (addedFestivalFocus != null)
            {
                foreach (var item in addedFestivalFocus)
                {
                    await _unitOfWork.Repository<FestivalFestivalFocus>().AddAsync(new FestivalFestivalFocus()
                    {
                        FestivalId=festivalId,
                        FestivalFocusId=item
                    });
                }
            }
        }
        private async Task UpdateFestivalArtCategory(List<int> artCategories, int festivalId)
        {
            var dbFestivalArtCategory = _unitOfWork.Repository<FestivalArtCategory>().Entities
                .Where(p => p.FestivalId == festivalId);

            var deletedFestivalArtCategory = dbFestivalArtCategory.Where(deadlneCat => !artCategories.Any(id => id == deadlneCat.Id))
               .ToList();

            var addedFestivalArtCategory = artCategories.Where(id => !dbFestivalArtCategory.Any(focus => focus.Id == id))
                .ToList();

            if (deletedFestivalArtCategory != null)
            {
                foreach (var item in deletedFestivalArtCategory)
                {
                    await _unitOfWork.Repository<FestivalArtCategory>().DeleteAsync(item);
                }
            }
            if (addedFestivalArtCategory != null)
            {
                foreach (var item in addedFestivalArtCategory)
                {
                    await _unitOfWork.Repository<FestivalArtCategory>().AddAsync(new FestivalArtCategory()
                    {
                        FestivalId = festivalId,
                        ArtCategoryId = item
                    });
                }
            }
        }
    }
}

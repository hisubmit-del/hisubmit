using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services.Text;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditEventCategory
{
    public class AddEditEventCategoryCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FestivalId { get; set; }
        //RunTime
        public ProjectType? ProjectType { get; set; }
        public RuntimeType? RuntimeType { get; set; }
        public int FirstRunTimeValue { get; set; }
        public int? SecoundRunTimeValue { get; set; }

        public bool RequirePassword { get; set; }
        public string Password { get; set; }
        public bool StudentProject { get; set; }

        //Locations
        public LocationType? LocationType { get; set; }
        //public int CountryId { get; set; }
        public List<int> CountriesId { get; set; }
        public string CityOrStateName { get; set; }
        public List<UpdateDeadlineCategoryonFee> CategoryonFees { get; set; }

        public AddEditEventCategoryCommand()
        {
            CountriesId = new List<int>();
        }
    }

    public class AddEditEventCategoryCommandHandler : IRequestHandler<AddEditEventCategoryCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditEventCategoryCommandHandler> _localizer;
        private readonly IMapper _mapper;

        public AddEditEventCategoryCommandHandler(
            IUnitOfWork<int> unitOfWork, IMapper mapper,
            IStringLocalizer<AddEditEventCategoryCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditEventCategoryCommand request, CancellationToken cancellationToken)
        {
            request.Name = request.Name?.Trim();
            request.Description = HtmlTextSanitizer.SanitizeWithoutLinks(request.Description);

            if (request.Id == 0)
            {
                var category = _mapper.Map<EventCategory>(request);
                //Add Event Category Deadline Fess
                var categoriesDeadLine = _mapper.Map<List<DeadlineEventCategory>>(request.CategoryonFees);
                category.DeadlineEventCategories = categoriesDeadLine;
                category.EventCategoryCountries = new List<EventCategoryCountry>();
                foreach (var cId in request.CountriesId)
                {
                    category.EventCategoryCountries.Add(new EventCategoryCountry()
                    {
                        CountryId = cId
                    });
                }

                await _unitOfWork.Repository<EventCategory>().AddAsync(category);
                await UpdateMinMaxFeeInFestival(request.CategoryonFees, request.FestivalId);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken,
                    ApplicationConstants.Cache.GetAllEventCategoryCacheKefy);
                

                
                
                return await Result<int>.SuccessAsync(category.Id, _localizer["Added Category"]);
            }
            else
            {
                var dbCategory = await _unitOfWork.Repository<EventCategory>().GetByIdAsync(request.Id);
                if (dbCategory != null)
                {
                    var updatedCategory = _mapper.Map(request, dbCategory);
                    await UpdateDeadLineCategory(request.CategoryonFees);
                    await UpdateCountries(request.CountriesId, request.Id);
                    await _unitOfWork.Repository<EventCategory>().UpdateAsync(updatedCategory);
                    await UpdateMinMaxFeeInFestival(request.CategoryonFees, request.FestivalId);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken,
                        ApplicationConstants.Cache.GetAllEventCategoryCacheKefy);
                    return await Result<int>.SuccessAsync(updatedCategory.Id, _localizer["Updated Category"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Event Category not Found"]);
                }
            }
        }

        private async Task UpdateDeadLineCategory(List<UpdateDeadlineCategoryonFee> fees)
        {
            if (fees is { Count: > 0 })
            {
                var feesId = fees.Select(p => p.Id);
                var deadLineEventCategory = await _unitOfWork.Repository<DeadlineEventCategory>()
                    .Entities.Where(deadLineEvent => feesId.Any(id => id == deadLineEvent.Id))
                    .ToListAsync();

                foreach (var fee in fees)
                {
                    var deadLineEventCatDb = deadLineEventCategory.FirstOrDefault(p => p.Id == fee.Id);
                    var UpdateddeadLineEvent = _mapper.Map(fee, deadLineEventCatDb);
                    await _unitOfWork.Repository<DeadlineEventCategory>().UpdateAsync(UpdateddeadLineEvent);
                }
            }
        }

        private async Task UpdateMinMaxFeeInFestival
            (List<UpdateDeadlineCategoryonFee> catFees,int festivalId)
        {
            var festival = await _unitOfWork.Repository<Festival>()
                .GetByIdAsync(festivalId);
            var f = new List<double>();
            
            foreach (var d in catFees)
            {
                if (d.GoldFee != null)
                    f.Add(d.GoldFee.Value);
                if (d.StudentFee != null)
                    f.Add(d.StudentFee.Value);
                if (d.StandardFee != null)
                    f.Add(d.StandardFee.Value);
            }

            if (festival.MinFee != null)
            {
                if (festival.MinFee > f.Min())
                    festival.MinFee = f.Min();
            }
            else
            {
                festival.MinFee = f.Min();
            }
            
            if (festival.MaxFee !=null)
            {
                if (festival.MaxFee < f.Max())
                    festival.MaxFee = f.Max();
            }
            else
            {
                festival.MaxFee = f.Max();
            }

            await _unitOfWork.Repository<Festival>()
                .UpdateAsync(festival);
        }
        
        private async Task UpdateCountries(List<int> countriesId, int categoryId)
        {
            var dbFestivalArtCategory = _unitOfWork.Repository<EventCategoryCountry>().Entities
                .Where(p => p.EventCategoryId == categoryId);

            var deletedFestivalArtCategory = dbFestivalArtCategory.Where(cId => countriesId.All(id => id != cId.Id))
                .ToList();

            var addedFestivalArtCategory = countriesId.Where(id => !dbFestivalArtCategory.Any(focus => focus.Id == id))
                .ToList();

            if (deletedFestivalArtCategory != null)
            {
                foreach (var item in deletedFestivalArtCategory)
                {
                    await _unitOfWork.Repository<EventCategoryCountry>().DeleteAsync(item);
                }
            }

            if (addedFestivalArtCategory != null)
            {
                foreach (var item in addedFestivalArtCategory)
                {
                    await _unitOfWork.Repository<EventCategoryCountry>().AddAsync(new EventCategoryCountry()
                    {
                        EventCategoryId = categoryId,
                        CountryId = item
                    });
                }
            }
        }
    }
    
}

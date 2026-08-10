using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Festivals.Queries.GetDeadLineById;
using HiSubmit.Client.SharedModels.Extensions;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditDeadLineEntry;

public class AddEditDeadLineEntryCommand : AddEditDeadLineEntryRequest, IRequest<Result<GetDeadLineByIdResponse>>;

public class AddEditDeadLineEntryCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<AddEditDeadLineEntryCommand> stringLocalize)
    : IRequestHandler<AddEditDeadLineEntryCommand, Result<GetDeadLineByIdResponse>>
{
    public async Task<Result<GetDeadLineByIdResponse>> Handle(AddEditDeadLineEntryCommand request, CancellationToken cancellationToken)
    {
        var deadLineDates = await unitOfWork.Repository<DeadLine>()
            .Entities.Where(p=>p.FestivalId==request.FestivalId).Select(p => p.Date).ToListAsync(cancellationToken);

        var festival = await unitOfWork.Repository<Festival>()
            .Entities.Where(p => p.Id == request.FestivalId)
            .FirstOrDefaultAsync(cancellationToken);
            
        if (festival.OpeningDate > request.Date)
        {
            return await Result<GetDeadLineByIdResponse>
            .FailAsync($"Date must be after opening date ({festival.OpeningDate.Value.ToLongDate()})");
        }

        if (festival.NotificationDate < request.Date)
        {
            return await Result<GetDeadLineByIdResponse>
                .FailAsync($"Date must be before notification date ({festival.NotificationDate.Value.ToLongDate()})");
        }

        if (request.Id != 0)
        {
            var deadLine = await unitOfWork.Repository<DeadLine>().GetByIdAsync(request.Id);
            deadLineDates.Remove(deadLine.Date);
        }

        if (!request.ApplyToAllCategory && request.CategoryId.Count == 1)
        {
        }
        else
        {
            if (deadLineDates.Select(date => date - request.Date)
                .Any(dateSpan => dateSpan?.Days.AbsoluteValue() < 14))
            {
                return await Result<GetDeadLineByIdResponse>
                    .FailAsync("The set dates must not be closer than 2 weeks period");
            }
        }
       

        if (request.Id == 0)
        {
            var deadLine = mapper.Map<DeadLine>(request);
            if (request.ApplyToAllCategory)
            {
                var allCatsId = await unitOfWork.Repository<EventCategory>()
                    .Entities.Where(p => p.FestivalId == request.FestivalId)
                    .Select(p => p.Id).ToListAsync(cancellationToken);

                var catList = allCatsId
                    .Select(catId => new DeadlineEventCategory() {EventCategoryId = catId})
                    .ToList();

                deadLine.DeadlineEventCategories = catList;
            }
            else
            {
                var catList = new List<DeadlineEventCategory>();
                if (request.CategoryId != null)
                {
                    catList.AddRange(request.CategoryId.Select
                        (catId => new DeadlineEventCategory() {EventCategoryId = catId,}));
                }

                deadLine.DeadlineEventCategories = catList;
            }

            await unitOfWork.Repository<DeadLine>().AddAsync(deadLine);
            await unitOfWork.CommitAndRemoveCache(cancellationToken,
                ApplicationConstants.Cache.GetAllDeadLineCacheKey);
            return await Result<GetDeadLineByIdResponse>
                .SuccessAsync(mapper.Map<GetDeadLineByIdResponse>(deadLine), stringLocalize["DeadLine Added"]);
        }
        else
        {
            var deadLine = await unitOfWork.Repository<DeadLine>().GetByIdAsync(request.Id);
            if (deadLine != null)
            {
                var updatedDeadline = mapper.Map(request, deadLine);
                if (!request.ApplyToAllCategory)
                {
                    await UpdateCategories(request.CategoryId, request.Id);
                }
                else
                {
                    updatedDeadline.DeadlineEventCategories = null;
                }

                await unitOfWork.Repository<DeadLine>().UpdateAsync(updatedDeadline);
                await unitOfWork.CommitAndRemoveCache(cancellationToken,
                    ApplicationConstants.Cache.GetAllDeadLineCacheKey);
                return await Result<GetDeadLineByIdResponse>.SuccessAsync(mapper.Map<GetDeadLineByIdResponse>(updatedDeadline)
                    ,stringLocalize["DeadLine Updated"]);
            }
            else
            {
                return await Result<GetDeadLineByIdResponse>.FailAsync(stringLocalize["DeadLine not Found"]);
            }
        }
    }

    private async Task UpdateCategories(IReadOnlyCollection<int> catsId, int deadLineId)
    {
        var deadLineCats = await unitOfWork.Repository<DeadlineEventCategory>().Entities
            .Where(p => p.DeadLineId == deadLineId)
            .ToListAsync();
        var deletedCats = deadLineCats.Where(deadlineCat => catsId.All(id => id != deadlineCat.Id))
            .ToList();
        var addedCats = catsId.Where(id => deadLineCats.All(deadLine => deadLine.Id != id))
            .ToList();
        foreach (var item in deletedCats)
        {
            await unitOfWork.Repository<DeadlineEventCategory>().DeleteAsync(item);
        }

        foreach (var item in addedCats)
        {
            await unitOfWork.Repository<DeadlineEventCategory>().AddAsync(new DeadlineEventCategory()
            {
                EventCategoryId = item,
                DeadLineId = deadLineId
            });
        }
    }
}
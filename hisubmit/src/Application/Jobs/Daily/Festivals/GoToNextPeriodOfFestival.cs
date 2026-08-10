using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.Festivals;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Jobs.Daily.Festivals;

public interface IGoToNextPeriodOfFestival
{
    Task InvokeAsync();
}

public class GoToNextPeriodOfFestival(
    IBackGroundJobService backGroundJobService,
    IUnitOfWork<int> unitOfWork,
    IMediator mediator)
    : IGoToNextPeriodOfFestival
{

    public async Task InvokeAsync()
    {
        backGroundJobService.AddRecurring(() => CreateNewPeriod(), CornJob.Daily, 0, "GoNextFestivalPeriod");
    }

    public async Task CreateNewPeriod()
    {
        var festivals = unitOfWork.Repository<Festival>()
            .Entities
            .Where(p => p.EventEndDate != null && p.IsActivePeriod &&
                        p.EventEndDate.Value <= DateTime.Today)
            .ToList();

        foreach (var festival in festivals)
        {
           var newFestival = festival.Clone();
           
            newFestival.YearsRunning++;
            newFestival.Id = 0;
            newFestival.IsActivePeriod = true;
            festival.IsActivePeriod = false;
            newFestival.IsActive = false;

            if (festival.OpeningDate != null)
                newFestival.OpeningDate = festival.OpeningDate.Value.AddYears(1);
            if (festival.NotificationDate != null)
                newFestival.NotificationDate = festival.NotificationDate.Value.AddYears(1);
            if (festival.EventStartDate != null)
                newFestival.EventStartDate = festival.EventStartDate.Value.AddYears(1);
            if (festival.EventEndDate != null) newFestival.EventEndDate = festival.EventEndDate.Value.AddYears(1);

            var f = await unitOfWork.Repository<Festival>().AddAsync(newFestival);

            await unitOfWork.Repository<Festival>().UpdateAsync(festival);

            await unitOfWork.SaveChangesAsync(CancellationToken.None);

            //Update ProductFestivalId Master 

            var festivalMaster = await unitOfWork
                .Repository<FestivalMaster>().GetByIdAsync(festival.FestivalMasterId);

            festivalMaster.ActiveId = f.Id;
            festivalMaster.ActivePeriod = newFestival.YearsRunning;

            await unitOfWork.Repository<FestivalMaster>().UpdateAsync(festivalMaster);
            await unitOfWork.SaveChangesAsync(CancellationToken.None);

            await mediator.Publish(new CreatedFestival
            {
                Id = f.Id
            });
        }
    }
}
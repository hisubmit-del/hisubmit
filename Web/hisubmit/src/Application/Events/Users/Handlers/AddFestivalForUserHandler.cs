using AutoMapper;
using HiSubmit.Application.Events.Festivals;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Events.Users.Handlers;

public class AddFestivalForUserHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IMediator mediator)
    : INotificationHandler<FestivalUserRegisteredEvent>
{
    public async Task Handle(FestivalUserRegisteredEvent notification, CancellationToken cancellationToken)
    {
          
        var festival = mapper.Map<Festival>(notification);
        //by default festival showing in site
        festival.Public = true;
        festival.YearsRunning = 1;
      

        var festivalMaster = new FestivalMaster
        {
            ActivePeriod = 1,
            UserId = notification.UserId,
            Name = notification.FestivalName,
            ActiveId=festival.Id,            
            Festivals = [festival]
        };
        
        
        
        var en=await unitOfWork.Repository<FestivalMaster>().AddAsync(festivalMaster);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var festiv2Al =await unitOfWork.Repository<Festival>()
            .Entities
            .Where(p=>p.FestivalMasterId==en.Id)
            .Include(p=>p.FestivalMaster)
            .FirstOrDefaultAsync(cancellationToken);

        festiv2Al.FestivalMaster.ActiveId = festiv2Al.Id;
        festiv2Al.IsActivePeriod = true;
        await unitOfWork.Repository<Festival>().UpdateAsync(festiv2Al);

        //festivalMaster.ActiveId=en.Festivals.First().Id;
        //await unitOfWork.Repository<FestivalMaster>().UpdateAsync(festivalMaster);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new CreatedFestival() { Id=festival.Id}, cancellationToken);
    }
}
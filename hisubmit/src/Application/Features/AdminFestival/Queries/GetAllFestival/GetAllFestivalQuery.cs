using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Specifications.Festivals;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;

namespace HiSubmit.Application.Features.AdminFestival.Queries.GetAllFestival;

public class GetAllFestivalQuery :GetAllFestivalRequest
    , IRequest<PaginatedResult<GetAllFestivalResponse>>
{
    public bool? IsActivePeriod { get; set; }
}

public class GetAllFestivalQueryHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IUserService userService)
    : IRequestHandler<GetAllFestivalQuery, PaginatedResult<GetAllFestivalResponse>>
{
    public async Task<PaginatedResult<GetAllFestivalResponse>> Handle
        (GetAllFestivalQuery request, CancellationToken cancellationToken)
    {
        var docSpec = new FestivalFilterSpecification
            (request);

        var data = await unitOfWork.Repository<Festival>().Entities
            .Include(p => p.Address).ThenInclude(p => p.Country)
            .Include(p=>p.FestivalFestivalFoci)
            .Include(p=>p.DeadLines)
            .Include(p=>p.FestivalFestivalQualifyings)
            .Include(p=>p.Venues).ThenInclude(p=>p.Tickets)
            .Specify(docSpec)
            .ProjectTo<GetAllFestivalResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        foreach (var d in data.Data)
        {
            if (DateTime.Now < d.OpeningDate)
            {
                d.NearDeadline = d.OpeningDate;
                d.DateTitle = d.OpeningDate.ToLongDateString();
                d.FestivalDateStatus = FestivalDateStatus.OpenSoon;
            }
            else
            {
                var nearDeadLine =
                    d.DeadLines.Where(p => p.Date > DateTime.Now)
                        .MinBy(p => p.Date);

                if (nearDeadLine == null)
                {
                    if (d.NotificationDate != null && DateTime.Now < d.NotificationDate.Value)
                    {
                        d.NearDeadline = d.NotificationDate;
                        d.DateTitle = "Notification Date";
                        d.FestivalDateStatus = FestivalDateStatus.Closed;   
                    }
                    else
                    {
                        d.NearDeadline = d.EventStartDate;
                        d.DateTitle = "Event Date";
                        d.FestivalDateStatus = FestivalDateStatus.Closed;   
                    }
                }
                else
                {
                    d.NearDeadline = nearDeadLine.Date;
                    d.DateTitle = "Next Deadline";
                    d.FestivalDateStatus = FestivalDateStatus.Submit;
                }
            }

            d.DeadLines = null;
        }
        
        
        var userNames = await userService.GetUser(data.Data.Select(p => p.UserId).ToList());

        if (userNames != null)
        {
            foreach (var item in data.Data.Where(p => p.UserId != null))
            {
                var u = userNames[item.UserId];
                item.UserName = u.UserName;
                item.AccountEmail = u.Email;
                item.AccountFullName = u.FullName;
            }
        }
        return data;
    }
}

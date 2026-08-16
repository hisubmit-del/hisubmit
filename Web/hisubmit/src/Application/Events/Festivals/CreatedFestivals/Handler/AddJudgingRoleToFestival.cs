using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Client.SharedModels.Constants.Role;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.Handler;

public class AddJudgingRoleToFestival(IRoleService roleService) 
    : INotificationHandler<CreatedFestival>
{
    public async Task Handle(CreatedFestival notification, CancellationToken cancellationToken)
    {
       var f= await roleService.SaveAsync(new RoleRequest()
        {
            FestivalId = notification.Id,
            Name = RoleConstants.Referee
        });
    }
}
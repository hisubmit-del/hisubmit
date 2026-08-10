using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Events.FestivalAddUsers.handlers
{
    public class FestivalAddUserEventHandler : INotificationHandler<FestivalAddUserEvent>
    {
        private readonly IUnitOfWork<int> _unitOfwork;
        public FestivalAddUserEventHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfwork = unitOfWork;
        }
        public async Task Handle(FestivalAddUserEvent notification, CancellationToken cancellationToken)
        {
            var festivalUser = new FestivalSubUser()
            {
                FestivalId=notification.FestivalId,
                UserId=notification.UserId
            };

            await _unitOfwork.Repository<FestivalSubUser>().AddAsync(festivalUser);
            await _unitOfwork.SaveChangesAsync(cancellationToken);
        }
    }
}

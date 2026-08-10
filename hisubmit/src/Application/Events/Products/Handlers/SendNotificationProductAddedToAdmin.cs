using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Products.Handlers
{
    public class SendNotificationProductAddedToAdmin
        (INotificationService notificationService):INotificationHandler<AddProductByFestivalEvent>
    {
        public async Task Handle(AddProductByFestivalEvent notification, CancellationToken cancellationToken)
        {
            await notificationService.AddAdminNotificationJob(new AddAdminNotificationRequest()
            {
                Link = $"admin/festival/products/{notification.FestivalId}",
                Title = $"{notification.FestivalName} Festival has defined a new product",
                NotificationType = NotificationType.AdminNewAddedProduct
            }); ;
        }
    }
}

using MediatR;

namespace HiSubmit.Application.Events.Products;

public class AddProductByFestivalEvent:INotification
{
    public int ProductId { get; set; }
    public int FestivalId { get; set; }
    public string FestivalName { get; set; }
}
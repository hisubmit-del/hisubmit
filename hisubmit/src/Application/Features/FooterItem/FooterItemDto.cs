using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.FooterItems;

public class FooterItemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public  bool IsEnable { get; set; }
    public  MenuItemPosition Position { get; set; }

}
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Content;

public class MenuItem:AuditableEntity<int>
{
    public  string Title { get; set; }
    public  string Link { get; set; }
    public  MenuItemPosition Position { get; set; }
}
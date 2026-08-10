using Hisubmit.Client.SharedModels.Enums;
namespace Hisubmit.Client.SharedModels.Features.Comments.Queries;

public class GetAllCommentsQuery
{
    public  int? FestivalId { get; set; }
    public  string UserId { get; set; }
    public  CommentType Type { get; set; }
    public  int? ParentId { get; set; }
}


public class CommentDto
{
    
}

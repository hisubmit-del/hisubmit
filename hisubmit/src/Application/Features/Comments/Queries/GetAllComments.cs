using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Comments.Queries;

public class GetAllCommentsQuery:IRequest<PaginatedResult<CommentDto>>
{
    public  int? FestivalId { get; set; }
    public  string UserId { get; set; }
    public  CommentType Type { get; set; }
    public  int? ParentId { get; set; }
}


public class CommentDto
{
    
}

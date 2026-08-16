using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Reviews.Commands;

public class AddReviewCommand 
{
    public string Text { get; set; }
    public string UserId { get; set; }
   
    public int Rate { get; set; }
    public CommentType Type { get; set; }
    public int FestivalId { get; set; }
}

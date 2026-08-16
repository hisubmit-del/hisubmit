
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Comments.Commands;

public class AddCommentCommand
{
    public string Text { get; set; }
    public string Title { get; set; }
    public bool ShowInSite { get; set; }
    public CommentType Type { get; set; }
    public bool ShowFestival { get; set; }

    public int? FestivalId { get; set; }

    public int? ParentId { get; set; }
}


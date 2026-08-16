namespace Hisubmit.Client.SharedModels.Features.News.Commands;

public class UpdateEnableNewCommand
{
    public  int Id { get; set; }
    public bool IsEnable { get; set; }
    public int? FestivalId { get; set; }
}

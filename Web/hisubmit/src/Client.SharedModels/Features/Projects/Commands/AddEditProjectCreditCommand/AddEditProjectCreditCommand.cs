using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;

public class UpdateProjectCreditsRequest 
{
    public List<AddEditProjectCreditCommand> Credits { get; set; }
    public int ProjectId { get; set; }
}

public class AddEditProjectCreditCommand
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public List<AddEditProjectCreditItemCommand> ProjectItemPeople { get; set; }
    public int ProjectId { get; set; }
    
}

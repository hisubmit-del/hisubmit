using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;

public class AddEditProjectCreditItemCommand
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string PriorCredit { get; set; }
    
    public int ProjectCreditId { get; set; }
}
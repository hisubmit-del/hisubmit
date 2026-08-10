using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Features.DocumentTypes.Commands.AddEdit;

public class AddEditDocumentTypeCommand 
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
}
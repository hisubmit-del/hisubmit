using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Documents.Commands.AddEdit;

public partial class AddEditDocumentCommand 
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public bool IsPublic { get; set; } = false;
    [Required]
    public string URL { get; set; }
    [Required]
    public int DocumentTypeId { get; set; }
    public UploadRequest UploadRequest { get; set; }
}
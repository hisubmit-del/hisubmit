//using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile;

public class UploadProjectFileCommand 
{
    public int ProjectId { get; set; }
    public int Fragment { get; set; }
  //  public IFormFile FormFile { get; set; }
}

public enum ProjectFilePosition : short
{
    [Display(Name="Header")]
    Header = 0,
    [Display(Name = "Side Bar ")]
    SideBarFile = 1,
    [Display(Name="Gallery")]
    Gallery = 2
}


using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Project;

public partial class ProjectFileConflictWays
{
    [CascadingParameter]
    public IMudDialogInstance DialogInstance { get; set; }

    private ConflictWays _conflictWays;


    private Task Cancel(MouseEventArgs arg)
    {
        DialogInstance.Cancel();
        return Task.CompletedTask;
    }

    private Task SaveAsync(MouseEventArgs arg)
    {
        DialogInstance.Close(_conflictWays);
        return Task.CompletedTask;
    }
}
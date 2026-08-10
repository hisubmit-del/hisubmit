using System;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;
using Microsoft.AspNetCore.Components.Forms;

namespace HiSubmit.Web.Components.Pages.Project.Awards;

public partial class ProjectScreenAwards
{
    #region Inject

    [Inject]
    private IProjectManager ProjectManager { get; set; }
    [Inject]
    private IMapper Mapper { get; set; }

    #endregion

    #region Parameters

    [CascadingParameter]
    public int ProjectId { get; set; }

    #endregion

    public UpdateScreenWritingRequest Request=new();
  //  public List<AddEditScreenWritingRequest> _model { get; set; } = new();
    public FluentValidationValidator _fluentValidationValidator;

    public bool Validated { get; set; }

    public bool _Loaded = false;
    private EditContext _editContext;
    protected override async Task OnInitializedAsync()
    {
        await LoadScreenAwards();
        _editContext = new EditContext(Request);
        await base.OnInitializedAsync();
        _Loaded = true;
    }


    private async Task LoadScreenAwards()
    {
        var response = await ProjectManager.DetailScreenAward(new GetScreenAwardRequest()
        {
            ProjectId = ProjectId
        });

        if (response.Succeeded)
        {
            if (response.Data.Any())
            {
                Request.ScreenWritings=Mapper.Map<List<AddEditScreenWritingRequest>>(response.Data);
                
                //_model = Mapper.Map<List<AddEditScreenWritingRequest>>(response.Data);
            }
            else
            {
                Request.ScreenWritings = new List<AddEditScreenWritingRequest>
                {
                    new AddEditScreenWritingRequest()
                    {
                        ProjectId = ProjectId,
                        ScreeningDate = DateTime.Today
                    }
                };
            }
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }


    private async Task DeleteAward(AddEditScreenWritingRequest request)
    {
        await Task.Run(() => { Request.ScreenWritings.Remove(request); });
    }
    private async Task AddAward()
    {
        await Task.Run(() => { Request.ScreenWritings.Add(new AddEditScreenWritingRequest() { ProjectId = ProjectId ,UploadRequest = new UploadRequest(){UploadType = UploadType.Awards}}); });
    }

    public bool CheckValid()
    {
        Validated = _fluentValidationValidator.Validate((p) => p.IncludeAllRuleSets());

        return Validated;
    }

    public void SetUnModifiedForm()
    {
        _editContext.MarkAsUnmodified();
    }

    public bool ModifiedForm()
    {
        return _editContext.IsModified();
    }
}
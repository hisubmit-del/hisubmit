using AutoMapper;
using Blazored.FluentValidation;
using FluentValidation;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Client.SharedModels.Extensions;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.DeleteProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateProjectFileOrder;
using HiSubmit.Client.Pages.Project.Files;
using MudBlazor;

namespace HiSubmit.Client.Pages.Project;

public partial class AddEditProjectFile
{
    #region Injection

    [Inject] private IProjectManager ProjectManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }

    #endregion

    #region Parameters

    [CascadingParameter] public int ProjectId { get; set; }

    #endregion

    #region Private Filled

    private List<GetAllProjectFileResponse> ProjectFiles { get; set; }
    private bool _loaded = false;
    private bool _uploadLocalFile;
    private bool _addFileForm;

    #endregion

    #region ChildComponent Ref

    private LocalFile _localFile;
    private ExternalFile _externalFile;
    private MudDropContainer<GetAllProjectFileResponse> _FilesZone;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadProjectFileUrl();
        await base.OnInitializedAsync();
        _loaded = true;
    }

    #endregion

    private async Task LoadProjectFileUrl()
    {
        var response = await ProjectManager.GetAllFiles(new GetAllProjectFilesQuery() { ProjectId = ProjectId });
        if (response.Succeeded)
        {
            ProjectFiles = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }


    private async Task DeleteFileUrl(int id)
    {
        var response = await ProjectManager.DeleteProjectFile(new DeleteProjectFilesCommand()
        {
            Id = id
        });
        if (response.Succeeded)
        {
            await LoadProjectFileUrl();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private Task ShowUploadForm()
    {
        _addFileForm = true;
        return Task.CompletedTask;
    }

    private async Task ReloadData()
    {
        await LoadProjectFileUrl();
    }

    private async Task AddedFile()
    {
        _addFileForm = false;
        await LoadProjectFileUrl();
    }

    private Task Cancel()
    {
        _addFileForm = false;
        return Task.CompletedTask;
    }

    public bool ModifiedForm()
    {
        if (!_addFileForm)
            return false;

        return _uploadLocalFile ? _localFile.ModifiedForm() : _externalFile.ModifiedForm();
    }

    public async Task<bool> SaveAsync()
    {
        if (!_addFileForm)
            return false;

        if (_uploadLocalFile)
            return await _localFile.SaveAsync();

        return await _externalFile.SaveAsync();
    }

    private MudDropZone<GetAllProjectFileResponse> _mud;

    private void ItemUpdated(MudItemDropInfo<GetAllProjectFileResponse> dropItem)
    {
        ChangeOrdering(dropItem.Item, dropItem.IndexInZone);
    }

    private async Task ChangeOrdering(GetAllProjectFileResponse file, int newOrder)
    {
        var oldOrder = file.Order;
        var selectedItem = ProjectFiles.FirstOrDefault(p => p.Order == oldOrder);
        if (newOrder < oldOrder)
            foreach (var item in ProjectFiles
                         .Where(p => p.Order >= newOrder && p.Order < oldOrder))
                item.Order += 1;
        else
            foreach (var item in ProjectFiles
                         .Where(p => p.Order <= newOrder && p.Order >= oldOrder))
                item.Order -= 1;

        selectedItem.Order = newOrder;

        await ProjectManager.UpdateProjectFileOrders(new UpdateProjectFileOrderCommand
        {
            FilesOrders = ProjectFiles.ToDictionary(p => p.Id, o => o.Order)
        });
    }
}
using AutoMapper;
using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalFileDetail;
using HiSubmit.Client.Infrastructure.Managers.FestivalFiles;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Managers.Festivals;

namespace HiSubmit.Web.Components.Pages.Festival.FestivalEditComponent;

public partial class FestivalFile
{
    [Parameter]
    public int FestivalId { get; set; }
    [Parameter]
    public EventCallback NextPanel { get; set; }
    [Parameter]
    public EventCallback PrevPanel { get; set; }
    
    [Parameter]
    public bool IsAdmin { get; set; }

    private bool _loaded;

    [Inject]
    private IMapper Mapper { get; set; }

    [Inject]
    private IFestivalFileManager FestivalFileManager { get; set; }
    [Inject]
    private ILocalStorageService LocalStorageService { get; set; }

    [Inject]
    private  IFestivalManager FestivalManager { get; set; }

    private List<GetAllFestivalFileResponse> Files { get; set; }
        
 
    private  bool ReadOnlyMood { get; set; }
        
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        // await GetFestivalId();
        await GetFestivalDetail();
        await GetFiles();
        _loaded = true;
    }

    private async Task GetFestivalDetail()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            ReadOnlyMood =(response.Data.FestivalStatus == FestivalStatus.UnderInvestigation || IsAdmin);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
        
    private async Task GetFiles()
    {
        var result = await FestivalFileManager.GetAllAsync(new GetAllFestivalFileQuery
        {
            FestivalId = FestivalId
        },FestivalId);
        if (result.Succeeded)
        {
            Files = result.Data;
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
    // private async Task GetFestivalId()
    // {
    //     _festivalId = await LocalStorageService.GetItemAsync<int>(StorageConstants.Local.FestivalId);
    //
    // }

    private async Task InvokeModal(int id=0)
    {
        var parameter = new DialogParameters();
        AddEditFestivalFileCommand file = null;
        string title = "";
        if (id == 0)
        {
            file = new AddEditFestivalFileCommand() { FestivalId=FestivalId};
            title = Localize["Add File"];
        }
        else
        {
            var response =await FestivalFileManager.GetDetailAsync(new GetFestivalFileDetailQuery() { Id=id},FestivalId);
            if (response.Succeeded)
            {
                title = Localize["Update File"];
                file =Mapper.Map<AddEditFestivalFileCommand>(response.Data);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message,Severity.Error);
                }
                return;
            }
        }
        parameter.Add(nameof(AddEditFestivalFileModal.File), file);
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            
        };
        var dialog = _dialogService.Show<AddEditFestivalFileModal>(title, parameter, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task DeleteAsync(int id, string name)
    {
        var deleteContent = name;
        var parameters = new DialogParameters
        {
            {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await FestivalFileManager.DeleteAsync(new DeleteFestivalFileCommand { Id = id },FestivalId);
            if (response.Succeeded)
            {
                await Reset();
                //   await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
    private async Task Reset()
    {
        Files = new List<GetAllFestivalFileResponse>();
        await GetFiles();
    }
        
    private async Task GoNext()
    {
        await NextPanel.InvokeAsync();
    }
    
    private async Task GoPrev()
    {
        await PrevPanel.InvokeAsync();
    }

        
        
}
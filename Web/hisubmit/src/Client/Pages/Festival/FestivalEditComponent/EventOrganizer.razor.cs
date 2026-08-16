using Blazored.LocalStorage;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEdiitEventOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteEventOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiSubmit.Client.Pages.Festival.FestivalEditComponent;

public partial class EventOrganizer
{

    [Inject]
    private IFestivalManager FestivalManager { get; set; }
    [Inject]
    private ILocalStorageService LocalStorageService { get; set; }
    [CascadingParameter]
    public HubConnection HubConnection { get; set; }
    [Parameter]
    public int FestivalId { get; set; }
    [Parameter] public bool ReadOnlyMood { get; set; }
    [Parameter]
    public bool IsAdmin { get; set; }
    private List<GetAllEventOrganizerResponse> _organizers { get; set; }

    private bool _loaded = true;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        //  await GetFestivalId();
        await GetOrginizer();
    }

    private async Task GetOrginizer()
    {
        var result = await FestivalManager.GetAllOrganizerAsync(new GetAllOrganizerQuery
        {
            FestivalId = FestivalId
        });
        if (result.Succeeded)
        {

            _organizers = result.Data;
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }
    //private async Task GetFestivalId()
    //{
    //    FestivalId = await LocalStorageService.GetItemAsync<int>(StorageConstants.Local.FestivalId);

    //}

    private async Task InvokeModal(GetAllEventOrganizerResponse item = null)
    {
        var model = new AddEditEventOrginizerCommand() { FestivalId=FestivalId};
        if(item != null)
        {
            model.Id=item.Id;
            model.Title=item.Title;
            model.Name=item.Name;
            model.ImageName=item.ImageName;
            model.Image=new Hisubmit.Client.SharedModels.Requests.UploadRequest();                            
        }
        var parameter = new DialogParameters
        { { nameof(AddEventOrganizerModal.Organizer),model} };


        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
        };
        var dialog = _dialogService.Show<AddEventOrganizerModal>(Localize["Add Organizer"], parameter, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }


    private async Task DeleteAsync(int id, string name)
    {
        string deleteContent = name;
        var parameters = new DialogParameters
        {
            {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await FestivalManager.DeleteOrginizer(new DeleteEventOrginizerCommand { Id=id, FestivalId=FestivalId });
            if (response.Succeeded)
            {
                await Reset();
                _snackBar.Add(response.Messages[0], MudBlazor.Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, MudBlazor.Severity.Error);
                }
            }
        }
    }
    private async Task Reset()
    {
        _organizers=new List<GetAllEventOrganizerResponse>();
        await GetOrginizer();
    }

}
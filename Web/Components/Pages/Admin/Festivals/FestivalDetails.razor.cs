using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.ReleaseFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.SpecialRequest;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Managers.AdminFestival;
using Web.Components.Pages.Festival.FestivalEditComponent;
using ClientComponents.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Web.Components.Shared.Dialogs;

namespace Web.Components.Pages.Admin.Festivals;

public partial class FestivalDetails
{
       #region Parameters

    [Parameter] public int FestivalId { get; set; }

    #endregion

    #region Inject

    [Inject] private IAdminFestivalManager FestivalManager { get; set; }

    #endregion

    #region Private Filled

    private bool _loaded;
    private bool _checkForNext;
    private bool _releaseProcess;
    private int _activePanelIndex;

    #endregion

    #region ChildComponentRef
    private MudTabs MainTab { get; set; }

    private FestivalFile _festivalFile;
    private FestivalImages _festivalImages;
    private FestivalDetail _festivalDetails;
    private ContactAndVenue _contactAndVenue;
    private FestivalDeadline _festivalDeadline;
    private FestivalEventCategory _festivalEventCategory;
    private FestivalAdditionalSetting _festivalAdditionalSetting;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    #endregion


    private GetFestivalDetailResponse _detail = new();

    private async Task ChangeTab(int index)
    {
        await Task.Run(() => { MainTab.ActivatePanel(index); });
    }

    private async Task LoadData()
    {
        await LoadFestival();
    }

    private async Task LoadFestival()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            _detail = response.Data;
        }

        foreach (var message in response.Messages)
        {
            _snackBar.Add(message, Severity.Error);
        }
    }
    
    #region ChangeTabFunction

    private async Task ChangeTabChecked(int selectedIndex)
    {
        _checkForNext = !_checkForNext;
        if (selectedIndex != _activePanelIndex)
        {
            switch (_activePanelIndex)
            {
                case 0: //festival detail
                    if (_festivalDetails.ModifiedForm())
                        await ShowNextModal(selectedIndex);
                    else
                        _activePanelIndex = selectedIndex;
                    break;
                case 1: //contact and venue
                    if (_contactAndVenue.ModifiedForm())
                        await ShowNextModal(selectedIndex);
                    else
                        _activePanelIndex = selectedIndex;
                    break;

                case 2: //festival deadline
                    if (_festivalDeadline.ModifiedForm())
                        await ShowNextModal(selectedIndex);
                    else
                        _activePanelIndex = selectedIndex;
                    break;

                case 3: //festival event category
                    if (_festivalEventCategory.ModifiedForm())
                        await ShowNextModal(selectedIndex, false);
                    else
                        _activePanelIndex = selectedIndex;
                    break;

                case 4: //festival file
                    _activePanelIndex = selectedIndex;
                    break;

                case 5: //festival images
                    if (_festivalImages.ModifiedForm())
                        await ShowNextModal(selectedIndex);
                    else
                        _activePanelIndex = selectedIndex;
                    break;

                case 6: //additional setting
                    if (_festivalAdditionalSetting.ModifiedForm())
                        await ShowNextModal(selectedIndex);
                    else
                        _activePanelIndex = selectedIndex;
                    break;
            }
        }
    }

    private async Task ShowNextModal(int selectedTab, bool saveButton = true)
    {
        var option = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            
        };
        var parameters = new DialogParameters
        {
            { nameof(SaveAndNext.SaveButton), saveButton }
        };
        var dialog = _dialogService.Show<SaveAndNext>("Save or Next", parameters, option);
        var result = await dialog.Result;

        if (result.Canceled || result.Data is null)
            return;

        switch (result.Data.ToString())
        {
            case "SaveAndNext":
                switch (_activePanelIndex)
                {
                    case 0: //festival detail
                        if (await _festivalDetails.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;
                    case 1: //contact and venue
                        if (await _contactAndVenue.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;

                    case 2: //festival deadline
                        if (await _festivalDeadline.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;

                    case 3: //festival event category
                        //save button not active this panel
                        break;

                    case 4: //festival file
                        //No form exists without making changes to this form
                        break;

                    case 5: //festival images
                        if (await _festivalImages.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;

                    case 6: //additional setting
                        if (await _festivalAdditionalSetting.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;
                }

                break;
            case "Next":
                _activePanelIndex = selectedTab;
                break;
        }
    }

    #endregion
}

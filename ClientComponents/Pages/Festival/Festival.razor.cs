using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.ReleaseFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.SpecialRequest;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using ClientComponents.Pages.Festival.FestivalEditComponent;
using ClientComponents.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace ClientComponents.Pages.Festival;

public partial class Festival
{
    #region Inject

    [Inject] private IFestivalManager FestivalManager { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int? FestivalIdParam { get; set; }

    #endregion

    #region Private Filled

    private int _activePanelIndex;
    private bool _checkForNext;

    private bool _releaseProcess;

    private int FestivalId { get; set; }
    private bool _loaded;

    #endregion

    #region ChildComponentRef

    private MudTabs MainTab { get; set; }

    private FestivalFile _festivalFile;
    private FestivalImages _festivalImages;
    private FestivalDetail _festivalDetail;
    private ContactAndVenue _contactAndVenue;
    private FestivalDeadline _festivalDeadline;
    private FestivalEventCategory _festivalEventCategory;
    private FestivalAdditionalSetting _festivalAdditionalSetting;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await CheckPermission(Permissions.Festival.View);
        await LoadData();
        _loaded = true;
        await base.OnInitializedAsync();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await  LoadSelectedFestivalId();
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            FestivalId = SelectedFestivalId,
            AccountType = SiteAccountType.Festival,
            NotificationTypes = NotificationType.FestivalAnsweredReleasedRequest
        });
        NotificationService.ChangeNotificationBar();
    }

    #endregion


    private GetFestivalDetailResponse FestivalDetail { get; set; } = new();

    private async Task ChangeTab(int index)
    {
        await Task.Run(() => { MainTab.ActivatePanel(index); });
    }

    private async Task LoadData()
    {
         LoadFestivalId();
        await LoadFestival();
    }

    private void LoadFestivalId()
    {
        FestivalId = FestivalIdParam ?? SelectedFestivalId;
    }

    private async Task LoadFestival()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            FestivalDetail = response.Data;
        }

        foreach (var message in response.Messages)
        {
            _snackBar.Add(message, Severity.Error);
        }
    }

    private async Task SpecialRequest()
    {
        var response = await FestivalManager.SpecialRequest(new SpecialRequestCommand()
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            await PendingApproval();
            //_snackBar.Add(response.Messages[0], Severity.Success);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }


    private async Task PendingApproval()
    {
        var options = new DialogOptions()
        {
            BackdropClick = true,
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        };
        var d = await _dialogService.ShowAsync<ApprovedEmail>("Admin Approval", options);
        var res = await d.Result;
    }
    private async Task ReleaseFestival()
    {
        _releaseProcess = true;
        var response = await FestivalManager.Release(new ReleaseFestivalCommand()
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            var parameters = new DialogParameters
                { { nameof(SuccessfullyProccess.ContentText), response.Messages[0] } };

            var options = new DialogOptions() { CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Small };

            _dialogService.Show<SuccessfullyProccess>("Success", parameters, options);
        }
        else
        {
            var parameter = new DialogParameters
                { { nameof(EmptyItemForReleaseFestival.Messages), response.Messages } };
            var options = new DialogOptions() { CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Small };
            var dialog = _dialogService.Show<EmptyItemForReleaseFestival>("Empty Item", parameter, options);
        }

        _releaseProcess = false;
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
                    if (_festivalDetail.ModifiedForm())
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

        switch (result.Data.ToString())
        {
            case "SaveAndNext":
                switch (_activePanelIndex)
                {
                    case 0: //festival detail
                        if (await _festivalDetail.SaveAsync())
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

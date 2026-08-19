using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.ReleaseFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.SpecialRequest;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Web.Components.Pages.Festival.FestivalEditComponent;
using ClientComponents.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Web.Components.Shared.Dialogs;
using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Web.Components.Pages.Festival;

public partial class Festival : IDisposable
{
    #region Inject

    [Inject] private IFestivalManager FestivalManager { get; set; }
    [Inject] private ILogger<Festival> Logger { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int? FestivalIdParam { get; set; }

    #endregion

    #region Private Filled

    private int _activePanelIndex;
    private bool _checkForNext;

    private bool _releaseProcess;
    private Timer _draftTimer;
    private int _draftSaveInProgress;

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
        _draftTimer = new Timer(
            _ => _ = InvokeAsync(AutoSaveDraftAsync),
            null,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(1));
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

        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion


    private GetFestivalDetailResponse FestivalDetail { get; set; } = new();

    private async Task ChangeTab(int index)
    {
        if (index == _activePanelIndex)
            return;

        if (!await SaveCurrentPanelAsync())
            return;

        _activePanelIndex = index;
        await InvokeAsync(StateHasChanged);
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
        var response = await FestivalManager
            .GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
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
        if (selectedIndex != _activePanelIndex)
            await ChangeTab(selectedIndex);
    }

    private async Task<bool> SaveCurrentPanelAsync()
    {
        switch (_activePanelIndex)
        {
            case 0:
                return !_festivalDetail.ModifiedForm() || await _festivalDetail.SaveAsync();
            case 1:
                return !_contactAndVenue.ModifiedForm() || await _contactAndVenue.SaveAsync();
            case 2:
                return !_festivalEventCategory.ModifiedForm() || await _festivalEventCategory.SaveAsync();
            case 3:
                return !_festivalDeadline.ModifiedForm() || await _festivalDeadline.SaveAsync();
            case 4:
                return true;
            case 5:
                return !_festivalImages.ModifiedForm() || await _festivalImages.SaveAsync();
            case 6:
                return !_festivalAdditionalSetting.ModifiedForm() ||
                       await _festivalAdditionalSetting.SaveAsync();
            default:
                return true;
        }
    }

    private async Task AutoSaveDraftAsync()
    {
        if (!_loaded || Interlocked.Exchange(ref _draftSaveInProgress, 1) == 1)
            return;

        try
        {
            if (await SaveCurrentPanelAsync())
            {
                _snackBar.Add(
                    Localize["Draft saved automatically"],
                    Severity.Info);
            }
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, "Automatic festival draft save failed for festival {FestivalId}", FestivalId);
        }
        finally
        {
            Interlocked.Exchange(ref _draftSaveInProgress, 0);
        }
    }

    private string GetStepHint() => _activePanelIndex switch
    {
        0 => Localize["Start with the festival identity, description, awards and rules."],
        1 => Localize["Add contact details, venues and event locations."],
        2 => Localize["Define the submission categories and questions before setting their deadlines and fees."],
        3 => Localize["Set the opening, notification and event dates, then add the fee schedule for each category."],
        4 => Localize["Upload the festival files required for review or publication."],
        5 => Localize["Add the cover, gallery images and public presentation assets."],
        6 => Localize["Review automation and additional settings before release."],
        _ => Localize["Review your festival."]
    };

    public void Dispose()
    {
        _draftTimer?.Dispose();
    }

    #endregion
}

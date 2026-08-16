using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace ClientComponents.Pages.Public.Festivals.Components;

public partial class FestivalReviews
{
    #region Injection

    [Inject] public IPublicFestivalManager FestivalManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int FestivalId { get; set; }

    #endregion

    #region Private Filled

    private PaginatedResult<GetAllReviewResponse> ReviewResponse;
    private List<GetAllReviewResponse> _reviews;
    private bool _loaded;
    private int _pageNumber = 1;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        _subscription = ApplicationState.RegisterOnPersisting(PersistFestival);
        if (ApplicationState.TryTakeFromJson<PaginatedResult<GetAllReviewResponse>>
                ("reviewResponse", out var stored))
        {
            ReviewResponse = stored;
            _reviews = ReviewResponse.Data;
        }
        else
        {
            await LoadReview(new GetAllReviewQuery()
            {
                FestivalId = FestivalId,
                PageNumber = _pageNumber,
                PageSize = 4
            });
        }

        await base.OnInitializedAsync();
        _loaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await _jsRuntime.InvokeVoidAsync("CreateOrganizerSlider");
            await _jsRuntime.InvokeVoidAsync("CreateImageSlider");
        }
    }

    #endregion

    private async Task LoadReview(GetAllReviewQuery query)
    {
        var response = await FestivalManager.GetAllReviews(query);
        if (response.Succeeded)
        {
            _reviews = response.Data;
            ReviewResponse = response;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task ChangePage(int pageNumber)
    {
        _pageNumber = pageNumber;
        var query = new GetAllReviewQuery
        {
            FestivalId = FestivalId,
            PageNumber = pageNumber,
            PageSize = 4
        };
        await LoadReview(query);
    }

    #region Prerendering

    private PersistingComponentStateSubscription _subscription;

    private Task PersistFestival()
    {
        ApplicationState.PersistAsJson("reviewResponse", ReviewResponse);
        return Task.CompletedTask;
    }

    #endregion
}
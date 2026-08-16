using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Client.Infrastructure.Managers.Contents;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Client.Infrastructure.Managers.AdminFestival;
using HiSubmit.Client.SharedModels.Wrapper;
using Hisubmit.Hisubmit.Client.SharedModels.Features;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Likes;
using Microsoft.JSInterop;

namespace Web.Components.Pages.Public.Content
{
    public partial class New
    {
        [Inject] private IContentManager ContentManager { get; set; }

        [Inject] private IAdminFestivalManager FestivalManager { get; set; }

        [Parameter] public int NewId { get; set; }
        [Parameter] public string Title { get; set; }

        private GetDetailNewResponse _new = new();
        private PaginatedResult<GetAllNewResponse> _newsResponse = new(new List<GetAllNewResponse>());
        private PaginatedResult<GetAllFestivalResponse> _festivalResponse = new(new List<GetAllFestivalResponse>());

        private bool _liked;
        private int _likedCount;

        protected override async Task OnInitializedAsync()
        {
            _subscription = ApplicationState.RegisterOnPersisting(PersistFestival);
            await LoadNew();
            await LoadNews();
            await LoadFestivals();
            await LoadLikes();
            await base.OnInitializedAsync();
        }

        #region Prerendering

        private PersistingComponentStateSubscription _subscription;

        private Task PersistFestival()
        {
            ApplicationState.PersistAsJson("festivals", _festivalResponse);
            ApplicationState.PersistAsJson("new", _new);
            ApplicationState.PersistAsJson("news", _newsResponse);
            return Task.CompletedTask;
        }

        #endregion
        // protected override async Task OnAfterRenderAsync(bool firstRender)
        // {
        //     if (firstRender)
        //     {
        //         await LoadNews();
        //         await LoadFestivals();
        //         StateHasChanged();
        //     }
        //
        //     await base.OnAfterRenderAsync(firstRender);
        // }
        //
        // protected override async Task OnParametersSetAsync()
        // {
        //     await LoadNew();
        //     //StateHasChanged();
        // }

        private async Task LoadNew()
        {
            if (ApplicationState.TryTakeFromJson
                    <GetDetailNewResponse>
                    ("new", out var stored))
            {
                _new = stored;
            }
            else
            {
                var response = await ContentManager.GetNewDetail(new GetDetailNewQuery
                {
                    Id = NewId
                });
                if (response.Succeeded)
                    _new = response.Data;
                else
                    foreach (var message in response.Messages)
                        _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }

        private async Task LoadNews()
        {
            if (ApplicationState.TryTakeFromJson
                    <PaginatedResult<GetAllNewResponse>>
                    ("news", out var stored))
            {
                _newsResponse = stored;
            }
            else
            {
                var response = await ContentManager.GetAllNew(new GetAllNewRequest()
                {
                    PageSize = 5
                });
                if (response.Succeeded)
                {
                    _newsResponse = response;
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, MudBlazor.Severity.Error);
                    }
                }
            }
        }

        private async Task CopyLinkToClipboard()
        {
            var text = _navigationManager.Uri;
            await _jsRuntime.InvokeVoidAsync("clipboardCopy", text);
        }

        private async Task LoadFestivals()
        {
            if (ApplicationState.TryTakeFromJson
                    <PaginatedResult<GetAllFestivalResponse>>
                    ("festivals", out var stored))
            {
                _festivalResponse = stored;
            }
            else
            {
                var response = await FestivalManager.GetAllAsync(new GetAllFestivalRequest()
                {
                    PageSize = 5
                });
                if (response.Succeeded)
                {
                    _festivalResponse = response;
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, MudBlazor.Severity.Error);
                    }
                }
            }
        }

        private async Task UpdateLike()
        {
            if ((await AuthenticationManager.CurrentUser()).Identity!.IsAuthenticated)
            {
                var res = await ContentManager.AddDeleteLike
                    (new GetLikeCountRequest() { NewId = NewId });
                if (res.Succeeded)
                {
                    _liked = !_liked;
                    _likedCount=_liked ? _likedCount+1 : _likedCount-1;
                }
            }
        }
        private async Task LoadLikes()
        {
            var response = await ContentManager
                .GetLikeCount(new GetLikeCountRequest() { NewId = NewId });

            if (response.Succeeded) _likedCount=response.Data;

            var identity = (await AuthenticationManager.CurrentUser()).Identity;
            if (identity is { IsAuthenticated: true })
            {
                var response2 = await ContentManager
                    .GetLikeState(new GetLikeCountRequest() { NewId = NewId });
                if (response2.Succeeded) _liked=response2.Data;
            }
        }


    }
}
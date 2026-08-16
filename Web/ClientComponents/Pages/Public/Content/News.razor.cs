using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Client.Infrastructure.Managers.Contents;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;

namespace ClientComponents.Pages.Public.Content;

public partial class News
{
    #region Inject

    [Inject] private IContentManager ContentManager { get; set; }

    #endregion

    #region Private Feild

    private List<GetAllNewResponse> _news = new();
    private PaginatedResult<GetAllNewResponse> _newResponse;
    private GetAllNewRequest _request;
    private int PageNumber = 1;
    private bool _loaded;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        _subscription = ApplicationState.RegisterOnPersisting(PersistNews);
        await LoadNews(new GetAllNewRequest
        {
            PageNumber = 1,
            PageSize = 12
        });
        _loaded = true;
        await base.OnInitializedAsync();
    }

    #endregion

    #region Prerendering

    private PersistingComponentStateSubscription _subscription;

    private Task PersistNews()
    {
        ApplicationState.PersistAsJson("news", _newResponse);
        return Task.CompletedTask;
    }

    #endregion


    public string SearchString { get; set; }
    private async Task LoadNews(GetAllNewRequest request)
    {
        request.ReturnLastNews = true;
        if (ApplicationState.TryTakeFromJson
                <PaginatedResult<GetAllNewResponse>>
                ("news", out var stored))
        {
            _newResponse = stored;
            if (_newResponse != null)
                _news = _newResponse.Data;
        }
        else
        {
            var response = await ContentManager.GetAllNew(request);
            if (response.Succeeded)
            {
                _news = response.Data;
                _newResponse = response;
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

    private async Task ChangePage(int pageNumber)
    {
        PageNumber = pageNumber;
        _request.PageNumber = pageNumber;
        await LoadNews(_request);
    }

    private async Task Search()
    {
        _request = new GetAllNewRequest
        {
            PageNumber = 1,
            SearchString = SearchString,
            ReturnLastNews = true
        };
        await LoadNews(_request);
    }
}
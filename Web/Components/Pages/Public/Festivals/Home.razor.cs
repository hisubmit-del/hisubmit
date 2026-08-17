using MudBlazor;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Timers;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using HiSubmit.Client.Infrastructure.Managers.FestivalQualifires;
using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;
using Web.Components.Pages.Public.Festivals.Components;
using Microsoft.JSInterop;
using Web.Components.Shared.Dialogs;
using Timer = System.Timers.Timer;

namespace Web.Components.Pages.Public.Festivals;



public partial class Home : IDisposable
{
    #region Inject
    [Inject] private IJSRuntime Js { get; set; }
    [Inject] private IPublicFestivalManager FestivalManager { get; set; }
    [Inject] private IFestivalQualifiersManager FestivalQualifiersManager { get; set; }
    #endregion

    #region Parameters

    [Parameter] public int PageNumber { get; set; } = 1;
    

    #endregion

    #region Private Field

    private IEnumerable<GetAllFestivalResponse> _pagedData = new List<GetAllFestivalResponse>();
    private PaginatedResult<GetAllFestivalResponse> _festivalResponse;
    private GetAllFestivalRequest _advancedSearch = new();
    private bool _loaded;
    private bool _prerender=true;
    private DateRange _dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);

    private Timer _timer;

    private int ActiveFilterCount =>
        (_advancedSearch.SearchString is { Length: > 0 } ? 1 : 0)
        + (_advancedSearch.FestivalType is not null ? 1 : 0)
        + (_advancedSearch.EventDateTo is not null ? 1 : 0)
        + (_advancedSearch.EntryDeadlineTo is not null ? 1 : 0)
        + (_advancedSearch.Focus is not null ? 1 : 0)
        + (_advancedSearch.Category is not null ? 1 : 0)
        + (_advancedSearch.OpenOnly ? 1 : 0)
        + (_advancedSearch.TicketOnly ? 1 : 0)
        + (_advancedSearch.FeeMinVal is not null ? 1 : 0)
        + (_advancedSearch.FeeMaxVal is not null ? 1 : 0)
        + (_advancedSearch.YearsRunningMinVal is not null ? 1 : 0)
        + (_advancedSearch.YearsRunningMaxVal is not null ? 1 : 0)
        + (!string.IsNullOrWhiteSpace(_advancedSearch.Orderby.FirstOrDefault()) ? 1 : 0);
    #endregion

    #region Prerendering
    
    private PersistingComponentStateSubscription _subscription;

    private Task PersistFestival()
    {
        ApplicationState.PersistAsJson("focuses", _focuses);
        ApplicationState.PersistAsJson("artCategories", _artCategories);
        ApplicationState.PersistAsJson("prerender", _prerender);
        ApplicationState.PersistAsJson("Qualifiers",Qualifiers);
        return Task.CompletedTask;
    }

    #endregion

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // module = await JS.InvokeAsync<IJSObjectReference>(
            //     "import", "./Components/Pages/DOMCleanup.razor.js");
           

            await Js.InvokeVoidAsync("CreateSlider");
        }
    }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        _timer = new Timer(500); // 2 ثانیه
        _timer.Elapsed += OnSearchElapsed;
        _timer.AutoReset = false;
        
        _subscription = ApplicationState.RegisterOnPersisting(PersistFestival);
       
    }
    
    

    protected override async Task OnParametersSetAsync()
    {
        await LoadFestivals(new GetAllFestivalRequest { PageNumber = PageNumber });
        await LoadQualifiers();
        await LoadFocuses();
        await LoadArtCategories();
        _loaded = true;

    }


   
   
   
    private async Task AdvancedSearch()
    {
        // IsAdvancedSearch = true;
        // await _table.ReloadServerData();
        // IsAdvancedSearch = false;
        _advancedSearch.PageNumber = 1;
        await LoadFestivals(_advancedSearch);
    }

    private async Task ResetFilters()
    {
        _advancedSearch = new GetAllFestivalRequest { PageNumber = 1 };
        PageNumber = 1;
        await LoadFestivals(_advancedSearch);
    }

    private async Task ChangePage(int pageNumber)
    {
        PageNumber = pageNumber;
        _advancedSearch.PageNumber = pageNumber;
        await LoadFestivals(_advancedSearch);
    }

    private async Task LoadFestivals(GetAllFestivalRequest request)
    {
        var response = await FestivalManager.GetAllFestival(request);
        if (response.Succeeded)
        {
            _festivalResponse = response;
            _pagedData = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
        }
    }

    #region ArtCategories

    private List<GetAllArtCategoryResponse> _artCategories=new();
   
    private async Task LoadArtCategories()
    {
        if (ApplicationState.TryTakeFromJson
                <List<GetAllArtCategoryResponse>>
                ("artCategories", out var stored))
        {
            _artCategories = stored;
        }
        else
        {
            var response = await FestivalManager.GetAllArtCategories(new GetAllArtCategoryRequest());
            if (response.Succeeded)
                _artCategories = response.Data;
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
    }

    #endregion

    #region  Focuses

    private List<GetAllFestivalFocusResponse> _focuses=new();
   
    private async Task LoadFocuses()
    {
        if (ApplicationState.TryTakeFromJson
                <List<GetAllFestivalFocusResponse>>
                ("focuses", out var stored))
        {
            _focuses = stored;
        }
        else
        {
            var response = await FestivalManager.GetAllFestivalFocuses(new GetAllFestivalFocusQuery());
            if (response.Succeeded)
                _focuses = response.Data;
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
    }

    #endregion

    #region  festival Box

    private async Task Submit(int festivalId)
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            
        };
        var user = await AuthenticationManager.CurrentUser();
        if (!user.Identity.IsAuthenticated)
        {
            var option = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                
            };
            var parameters = new DialogParameters();
            _dialogService.Show<NeedToLogin>("Need To Login", parameters, option);
        }
        else
        {
            var parameter = new DialogParameters
            {
                { nameof(FestivalCategorySelected.FestivalId), festivalId }
            };
            _dialogService.Show<FestivalCategorySelected>(Localize["Selected category"], parameter, options);
        }
    }

    private void GoToFestivalDetail(string url)
    {
        _navigationManager.NavigateTo($"/festivalPage/{url}");
    }

    #endregion

    #region Qualifires

    private List<GetAllFestivalQualifiersResponse> Qualifiers = new();

    private string ClassFestivalHeader => _prerender ? "align-self-center  flex-1 animate__animated  animate__backInDown" : "align-self-center  flex-1";

    private async Task LoadQualifiers()
    {
        if (ApplicationState.TryTakeFromJson
                <List<GetAllFestivalQualifiersResponse>>
                ("Qualifiers", out var stored))
        {
            Qualifiers = stored;
            return;
        }
        var response =
            await FestivalQualifiersManager.GetAllAsync(new GetAllFestivalQualifiersQuery());
        if (response.Succeeded)
        {
            Qualifiers = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    #endregion

    private async Task ReloadFestivals()
    {
        if (_timer is null)
            return;
        _timer.Stop();
        _timer.Start();
    }

    private void OnSearchElapsed(object sender, ElapsedEventArgs e)
    {       
        InvokeAsync(async () =>
        {
            _advancedSearch.PageNumber = 1;
            PageNumber = 1;
            await LoadFestivals(_advancedSearch);
            StateHasChanged();
        });
        _timer.Stop();
    }

    
    private async Task ChangeYearMinVal(int? i)
    {
        _advancedSearch.YearsRunningMinVal = i;
        await ReloadFestivals();
    }
    private async Task ChangeYearMaxVal(int? i)
    {
        _advancedSearch.YearsRunningMaxVal = i;
        await ReloadFestivals();
    }
    private async Task ChangeFeeMinVal(int? i)
    {
        _advancedSearch.FeeMinVal = i;
        await ReloadFestivals();
    }
    private async Task ChangeFeeMaxVal(int? i)
    {
        _advancedSearch.FeeMaxVal = i;
        await ReloadFestivals();
    }
    //private static RenderFragment<GetAllFestivalResponse> F
    //    = item=>@<p>item.</p> ;

    //public static RenderFragment RenderFregmentInfo="<p>AMir Mohammadi</p>";

    public void Dispose()
    {
        _timer?.Dispose();
        _subscription.Dispose();
    }
}

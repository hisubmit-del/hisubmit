using System;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.Comment;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetAllTicket;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using HiSubmit.Client.Infrastructure.Managers.Tickets;
using Web.Components.Pages.Public.Festivals.Components;
using Web.Components.Shared.Components;
using ClientComponents.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Comments.Commands;
using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using Hisubmit.Hisubmit.Client.SharedModels.Features;
using Microsoft.JSInterop;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalFile;
using Web.Components.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;

namespace Web.Components.Pages.Public.Festivals;

public partial class FestivalDetail
{
    #region Inject

    [Inject] public IPublicFestivalManager FestivalManager { get; set; }
    [Inject] public ITicketManager TicketManager { get; set; }
    [Inject] private ISubmitManager SubmitManager { get; set; }
    [Inject]private ICommentManager CommentManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public string FestivalUrl { get; set; }

    #endregion

    #region Private Filled

    private bool _loaded;
    private int _currentUserSubmitId;
    private int FestivalId { get; set; }
    private bool _canReview;
    private string _reviewEligibilityMessage;
    private List<GetAllSubmitsResponse> _currentUserSubmits = [];

    private List<GetAllEventCategoryResponse> EventCategories { get; set; }
    private GetFestivalDetailResponse Festival { get; set; }

    private List<GetAllTicketResponse> _tickets = [];
    private List<GetAllNewResponse> _festivalNews = [];
    private List<GetAllPagedProductsResponse> _products = [];
    private List<GetAllFestivalImageResponse> _imageCovers = [];
    private List<GetAllFestivalImageResponse> _images = [];
    private List<GetAllFestivalImageResponse> _covers = [];
    private List<GetAllFestivalFileResponse> _files=[];

    
    //like
    private bool _liked;
    private int _likedCount;
    private double _festivalAverageRating;
    private int _festivalRatingCount;
    private int _selectedFestivalRating;
    private bool _hasRatedFestival;
    private int _ratingRefreshToken;
    
    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        _subscription = ApplicationState.RegisterOnPersisting(PersistFestival);

        await LoadFestivalDetail();
        if (Festival is { Public: true })
        {
            FestivalId = Festival.Id;
            await LoadFestivalImages();
            await LoadOrganizer();
            await LoadNews();
        }

        _loaded = true;
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Festival is { Public: true })
        {
            await UserIsAuthenticate();
            await LoadTickets();
            await LoadUserSubmit();
            await LoadProducts();
            await LoadQualifiers();
            await LoadLikes();
            await LoadRatingSummary();
        }

        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await _jsRuntime.InvokeVoidAsync("CreateImageSlider");
            await _jsRuntime.InvokeVoidAsync("CreateOrganizerSlider");
        }
    }

    #endregion


private async Task LoadFiles(){
    var response = await FestivalManager.GetAllFestivalFiles(new GetAllFestivalFileQuery()
    {
        FestivalId = FestivalId
    });
    if(response.Succeeded)_files=response.Data;
}


    private async Task LoadFestivalDetail()
    {
        if (ApplicationState.TryTakeFromJson<GetFestivalDetailResponse>
                (StateKey("festival"), out var stored) &&
            stored is not null &&
            string.Equals(stored.URL, FestivalUrl, StringComparison.OrdinalIgnoreCase))
        {
            Festival = stored;
        }
        else
        {
            var response = await FestivalManager.GetFestivalDetailAsync(
                new GetFestivalDetailByIdQuery
                {
                    FestivalUrl = FestivalUrl,
                    WithInclude = true
                });
            if (response.Succeeded)
            {
                Festival = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    private async Task LoadTickets()
    {
        var response =
            await TicketManager.GetAllAsync(new GetAllTicketQuery()
            {
                FestivalId = FestivalId,
                GetAllData = true,
                GetActiveTicket = true
            });
        if (response.Succeeded)
        {
            _tickets = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        // throw new Exception("exception message");
        StateHasChanged();
    }


    private async Task LoadLikes()
    {
        var response = await FestivalManager
            .GetLikeCount(new BaseFestivalRequest() { FestivalId = FestivalId });
        
        if(response.Succeeded)_likedCount=response.Data;

        var identity = (await AuthenticationManager.CurrentUser()).Identity;
        if (identity is { IsAuthenticated: true })
        {
            var response2 = await FestivalManager.GetLikeState(new BaseFestivalRequest()
            {
                FestivalId = FestivalId,
            });
            if(response2.Succeeded)_liked=response2.Data;
        }
    }
    private async Task LoadNews()
    {
        if (ApplicationState.TryTakeFromJson<List<GetAllNewResponse>>
                (StateKey("festivalNews"), out var stored))
        {
            _festivalNews = stored;
        }
        else
        {
            var response = await FestivalManager.GetAllNewsAsync(new GetAllNewRequest
            {
                IsEnable = true,
                GetAllData = true,
                GetFestivalNews = true,
                FestivalId = FestivalId
            });
            if (response.Succeeded)
            {
                _festivalNews = response.Data;
            }
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
    }

    private async Task LoadProducts()
    {
        if (ApplicationState.TryTakeFromJson<List<GetAllPagedProductsResponse>>
                (StateKey("products"), out var stored))
        {
            _products = stored;
        }
        else
        {
            var response = await FestivalManager.GetAllProducts(new GetAllProductsRequest
            {
                GetAllData = true,
                FestivalId = FestivalId,
                IsEnable = true
            });

            if (response.Succeeded)
                _products = response.Data;
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
            StateHasChanged();
        }
    }

    private async Task LoadFestivalImages()
    {
        if (ApplicationState.TryTakeFromJson<List<GetAllFestivalImageResponse>>
                (StateKey("imageCovers"), out var stored))
        {
            _imageCovers = stored;
            _images = _imageCovers.Where(p => p.ImageType == ImageType.Images).ToList();
            _covers = _imageCovers.Where(p => p.ImageType == ImageType.Cover).ToList();
        }
        else
        {
            var response = await FestivalManager.GetAllImages(new GetAllFestivalImageQuery()
            {
                FestivalId = Festival.Id,
                GetAllData = true
            });
            if (response.Succeeded)
            {
                _imageCovers = response.Data;
                _images = response.Data.Where(p => p.ImageType == ImageType.Images).ToList();
                _covers = response.Data.Where(p => p.ImageType == ImageType.Cover).ToList();
            }
            else
            {
                foreach (var item in response.Messages)
                {
                    _snackBar.Add(item, Severity.Error);
                }
            }
        }
    }

    private bool UserIsAuthenticated { get; set; }

    private async Task UserIsAuthenticate()
    {
        var user = await AuthenticationManager.CurrentUser();
        UserIsAuthenticated = user.Identity is { IsAuthenticated: true };
    }

    private async Task LoadRatingSummary()
    {
        var response = await FestivalManager.GetFestivalRatingSummary(
            new GetFestivalRatingSummaryQuery { FestivalId = FestivalId });
        if (response.Succeeded && response.Data is not null)
        {
            _festivalAverageRating = response.Data.AverageRate;
            _festivalRatingCount = response.Data.TotalVotes;
            _hasRatedFestival = response.Data.HasRated;
        }
    }

    private async Task RateFestival()
    {
        if (_selectedFestivalRating is < 1 or > 5 || _hasRatedFestival)
            return;

        var response = await FestivalManager.AddReview(new AddReviewCommand
        {
            FestivalId = FestivalId,
            Rate = _selectedFestivalRating,
            Type = CommentType.Review,
            Text = string.Empty
        });

        if (response.Succeeded)
        {
            _selectedFestivalRating = 0;
            await LoadRatingSummary();
            _ratingRefreshToken++;
            _snackBar.Add("Thank you for rating this festival.", Severity.Success);
        }
        else
        {
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Warning);
        }
    }

    private async Task LoadUserSubmit()
    {
        _currentUserSubmits = [];
        _canReview = false;
        _reviewEligibilityMessage = null;

        if (!UserIsAuthenticated)
        {
            _reviewEligibilityMessage = "Sign in with an accepted submission to review this festival.";
            return;
        }

        var response = await SubmitManager.GetAll(new GetAllSubmitsRequest()
        {
            GetAllData = true,
            GetCurrentUserSubmits = true,
        });
        if (response.Succeeded)
        {
            _currentUserSubmits = response.Data
                .Where(p => p.FestivalId == FestivalId)
                .ToList();

            var acceptedStatuses = new[]
            {
                JudgingStatus.Selected,
                JudgingStatus.AwardWinner,
                JudgingStatus.Finalist,
                JudgingStatus.SemiFinalist,
                JudgingStatus.QuarterFinalist,
                JudgingStatus.Nominee,
                JudgingStatus.HonorableMention
            };

            var acceptedSubmission = _currentUserSubmits
                .FirstOrDefault(p => acceptedStatuses.Contains(p.JudgingStatus));
            if (acceptedSubmission != null)
            {
                _currentUserSubmitId = acceptedSubmission.Id;
                if (Festival?.EventEndDate is { } eventEndDate &&
                    DateTime.Now >= eventEndDate.AddDays(14))
                {
                    _canReview = true;
                }
                else
                {
                    _reviewEligibilityMessage =
                        "Reviews become available two weeks after the festival ends.";
                }
            }
            else
                _reviewEligibilityMessage =
                    "Only participants with an accepted submission can review this festival.";
        }
        else if (response.Messages?.Any() == true)
            _reviewEligibilityMessage = response.Messages.First();

        StateHasChanged();
    }

    private async Task CopyLinkToClipboard()
    {
        var text = _navigationManager.Uri;
        await _jsRuntime.InvokeVoidAsync("shareLink", text, Festival?.Name);
    }

    private async Task ViolationReport()
    {
        var parameters = new DialogParameters
        {
            { nameof(ReviewDialog.Type), CommentType.ViolationReport },
            { nameof(ReviewDialog.FestivalId), FestivalId }
        };
        var options = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            
        };
        var dialog = _dialogService.Show<ReviewDialog>(Localize["Review"], parameters, options);
    }

    #region Prerendering

    private PersistingComponentStateSubscription _subscription;
    private FestivalTimeLine _timeLines;
    
    private Task PersistFestival()
    {
        ApplicationState.PersistAsJson(StateKey("imageCovers"), _imageCovers);
        ApplicationState.PersistAsJson(StateKey("Organizers"), Organizers);
        ApplicationState.PersistAsJson(StateKey("products"), _products);
        ApplicationState.PersistAsJson(StateKey("festivalNews"), _festivalNews);
        ApplicationState.PersistAsJson(StateKey("tickets"), _tickets);
        ApplicationState.PersistAsJson(StateKey("festival"), Festival);
        ApplicationState.PersistAsJson(StateKey("qualifiers"), Qualifiers);
        return Task.CompletedTask;
    }

    private string StateKey(string name) =>
        $"festival:{FestivalUrl?.Trim().ToLowerInvariant()}:{name}";

    #endregion

    private async Task Submit()
    {
        var festivalId = Festival.Id;
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

    #region Qualifiers

    private List<GetAllFestivalQualifiersResponse> Qualifiers { get; set; } = new();

    private async Task LoadQualifiers()
    {
        if (ApplicationState.TryTakeFromJson<List<GetAllFestivalQualifiersResponse>>
                (StateKey("qualifiers"), out var stored))
        {
            Qualifiers = stored;
        }
        else
        {
            var response = await FestivalManager.GetAllFestivalQualifires(new GetAllFestivalQualifiersQuery()
            {
               FestivalId = FestivalId
            });
            if (response.Succeeded)
            {
                Qualifiers = response.Data;
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

    #endregion
    
    #region Organizer

    private List<GetAllEventOrganizerResponse> Organizers { get; set; } = new();
    public string CommentText { get; set; }


    private async Task LoadOrganizer()
    {
        if (ApplicationState.TryTakeFromJson<List<GetAllEventOrganizerResponse>>
                (StateKey("Organizers"), out var stored))
        {
            Organizers = stored;
        }
        else
        {
            var response = await FestivalManager.GetAllOrganizerAsync(new GetAllOrganizerQuery()
            {
                FestivalId = Festival.Id,
            });
            if (response.Succeeded)
            {
                Organizers = response.Data;
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

    #endregion

    private async Task UpdateLike()
    {
        if (!UserIsAuthenticated)
        {
            _dialogService.Show<NeedToLogin>("Need To Login", new DialogParameters(),
                new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true });
            return;
        }

        var res = await FestivalManager.AddDeleteLike(new BaseFestivalRequest()
        {
            FestivalId = Festival.Id,
        });
        if (res.Succeeded)
        {
            _liked = !_liked;
            _likedCount=_liked ? _likedCount+1 : _likedCount-1;
        }
    }
    
    
    
    // private async Task Review()
    // {
    //     var parameters = new DialogParameters
    //     {
    //         { nameof(ReviewDialog.Type), CommentType.ViolationReport },
    //         { nameof(ReviewDialog.FestivalId), FestivalId }
    //     };
    //     var options = new DialogOptions
    //     {
    //         FullWidth = true,
    //         CloseButton = true,
    //         MaxWidth = MaxWidth.Small,
    //         
    //     };
    //     var dialog = _dialogService.Show<ReviewDialog>(Localize["Review"], parameters, options);
    //     var result = await dialog.Result;
    //     if (!result.Canceled)
    //     {
    //       //  await _table.ReloadServerData();
    //     }
    // }

    private AddReviewCommand _review = new();
    private async Task AddComment()
    {
        if (!_canReview || string.IsNullOrWhiteSpace(_review.Text))
            return;
        
        _review.Type = CommentType.Review;
        _review.FestivalId = Festival.Id;

        var res = await SubmitManager.Review(_review);
        if (res.Succeeded)
        {
            _snackBar.Add(res.Messages[0], MudBlazor.Severity.Success);
            _review.Text = string.Empty;
        }
    }
}

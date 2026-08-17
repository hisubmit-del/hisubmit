using System;
using MudBlazor;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using HiSubmit.Client.Infrastructure.Managers.Dashboard;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Queries;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using Hisubmit.Client.SharedModels.Features.SubUsers.Queries.GetFestivalUsers;
using HiSubmit.Client.Infrastructure.Managers.EventCategoris;
using HiSubmit.Client.Infrastructure.Managers.FestivalNews;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using HiSubmit.Client.Infrastructure.Managers.SoldTickets;

namespace Web.Components.Pages.Festival;

public partial class Dashboard
{
    #region Inject

    [Inject] private ISubmitManager SubmitManager { get; set; }
    [Inject] private IFestivalManager FestivalManager { get; set; }
    [Inject] private IDashboardManager DashboardManager { get; set; }
    [Inject] public IEventCategoryManager EventCategoryManager { get; set; }
    [Inject] public IFestivalNewsManager FestivalNewsManager { get; set; }
    [Inject] public IFestivalSubUserManager FestivalSubUserManager { get; set; }
    [Inject] private IFestivalSoldTicketManager FestivalSoldTicketManager { get; set; }

    #endregion

    private bool _loaded;

    #region Data Count

    private int _ticketsSoldCount;
    private int _userCount;
    private int _reviewCount;
    private int _newsCount;
    private int _categoryCount;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadSelectedFestivalId();
        await LoadFestivalDetail();
        await LoadCount();
        await LoadSubmits();
        _loaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }


    private bool _festivalLoaded;

    private GetFestivalDetailResponse _festival = new();

    private async Task LoadFestivalDetail()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
        {
            _festival = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        _festivalLoaded = true;
    }

    #region Submits

    private List<GetAllSubmitsResponse> _submits = new();
    private bool _submitLoaded;
    private int _submitCount;

    #endregion

    private async Task LoadCount()
    {
        await LoadCategories();
        await LoadReviews();
        await LoadNews();
        await LoadUsers();
        await LoadSoldTickets();
        // StateHasChanged();
    }

    private async Task LoadSubmits()
    {
        var response = await SubmitManager.GetAll(new GetAllSubmitsRequest()
        {
            FestivalId = SelectedFestivalId,
            GetAllData = true,
        });
        if (response.Succeeded)
        {
            _submits = response.Data;
            if (response.Data != null)
                _submitCount = response.Data.Count;
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

        _submitLoaded = true;
    }

    private async Task LoadSoldTickets()
    {
        var response = await FestivalSoldTicketManager.GetAllSoldTicket(new GetAllSoldTicketQuery
        {
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
            _ticketsSoldCount = response.Data?.Count ?? 0;
    }

    private async Task LoadCategories()
    {
        var response = await EventCategoryManager.GetAllAsync(new GetAllEventCategoryQuery
        {
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
            _categoryCount = response.Data.Count;
    }

    private async Task LoadReviews()
    {
        var response = await FestivalManager.GetAllReview(new GetAllReviewQuery()
        {
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
            _reviewCount = response.Data.Count;
    }

    private async Task LoadNews()
    {
        var response = await FestivalNewsManager.GetAllAsync(new GetAllNewRequest()
        {
            FestivalId = SelectedFestivalId,
            GetAllData = true
        }, SelectedFestivalId);

        if (response.Succeeded)
            _newsCount = response.Data.Count;
    }

    private async Task LoadUsers()
    {
        var response = await FestivalSubUserManager.GetFestivalUserAsync(new GetFestivalSubUserQuery
        {
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
            _userCount = response.Data.Count;
    }


    // private List<GetAllSubmitsResponse> Submits = new List<GetAllSubmitsResponse>()
    // {
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today, Id = 1, Name = "festival",
    //         JudgingStatus = JudgingStatus.Nominee,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 1, EventCategoryId = 1, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category1"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 2, EventCategoryId = 3, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category3"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 2, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category2"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 4, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category4"
    //             }
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(3), Id = 1, Name = "festival",
    //         JudgingStatus = JudgingStatus.Selected,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 2, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category2"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 4, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category4"
    //             }
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(3), Id = 2, Name = "festival",
    //         JudgingStatus = JudgingStatus.Finalist,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 2, EventCategoryId = 3, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category3"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 2, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category2"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 4, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category4"
    //             }
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(1), Id = 3, Name = "festival",
    //         JudgingStatus = JudgingStatus.Nominee,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 1, EventCategoryId = 1, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category1"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 2, EventCategoryId = 3, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category3"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 2, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category2"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 4, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category4"
    //             }
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(2), Id = 4, Name = "festival",
    //         JudgingStatus = JudgingStatus.Nominee,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 4, DeadLineId = 2, DeadLineName = "deadline2",
    //                 EventCategoryName = "category4"
    //             }
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(4), Id = 5, Name = "festival",
    //         JudgingStatus = JudgingStatus.Undecided,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 1, EventCategoryId = 1, DeadLineId = 2, DeadLineName = "deadline2",
    //                 EventCategoryName = "category1"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 2, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category2"
    //             }
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(3), Id = 6, Name = "festival",
    //         JudgingStatus = JudgingStatus.Undecided,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 1, EventCategoryId = 1, DeadLineId = 3, DeadLineName = "deadline3",
    //                 EventCategoryName = "category1"
    //             },
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 2, EventCategoryId = 3, DeadLineId = 1, DeadLineName = "deadline1",
    //                 EventCategoryName = "category3"
    //             },
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(2), Id = 7, Name = "festival",
    //         JudgingStatus = JudgingStatus.QuarterFinalist,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 3, EventCategoryId = 4, DeadLineId = 2, DeadLineName = "deadline2",
    //                 EventCategoryName = "category4"
    //             }
    //         }
    //     },
    //     new GetAllSubmitsResponse()
    //     {
    //         SubmitDateFrom = DateTime.Today + TimeSpan.FromDays(2), Id = 8, Name = "festival",
    //         JudgingStatus = JudgingStatus.SemiFinalist,
    //         DeadlineEventCategories = new List<DeadLineCategoryDto>()
    //         {
    //             new DeadLineCategoryDto()
    //             {
    //                 Id = 1, EventCategoryId = 1, DeadLineId = 2, DeadLineName = "deadline2",
    //                 EventCategoryName = "category1"
    //             },
    //         }
    //     }
    // };
}

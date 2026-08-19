using System;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLineEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using HiSubmit.Client.Infrastructure.Managers.EventCategoris;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Components.Pages.Public.Festivals.Components
{
    public partial class FestivalCategories
    {
        #region Injection

        [Inject] public IPublicFestivalManager PublicFestivalManager { get; set; }

        [Inject] public IEventCategoryManager EventCategoryManager { get; set; }
        [Inject] public ISubmitManager SubmitManager { get; set; }

        #endregion

        [Parameter] public int FestivalId { get; set; }

        private List<GetAllDeadLineEventCategoryResponse> DeadLineCategories { get; set; } = new();


        private List<GetAllEventCategoryResponse> EventCategories { get; set; }

        private IEnumerable<IGrouping<int, GetAllDeadLineEventCategoryResponse>> DeadlineCategoriesGrouping { get; set; }
            = Enumerable.Empty<IGrouping<int, GetAllDeadLineEventCategoryResponse>>();

        private bool _loaded;
        private int _loadedFestivalId;

        protected override async Task OnInitializedAsync()
        {
            _subscription = ApplicationState.RegisterOnPersisting(PersistFestival);
            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (FestivalId <= 0 || (_loaded && _loadedFestivalId == FestivalId))
            {
                return;
            }

            _loaded = false;
            _loadedFestivalId = FestivalId;
            await LoadDeadLineCategories();
            GroupedCategories();
            _loaded = true;
        }

        private async Task LoadDeadLineCategories()
        {
            if (ApplicationState.TryTakeFromJson<List<GetAllDeadLineEventCategoryResponse>>(StateKey("deadLineCategories"),
                    out var stored))
            {
                DeadLineCategories = stored ?? new();
            }
            else
            {
                var response = await PublicFestivalManager.GetAllGetDeadLineCategory(
                    new GetAllDeadLineEventCategoryQuery()
                    {
                        FestivalId = FestivalId
                    });

                if (response.Succeeded)
                {
                    DeadLineCategories = response.Data ?? new();
                }
                else
                {
                    DeadLineCategories = new();
                    foreach (var message in response.Messages ?? Enumerable.Empty<string>())
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private Task SubmitToFestival(GetAllDeadLineEventCategoryResponse deadCat)
        {
            var selectCats = new List<int> { deadCat.Id };
            var parameter = new DialogParameters
            {
                { nameof(FestivalCategorySelected.FestivalId), FestivalId },
                { nameof(FestivalCategorySelected.SelectedCategoryId), selectCats }
            };

            var options = new DialogOptions
            {
                FullWidth = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                
            };
            _dialogService.Show<FestivalCategorySelected>("Selected category", parameter, options);
            return Task.CompletedTask;
        }

        private void GroupedCategories()
        {
            if (DeadLineCategories.Count == 0)
            {
                DeadlineCategoriesGrouping = Enumerable.Empty<IGrouping<int, GetAllDeadLineEventCategoryResponse>>();
                return;
            }

            var f =
                DeadLineCategories.GroupBy(p => p.EventCategoryId);
            foreach (var r in f)
            {
                var nearDeadLine =
                    r.Where(p => p.DeadLineDate.Date >= DateTime.Now.Date)
                        .MinBy(p => p.DeadLineDate.Date);
                if (nearDeadLine != null)
                {
                    nearDeadLine.Nearest = true;
                }
            }

            DeadlineCategoriesGrouping = f;
        }

        #region Prerendering

        private PersistingComponentStateSubscription _subscription;

        private Task PersistFestival()
        {
            ApplicationState.PersistAsJson(StateKey("deadLineCategories"), DeadLineCategories);
            return Task.CompletedTask;
        }

        private string StateKey(string name) => $"festival-categories:{FestivalId}:{name}";

        #endregion
    }
}

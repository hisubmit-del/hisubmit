using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitCategories;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubmit;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Festival.DashboardComponent;

public partial class CategorySubmitChart
{
    [Inject] private IFestivalSubmitManager FestivalSubmitManager { get; set; }


    [Parameter] public int FestivalId { get; set; }

    private List<GetAllSubmitCategoriesResponse> _submitCategory = new();


    private bool _loaded { get; set; }

    //  private List<DeadLineCategoryDto> DeadLineCategoriesDto { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCategories();
        GenerateSeries();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    private int Tick = 1;
    private int MaxTick = 10;
    private string[] xLabels = { };

    private List<ChartSeries> Series = new List<ChartSeries>();


    private async Task LoadCategories()
    {
        var response = await FestivalSubmitManager.GetAllSubmitCategoriesAsync(new GetAllSubmitCategoriesQuery
        {
            FestivalId = FestivalId,
            Type = RequestSubmitCategoriesType.Festival,
            GetAllData = true
        });
        if (response.Succeeded)
            _submitCategory = response.Data;
    }

    private void GenerateSeries()
    {
        // foreach (var submit in _submitCategory)
        // {
        //     DeadLineCategoriesDto.AddRange(submit.DeadlineEventCategories);
        // }

        var categoryGroupedSubmits = _submitCategory
            .GroupBy(p => p.EventCategoryName).ToList();

        xLabels = new string[categoryGroupedSubmits.Count + 1];
        var serie = new ChartSeries()
        {
            Name = "category Submit",
            Data = new double[categoryGroupedSubmits.Count + 1]
        };

        xLabels[0] = string.Empty;
        serie.Data[0] = 0;

        for (var i = 1; i <= categoryGroupedSubmits.Count; i++)
        {
            var catName = _submitCategory
                .Where(p => p.EventCategoryName == categoryGroupedSubmits[i - 1].Key)
                .Select(p => p.EventCategoryName).FirstOrDefault();

            xLabels[i] = catName;
            serie.Data[i] = categoryGroupedSubmits[i - 1].Count();
        }

        SetMaxTick((int)serie.Data.Max());
        SetTick();
        Series.Add(serie);
    }

    private void SetMaxTick(int maxValue)
    {
        do
        {
            maxValue++;
        } while (maxValue % 5 == 0);

        MaxTick = maxValue;
    }

    private void SetTick()
    {
        Tick = MaxTick / 10;
        if (Tick < 1)
            Tick = 1;
    }
}
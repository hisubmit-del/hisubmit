using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubmit;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitCategories;

namespace HiSubmit.Web.Components.Pages.Festival.ProjectJudgings.SubmitDetailComponents;

public partial class SelectedCategories
{
    #region Injects

    [Inject] private IFestivalSubmitManager FestivalSubmitManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int FestivalId { get; set; }
    [Parameter] public int SubmitId { get; set; }

    #endregion

    private List<GetAllSubmitCategoriesResponse> _submitCategories;


    protected  override async Task OnInitializedAsync()
    {
        await LoadCategories();
        await base.OnInitializedAsync();
    }

    private async Task LoadCategories()
    {
        var response = await FestivalSubmitManager.GetAllSubmitCategoriesAsync(new GetAllSubmitCategoriesQuery
        {
            SubmitId = SubmitId,
            FestivalId = FestivalId
        });
        if (response.Succeeded)
            _submitCategories = response.Data;
    }
}
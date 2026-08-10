using MudBlazor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Queries;
using HiSubmit.Client.Infrastructure.Managers.FestivalProductsSold;

namespace HiSubmit.Web.Components.Pages.Festival.ProductsSold;

public partial class ProductSoldDetailDialog
{
    #region Injects

    [Inject] private IFestivalSoldProductManager FestivalSoldProductManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int FestivalId { get; set; }
    [Parameter] public int ProductSoldId { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    #endregion

    #region Private Field

    private bool _loaded;
    private GetSoldProductDetailResponse _productSold;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadDetail();
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task LoadDetail()
    {
        var response = await FestivalSoldProductManager.GetById(new GetSoldProductDetailQuery
        {
            Id = ProductSoldId,
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            _productSold = response.Data;
            _loaded = true;
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private void Close()
    {
        MudDialog.Close();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Public.Festivals.Components;

public partial class FestivalProducts
{
    #region Inject

    [Inject] public IPublicFestivalManager PublicFestivalManager { get; set; }

    #endregion

    #region Parameters

    [CascadingParameter] public int FestivalId { get; set; }

    #endregion

    #region Private Feild

    private List<GetAllPagedProductsResponse> _products=new();

    #endregion
    
    

    private async Task LoadProducts()
    {
        var response = await PublicFestivalManager.GetAllProducts(new GetAllProductsRequest
        {
            GetAllData = true,
            FestivalId = FestivalId,
        });

        if (response.Succeeded)
            _products = response.Data;
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }
}

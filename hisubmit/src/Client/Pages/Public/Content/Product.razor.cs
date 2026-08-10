using System.Collections.Generic;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.Contents;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using HiSubmit.Client.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Commands;
using HiSubmit.Client.SharedModels.Wrapper;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Products.Queries.GetById;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace HiSubmit.Client.Pages.Public.Content;

public partial class Product
{
    [Parameter]
    public int Id { get; set; }
    [Inject] private IPublicFestivalManager FestivalManager { get; set; }

    [Inject] private IContentManager ContentManager { get; set; }
    private AddEditProductRequest _product = new();

    private PaginatedResult<GetAllFestivalResponse> _festivalResponse =
        new (new List<GetAllFestivalResponse>());
    private PaginatedResult<GetAllNewResponse> _newsResponse = new (new List<GetAllNewResponse>());
    
    #region Prerendering
    
    private PersistingComponentStateSubscription _subscription;
    private bool _loaded;
    private Task PersistFestival()
    {
        ApplicationState.PersistAsJson("festivals", _festivalResponse);
        ApplicationState.PersistAsJson("product", _product);
        ApplicationState.PersistAsJson("news", _newsResponse);
        return Task.CompletedTask;
    }

    #endregion

    protected override async Task OnInitializedAsync()
    {
        _subscription=ApplicationState.RegisterOnPersisting(PersistFestival);
        await LoadProduct();
        await LoadFestivals();
        await LoadNews();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
       // if (firstRender)
            await _jsRuntime.InvokeVoidAsync("CreateProductImageSlider");
        
    }
    
    
     private async Task LoadProduct()
        {
            if (ApplicationState.TryTakeFromJson
                    <AddEditProductRequest>
                    ("product", out var stored))
            {
                _product = stored;
            }
            else
            {
                var response = await FestivalManager.GetProductById(new GetProductByIdRequest()
                {
                    Id = Id
                });
                if (response.Succeeded)
                    _product = response.Data;
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
                var response = await FestivalManager.GetAllFestival(new GetAllFestivalRequest()
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
        
        private async Task AddToCart()
        {
            // if ((await AuthenticationManager.CurrentUser()).Identity.IsAuthenticated)
            // {
                var productSold = new AddProductSoldCommand
                {
                    ProductId = _product.Id,
                    ProductType = _product.ProductType,
                    Address = new AddEditAddressCommand(),
                    Status = ProductSoldStatus.AwaitingPayment
                };
                var parameters = new DialogParameters
                {
                    { nameof(AddProductSold.Product), productSold }
                };
                var options = new DialogOptions
                {
                    FullWidth = true,
                    MaxWidth = MaxWidth.Small,
                    
                };
                _dialogService.Show<AddProductSold>("Add Product To Cart", parameters, options);
            // }
            // else
            // {
            //  //   await NeedToLogin();
            // }
            
        }

}
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Shared.Components.Base;
using HiSubmit.Client.Infrastructure.Managers.Catalog.Brand;
using HiSubmit.Client.Infrastructure.Managers.Catalog.Product;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Products.Queries.GetById;
using HiSubmit.Client.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Client.Pages.Festival.Products;

public partial class AddProduct:BaseFestival
{
    
    #region Inject

    [Inject] private IProductManager ProductManager { get; set; }
    [Inject] private IArtCategoryManager BrandManager { get; set; }

    #endregion
    [Parameter]
    public int ProductId { get; set; }

    private AddEditProductRequest Model = new(){UploadRequest = new UploadRequest(){UploadType = UploadType.Product}};
    
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated = true;
    private bool _processing;
    
    private async Task SaveAsync()
    {
        Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        if (Validated)
        {
            _processing = true;
            Model.FestivalId = SelectedFestivalId;
            var response = await ProductManager.SaveAsync(Model);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                await PendingApproval();
                _navigationManager.NavigateTo("/festival/products");
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        _processing = false;
    }

    private async Task PendingApproval()
    {
        var options = new DialogOptions()
        {
            BackdropClick = true,
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        };
        var d = await _dialogService.ShowAsync<ApprovedEmail>("Admin Approval", options);
        var res = await d.Result;
    }
    private void AddImage()
    {
        Model.ProductImages.Add(new ProductImageDto() { });
    }

    private void DeleteImage(ProductImageDto image)
    {
        Model.ProductImages.Remove(image);
    }

    private async Task LoadProducts()
    {
        var response = await ProductManager.GetByIdAsync(new GetProductByIdRequest(){Id = ProductId},SelectedFestivalId);
        if (response.Succeeded)
            Model = response.Data;
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
    
    protected override async Task OnInitializedAsync()
    {
        await LoadSelectedFestivalId();
        if (ProductId != 0)
        {
            await LoadProducts();
        }
        await base.OnInitializedAsync();
    }
}
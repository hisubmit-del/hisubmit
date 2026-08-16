using MudBlazor;
using Web.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using HiSubmit.Client.Infrastructure.Managers.AdminProducts;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Hisubmit.Client.SharedModels.Features.Products.Commands.Enable;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;

namespace Web.Components.Pages.Admin.Products
{
    public partial class Products
    {
        #region Inject

        [Inject] private IAdminProductManager ProductManager { get; set; }

        #endregion

        #region Parameters

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        [Parameter] public int? FestivalId { get; set; }

        #endregion

        #region Private Feild

        private bool _loaded;
        private int _totalItems;
        private string _searchString = "";
        private IEnumerable<GetAllPagedProductsResponse> _pagedData;
        private MudTable<GetAllPagedProductsResponse> _table;

        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
            await SeenNotification();
        }

        private async Task SeenNotification()
        {
            await NotificationManager.SeenNotifications(new SeenNotificationCommand
            {
                NotificationTypes = NotificationType.AdminNewAddedProduct,
                FestivalId = FestivalId,
                AccountType = SiteAccountType.Admin,
            });
            NotificationService.ChangeNotificationBar();
        }

        private async Task<TableData<GetAllPagedProductsResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }

            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllPagedProductsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None
                    ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                    : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllProductsRequest
            {
                FestivalId = FestivalId,
                PageSize = pageSize,
                PageNumber = pageNumber + 1,
                SearchString = _searchString,
                Orderby = orderings
            };
            var response = await ProductManager.GetAll(request);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _pagedData = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        // private async Task InvokeModal(int id = 0)
        // {
        //     var parameters = new DialogParameters
        //     {
        //         { nameof(AddEditProductModal.FestivalId), FestivalId }
        //     };
        //     if (id != 0)
        //     {
        //         var product = _pagedData.FirstOrDefault(c => c.Id == id);
        //         if (product != null)
        //         {
        //             parameters.Add(nameof(AddEditProductModal.AddEditProductModel), new AddEditProductRequest
        //             {
        //                 Id = product.Id,
        //                 Name = product.Name,
        //                 Description = product.Description,
        //                 Price = product.Price,
        //                 FestivalId = product.FestivalId,
        //                 //    Barcode = product.Barcode
        //             });
        //         }
        //     }
        //
        //     var options = new DialogOptions
        //         { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true,  };
        //     var dialog = _dialogService.Show<AddEditProductModal>(id == 0 ? Localize["Create"] : Localize["Edit"],
        //         parameters, options);
        //     var result = await dialog.Result;
        //     if (!result.Canceled)
        //     {
        //         OnSearch("");
        //     }
        // }


        private async Task ChangeEnable(int contextId, bool enable)
        {
            _loaded = false;
          //  Thread.Sleep(3000);
            var response = await ProductManager.UpdateEnable(new EnableProductCommand
            {
                IsEnable = enable,
                ProductId = contextId
            });
            if (response.Succeeded)
                await _table.ReloadServerData();
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
            _loaded = true;
        }
    }
}
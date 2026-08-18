using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Managers.Payments;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Components.Shared.Components
{
    public partial class CartItems
    {
        [Inject]
        public ICartManager CartManager { get; set; }

        public List<GetCartItemResponse> Items { get; set; } = new();


        protected override async Task OnInitializedAsync()
        {
            await LoadCartItems();
            await base.OnInitializedAsync();
        }

        private async Task LoadCartItems()
        {
            var response = await CartManager.GetItems(new GetUserOpenCartItemQuery()
            {
                UserId = string.Empty
            }) ;

            if (response.Succeeded)
            {
                Items = response.Data;
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
}

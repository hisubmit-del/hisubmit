using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Brands.Commands.AddEdit;
using Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Catalog.Brand;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace Web.Components.Pages.Admin.Catalog
{
    public partial class AddEditArtCategoryModal
    {
        [Inject] private IArtCategoryManager ArtCategoryManager { get; set; }

        [Parameter] public AddEditArtCatgoryRequest ArtCategoryModel { get; set; } = new();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
    

        private bool _processing ;
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {

            _processing = true;
            var response = await ArtCategoryManager.SaveAsync(ArtCategoryModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _processing = false;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private static async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}
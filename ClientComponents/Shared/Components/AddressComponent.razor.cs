using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Locations;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClientComponents.Shared.Components
{
    public partial class AddressComponent
    {
        [Parameter]
        public AddEditAddressCommand Address { get; set; } = new();

        private List<GetAllCountryResponse> Countries { get; set; } = new();


        private bool Validated { get; set; } = true;

        [Inject]
        private ILocationManager LocationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetCountriesAsync();
            await base.OnInitializedAsync();
        }

        private async Task GetCountriesAsync()
        {
            var result = await LocationManager.GetAllCountryAsync(new GetAllCountryQuery());
            if (result.Succeeded)
            {
                Countries = result.Data;
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, MudBlazor.Severity.Error);
                }
            }
        }
        private async Task<IEnumerable<int>> SearchCountries(string value,CancellationToken token)
        {
            // In real life use an asynchronous function for fetching data from an api.
            //await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Countries.Select(x => x.Id);

            return Countries.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }
        public AddEditAddressCommand GetAddress()
        {
            if (Validated)
            {
                return Address;
            }
            else
            {
                return null;
            }
        }
    }
}

using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Locations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Components.Shared.Components
{
    public partial class CountrySelector
    {
        [Inject]
        private ILocationManager LocationManager { get; set; }

        [Parameter]
        public int CountryId { get; set; }
        [Parameter]
        public EventCallback<int> CountryIdChanged { get; set; }
        

        [Parameter]
        public EventCallback<int> CountryChanged { get; set; }

        [Parameter]
        public Variant SelectorVariant { get; set; } = Variant.Filled;

        public List<GetAllCountryResponse> Countries { get; set; } = new();


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
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        private  async Task<IEnumerable<int>> SearchCountries(string value,CancellationToken token)
        {
            if (string.IsNullOrEmpty(value))
                return Countries.Select(x => x.Id);

            return Countries.Where(x => 
                    x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
            
        }
        private void ChangeCountry(int id)
        {
            CountryId= id;
            Task.Run(async () =>
            {
                await CountryIdChanged.InvokeAsync(id);
                await CountryChanged.InvokeAsync(id);
            });
        }
    }
}

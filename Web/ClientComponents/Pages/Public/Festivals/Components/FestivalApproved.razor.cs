using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientComponents.Pages.Public.Festivals.Components
{
    public partial class FestivalApproved
    {
        [Inject]
        private IPublicFestivalManager FestivalManager { get; set; }

        [Parameter]
        public List<int> SelectedQualifures { get; set; }

        private List<GetAllFestivalQualifiersResponse> _qualifires = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            var response = await FestivalManager
                .GetAllFestivalQualifires(new GetAllFestivalQualifiersQuery());

            if (response.Succeeded)
                _qualifires = response.Data;
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, MudBlazor.Severity.Error);
        }

        
    }
}

using AdminDashboard.Wasm.Models;
using Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;
using Hisubmit.Client.SharedModels.Features.MediaRights.Queries;
using HiSubmit.Client.Infrastructure.Managers.MediaRights;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSubmit.Client.Pages.Project.Awards
{
    public partial class ProjectDistribuationInformationItem
    {
        [Parameter]
        public AddEditDistributionInformationItemRequest Item { get; set; }

        [Inject]
        private IMediaRightManager MediaRightManager { get; set; }


        [Parameter]
        public EventCallback OnDelete { get; set; }

        private List<CheckBoxItem<int>> MediaRightItem { get; set; } = new();
        private List<GetAllMediaRightResponse> MediaRights { get; set; }

        public bool _Loaded = false;
        protected override async Task OnInitializedAsync()
        {
            await LoadMediaRights();
            await GenerateMediaRightCheckBox();
            await base.OnInitializedAsync();
            _Loaded = true;
        }

        private async Task LoadMediaRights()
        {
            var response = await MediaRightManager.GetAllAsync(new GetAllMediaRightQuery());
            if (response.Succeeded)
            {
                Console.WriteLine("Media Right Count:{0}",response.Data.Count);
                MediaRights = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, MudBlazor.Severity.Error);
                }
            }
        }
        private async Task GenerateMediaRightCheckBox()
        {
            await Task.Run(() =>
            {
                foreach (var right in MediaRights)
                {
                    bool selected = Item.MediaRightIds.Any(id => right.Id == id);
                    MediaRightItem.Add(new CheckBoxItem<int>
                    {
                        IsSelected = selected,
                        Name = right.Name,
                        Value = right.Id
                    });
                }
            });
        }

        private void ChangeMediaRightId(int id)
        {

            MediaRightItem.FirstOrDefault(p => p.Value == id).IsSelected = !MediaRightItem.FirstOrDefault(p => p.Value == id).IsSelected;
            if (Item.MediaRightIds.Any(p => p == id))
            {
                Item.MediaRightIds.Remove(id);
            }
            else
            {
                Item.MediaRightIds.Add(id);
            }
        }

        private async Task DeleteItem()
        {
            await OnDelete.InvokeAsync();
        }
    }
}

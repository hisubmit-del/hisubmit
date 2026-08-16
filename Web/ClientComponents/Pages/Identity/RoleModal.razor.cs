using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.SubUsers.Commands.AddEditRoles;
using Hisubmit.Client.SharedModels.Requests.Identity;
using ClientComponents.Extensions;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using HiSubmit.Client.Infrastructure.Managers.Identity.Roles;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;

namespace ClientComponents.Pages.Identity
{
    public partial class RoleModal
    {
        [Inject] private IRoleManager RoleManager { get; set; }
        [Inject] private IFestivalSubUserManager FestivalSubUserManager { get; set; }

        [Parameter] public RoleRequest RoleModel { get; set; } = new();
        [Parameter] public bool IsFestivalRole { get; set; }
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task SaveAsync()
        {
            IResult<string> response;
            if (IsFestivalRole)
            {
                response = await FestivalSubUserManager.SaveRole(new AddEditFestivalRoleRequest()
                {
                    Name=RoleModel.Name,
                    Id=RoleModel.Id,
                    Description=RoleModel.Description
                });
            }
            else
            {
                response = await RoleManager.SaveAsync(RoleModel);
            }
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}
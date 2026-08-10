using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.SubUsers.Queries.GetFestivalUsers;
using Hisubmit.Client.SharedModels.Responses.Identity;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using HiSubmit.Client.Infrastructure.Managers.JudgingProjects;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Web.Components.Pages.Festival.JudgingProjects;

public partial class AddReferesToProject
{
    #region Private Field

    private bool _processing;
    private string _searchString = "";
    private List<UserResponse> _userList = new();
    private HashSet<UserResponse> _selectedUsers = new();
    
    #endregion

    #region  Parameters
    [Parameter] public List<int> SubmitId { get; set; }
    [Parameter] public int FestivalId { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
    

    #endregion

    #region Injects

    
    [Inject] public ILocalStorageService LocalStorageService { get; set; }

    [Inject] public IProjectJudgingManager ProjectJudgingManager { get; set; }

    [Inject] public IFestivalSubUserManager FestivalSubUserManager { get; set; }


    #endregion
    protected override async Task OnInitializedAsync()
    {
        await GetUsersAsync();
        await LoadSelectedReferees();
    }

    private async Task GetUsersAsync()
    {
        var response = await FestivalSubUserManager
            .GetFestivalUserAsync(new GetFestivalSubUserQuery
            {
                FestivalId = FestivalId
            });
        if (response.Succeeded)
        {
            _userList = response.Data.ToList();
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

    }

    private bool Search(UserResponse user)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (user.FullName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private async Task AddToProject()
    {
        _processing = true;
        var selectedUserId = _selectedUsers.Select(p => p.Id).ToList();
        var response = await ProjectJudgingManager.AddJudging(
            new AddEditProjectJudgingCommand
                (SubmitId, selectedUserId, FestivalId, false)
                {
                    MultiProjectToMultiReferee = SubmitId.Count>1
                });
     
        if (response.Succeeded)
        {
            _snackBar.Add(Localize["Successfully added to Project"], Severity.Success);
            MudDialog.Close();
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

        _processing = false;
    }

    private async Task LoadSelectedReferees()
    {
        if (SubmitId.Count == 1)
        {
            var response 
                = await ProjectJudgingManager.GetAll(new GetAllProjectJudgingQuery
                {
                    SubmitId = SubmitId.First()
                });
            if (response.Succeeded)
            {
                _selectedUsers = _userList
                    .Where(user => response.Data.Any(projectJudging => projectJudging.UserId == user.Id))
                    .ToHashSet();
                StateHasChanged();
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
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using HiSubmit.Client.Infrastructure.Managers.Referee;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;
using Hisubmit.Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetRefereeData;
using Microsoft.AspNetCore.Components;
using Severity = MudBlazor.Severity;

namespace ClientComponents.Pages.User.Referee;

public partial class Dashboard
{
    [Inject] private IRefereeManager RefereeManager { get; set; }

    
    [Parameter]
    public string UserId { get; set; }
    
    private GetRefereeDataResponse _refereeData = new();
    private List<GetAllProjectJudgingResponse> _pagedDate=new();


    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        await LoadJudgingData();
        await base.OnInitializedAsync();
    }

    private async Task LoadData()
    {
        var response =
            await RefereeManager.GetRefereeData(new GetRefereeDataRequest
                { GetCurrentUserData =string.IsNullOrWhiteSpace(UserId) , UserId=UserId });
        if (response.Succeeded)
            _refereeData = response.Data;
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message);
    }
    
    private async Task LoadJudgingData()
    {
        var query = new GetAllProjectJudgingQuery();
        query.PageSize = 8;
        query.PageNumber =1;
        query.GetCurrentUser = true;
        query.UserId = UserId;
        query.GetCurrentUser = string.IsNullOrWhiteSpace(UserId);

        var response = await RefereeManager.GetAllAsync(query);

        if (response.Succeeded)
        {
         
            var data = response.Data;
           
            _pagedDate = data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message,Severity.Error);
            }
        }
    }
}
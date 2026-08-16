using System;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Payments.Commands.EditSiteCommission;
using Hisubmit.Client.SharedModels.Models.Chat;
using HiSubmit.Client.Infrastructure.Managers.SiteSetting;
using HiSubmit.Client.Pages.Project;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Admin.Payment;

public partial  class Commissions
{
    [Inject]
    private  ISiteSettingManager SiteSettingManager { get; set; }
    [Inject]
    private  IMapper Mapper { get; set; }

    private EditSiteCommissionCommand _commissions { get; set; }= new();

private  string Name { get; set; }
    private bool _processing;
    private FluentValidationValidator _fluentValidationValidator;
    protected  override async  Task OnInitializedAsync()
    {
        await LoadCommissions();
        await base.OnInitializedAsync();
    }

    private async Task LoadCommissions()
    {
        var response = await SiteSettingManager.GetSiteCommission();
        if (response.Succeeded)
        {
            _commissions = Mapper.Map<EditSiteCommissionCommand>(response.Data);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task SaveAsync()
    {
        _processing = true;
        var validated = 
             _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            var response = await SiteSettingManager.UpdateCommission(_commissions);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0],Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        _processing = false;
    }
}
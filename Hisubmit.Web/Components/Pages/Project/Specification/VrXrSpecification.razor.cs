using AdminDashboard.Wasm.Models;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.MonetaryUnits.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditFilmSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditVrXrSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Locations;
using HiSubmit.Client.Infrastructure.Managers.Monetaryunits;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.SubProjectTypes;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllSubProjectType;

namespace HiSubmit.Web.Components.Pages.Project.Specification;

public partial class VrXrSpecification
{
    #region Inject

    [Inject] private IMapper Mapper { get; set; }
    [Inject] private ILocationManager LocationManager { get; set; }
    [Inject] private IMonetaryUnitsManager MonetaryUnitsManager { get; set; }
    [Inject] private ISubProjectTypeManager SubProjectTypeManager { get; set; }
    [Inject] private IProjectSpecificationManager ProjectSpecificationManager { get; set; }

    #endregion

    #region  Parameters
    [CascadingParameter] public int ProjectId { get; set; }

    [Parameter] public EventCallback ClickedNextPanel { get; set; }

    #endregion

    #region  Private Feild
    
    private bool _loaded ;
    private bool _processing ;    
    private bool _validate = true;
    private EditContext _editContext;
    private List<GetAllSubProjectTypeResponse> _subProjectTypes;
    private FluentValidationValidator _fluentValidationValidator;
    private List<CheckBoxItem<int>> _subProjectTypesItem = new ();
    private List<GetAllMonetaryUnitRespnse> _monetaryUnits = new ();
    private AddEditVrXrSpecificationCommand Model { get; set; } = new();
    private List<GetAllCountryResponse> Countries { get; set; } = new();

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadMonetaryUnits();
        await LoadSpecification();
        await LoadSubProjectTypes();
        await GetCountriesAsync();
        await GenerateSubProjectTypeCheckBox();
        _editContext = new EditContext(Model);
        await base.OnInitializedAsync();
        _loaded = true;
    }


    #endregion

    #region Load Data

    private async Task LoadSpecification()
    {
        var response = await ProjectSpecificationManager
            .GetXrVRSpecification(new GetVrXrSpecificationDetailQuery() { ProjectId = ProjectId });
        if (response.Succeeded)
        {
            Model = Mapper.Map<AddEditVrXrSpecificationCommand>(response.Data);
            if (Model.SubProjectTypeIds == null)
            {
                Model.SubProjectTypeIds = new List<int>();
            }
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadMonetaryUnits()
    {
        var response = await MonetaryUnitsManager.GetAllAsync(new GetAllMonetaryUnitQuery());
        if (response.Succeeded)
        {
            _monetaryUnits = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadSubProjectTypes()
    {
        var response = await SubProjectTypeManager.GetAllAsync(new GetAllSubProjectTypeQuery()
            { ProjectType = ProjectType.Film });
        if (response.Succeeded)
        {
            _subProjectTypes = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    public async Task GetCountriesAsync()
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
    private Task<IEnumerable<int>> SearchCountries(string value,CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(Countries.Select(x => x.Id));

        return Task.FromResult(Countries.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id));
    }


    #endregion
    
    public async Task<bool> SaveAsync()
    {
        _validate = _fluentValidationValidator.Validate((p) => p.IncludeAllRuleSets());
        if (_validate)
        {
            _processing = true;
            Model.ProjectId = ProjectId;
            Model.SubProjectTypeIds = _subProjectTypesItem.Where(p => p.IsSelected).Select(p => p.Value).ToList();
            var response = await ProjectSpecificationManager.UpdateXrVrSpecification(Model);
            _processing = false;
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _editContext.MarkAsUnmodified();
                return true;
            }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }

            return false;
        }

        return false;
    }
    
    private async Task GenerateSubProjectTypeCheckBox()
    {
        await Task.Run(() =>
        {
            foreach (var item in _subProjectTypes)
            {
                bool selected = Model.SubProjectTypeIds.Any(id => item.Id == id);
                _subProjectTypesItem.Add(new CheckBoxItem<int>
                {
                    IsSelected = selected,
                    Name = item.Name,
                    Value = item.Id
                });
            }
        });
    }
    
    private async Task GoNext()
    {
        await ClickedNextPanel.InvokeAsync();
    }

    public bool ModifiedForm()
    {
        return _editContext.IsModified();
    }
   
}
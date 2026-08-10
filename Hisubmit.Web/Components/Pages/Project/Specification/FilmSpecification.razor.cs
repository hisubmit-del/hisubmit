using AdminDashboard.Wasm.Models;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.MonetaryUnits.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditFilmSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
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

public partial class FilmSpecification
{
    #region Inject

    [Inject] private IProjectSpecificationManager ProjectSpecificationManager { get; set; }
    [Inject] private ISubProjectTypeManager SubProjectTypeManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }
    [Inject] private IMonetaryUnitsManager MonetaryUnitsManager { get; set; }
    [Inject] private ILocationManager LocationManager { get; set; }

    #endregion

    #region Parameters

    [CascadingParameter] public int ProjectId { get; set; }
    [Parameter] public EventCallback ClickedNextPanel { get; set; }

    #endregion

    #region Private Field

    private AddEditFilmSpecificationCommand Model { get; set; } = new();

    private EditContext _editContext;

    private bool _loaded = false;
    private FluentValidationValidator _fluentValidationValidator;
    private bool _validate = true;
    private List<CheckBoxItem<int>> _subProjectTypesItem = new();
    private List<GetAllSubProjectTypeResponse> _subProjectTypes;
    private List<GetAllMonetaryUnitRespnse> _monetaryUnits = new();
    private List<GetAllCountryResponse> Countries { get; set; } = new();
    private bool _processing;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadSpecification();
        await LoadMonetaryUnits();
        await LoadSubProjectTypes();
        await GetCountriesAsync();
        await GenerateSubProjectTypeCheckBox();
        _editContext = new EditContext(Model);
        await base.OnInitializedAsync();
        _loaded = true;
    }

    #endregion

    #region LoadData

    private async Task LoadSpecification()
    {
        var response = await ProjectSpecificationManager
            .GetFilmSpecification(new GetFilmSpecificationDetailRequest() { ProjectId = ProjectId });
        if (response.Succeeded)
        {
            Model = Mapper.Map<AddEditFilmSpecificationCommand>(response.Data);
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

    private Task<IEnumerable<int>> SearchCountries(string value,CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(Countries.Select(x => x.Id));

        return Task.FromResult(Countries.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id));
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

    #endregion


    public async Task<bool> SaveAsync()
    {
        _validate = _fluentValidationValidator.Validate((p) => p.IncludeAllRuleSets());
        if (_validate)
        {
            _processing = true;
            Model.ProjectId = ProjectId;
            Model.SubProjectTypeIds = _subProjectTypesItem.Where(p => p.IsSelected).Select(p => p.Value).ToList();
            var response = await ProjectSpecificationManager.UpdateFilmSpecification(Model);
            _processing = false;
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                return true;
            }

            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

            return false;
        }

        return false;
    }



    private async Task GoNext()
    {
        await ClickedNextPanel.InvokeAsync();
    }

    public bool ModifiedForm()
    {
        Console.WriteLine("Modified Form Film FilmSpecification");
        return _editContext.IsModified();
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
    
    public int CountryId { get; set; }
    private Task SelectCountry(int i)
    {
        if (Model.FilmingCountryIds.All(p => p != i))
            Model.FilmingCountryIds.Add(i);
        CountryId = 0;
        return Task.CompletedTask;
    }

    private Task DeleteCountry(int item)
    {
        Model.FilmingCountryIds.Remove(item);
        return Task.CompletedTask;
    }

}
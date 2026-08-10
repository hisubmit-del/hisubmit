using System;
using AdminDashboard.Wasm.Models;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Managers.Catalog.Brand;
using HiSubmit.Client.Infrastructure.Managers.Catalog.FestivalFocus;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Hisubmit.Client.SharedModels.Extensions;

namespace HiSubmit.Web.Components.Pages.Festival.FestivalEditComponent;

public partial class FestivalAdditionalSetting
{
    #region Parameters

    [Parameter] public int FestivalId { get; set; }

[Parameter]
public bool IsAdmin { get; set; }

    [Parameter]
    public EventCallback OnReleasedFestivalClick { get; set; }

    [Parameter]
    public bool ReleasProcessing { get;set; }
    
    [Parameter]
    public EventCallback PrevPanel { get; set; }
    #endregion

    #region Inject

    [Inject] public IArtCategoryManager ArtCategoryManager { get; set; }
    [Inject] public IFestivalFocusManager FestivalFocusManager { get; set; }
    [Inject] public IFestivalManager FestivalManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    #endregion

    #region  Private Feild
    private bool _loaded;
    private bool _validate;
    private bool _processing;
    private EditContext _editForm;
    private List<int> _artCategorySelected;
    private List<int> _festivalFocusSelected;
    private AddEditAdditionalSettingCommand _model  = new();
    private List<CheckBoxItem<int>> _artCategoryList  = new();
    private List<CheckBoxItem<int>> _festivalFocusList  = new();
    private FluentValidationValidator _fluentValidationValidator;
    private List<GetAllArtCategoryResponse> _artCategories  = new();
    private List<GetAllFestivalFocusResponse> _festivalFocus  = new();
    #endregion
  
   

    protected override async Task OnInitializedAsync()
    {
        await GetFestivalFocus();
        await GetArtCategories();
        await GetAdditionalSetting();
        await GenerateArtCategoryCheckBoxItem();
        await GenerateFestivalFocusCheckBoxItem();
        _editForm = new EditContext(_model);
        await base.OnInitializedAsync();
        _loaded = true;
    }

    public async Task<bool> SaveAsync()
    {
        var result = false;
        _model.FestivalArtCategoriesId = _artCategoryList.Where(p => p.IsSelected).Select(p => p.Value).ToList();
        _model.FestivalFestivalFociId = _festivalFocusList.Where(p => p.IsSelected).Select(p => p.Value).ToList();
        _validate =await _fluentValidationValidator
            .ValidateAsync((option) => option.IncludeAllRuleSets());
        
        if (_validate)
        {
            _processing = true;
            
            var response = await FestivalManager.SaveAdditionalSetting(_model);
            _processing = false;
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], MudBlazor.Severity.Success);
                _editForm.MarkAsUnmodified();
                result = true;
            }
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, MudBlazor.Severity.Error);
        }

        return result;
    }


    private async Task GetFestivalFocus()
    {
        var response = await FestivalFocusManager.GetAllAsync(new GetAllFestivalFocusQuery());
        if (response.Succeeded)
        {
            _festivalFocus = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task GetArtCategories()
    {
        var response = await ArtCategoryManager.GetAllAsync();
        if (response.Succeeded)
        {
            _artCategories = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task GetAdditionalSetting()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = FestivalId,
            WithInclude = true
        });
        if (response.Succeeded)
        {
            _model = Mapper.Map<AddEditAdditionalSettingCommand>(response.Data);
            _festivalFocusSelected = response.Data.FestivalFestivalFoci.Select(p => p.FestivalFocusId).ToList();
            _artCategorySelected = response.Data.FestivalArtCategories.Select(p => p.ArtCategoryId).ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task GenerateFestivalFocusCheckBoxItem()
    {
        await Task.Run(() =>
        {
            foreach (var item in _festivalFocus)
            {
                bool selected = _festivalFocusSelected.Any(id => item.Id == id);
                _festivalFocusList.Add(new CheckBoxItem<int>
                {
                    IsSelected = selected,
                    Name = item.Name,
                    Value = item.Id
                });
            }
        });
    }

    private async Task GenerateArtCategoryCheckBoxItem()
    {
        await Task.Run(() =>
        {
            foreach (var item in _artCategories)
            {
                bool selected = _artCategorySelected.Any(id => item.Id == id);
                _artCategoryList.Add(new CheckBoxItem<int>
                {
                    IsSelected = selected,
                    Name = item.Name,
                    Value = item.Id
                });
            }
        });
    }

    public bool ModifiedForm()
    {
        return _editForm.IsModified();
    }

    private Task CheckCatValidate(bool t,CheckBoxItem<int> cats)
    {
        cats.IsSelected = t;
        _model.FestivalArtCategoriesId = _artCategoryList.Where(p => p.IsSelected).Select(p => p.Value).ToList();
        _model.FestivalFestivalFociId = _festivalFocusList.Where(p => p.IsSelected).Select(p => p.Value).ToList();
        _validate = _fluentValidationValidator.Validate(option=>option.IncludeAllRuleSets());
        return Task.CompletedTask;
    }

    private Task CheckFocusValidate(bool b, CheckBoxItem<int> focus)
    {
        focus.IsSelected = b;
        _model.FestivalFestivalFociId = _festivalFocusList.Where(p => p.IsSelected).Select(p => p.Value).ToList();
        _model.FestivalArtCategoriesId = _artCategoryList.Where(p => p.IsSelected).Select(p => p.Value).ToList();
        _validate = _fluentValidationValidator.Validate(option=>option.IncludeAllRuleSets());
        return Task.CompletedTask;
    }


    private async Task ReleaseFestival()
    {
        await OnReleasedFestivalClick.InvokeAsync();
    }

    private async Task GoPrev()
    {
        await PrevPanel.InvokeAsync();
    }

     private void HandleURLChange(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _model.URL = value.TrimAll();
        }
    }
}
using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;
using HiSubmit.Client.Infrastructure.Managers.FestivalSeo;
using ClientComponents.Shared.Components.Base;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.Festival.Seo;

public partial class SeoForm:BaseFestival
{
    [Inject] private IFestivalSeoManager FestivalSeoManager { get; set; }

    [Inject] private IMapper Mapper { get; set; }
    
    private AddEditSeoTagRequest _seoTags = new();
    private bool _processing;
    private FluentValidationValidator _fluentValidationValidator;

    protected override async Task OnInitializedAsync()
    {
        await CheckPermission(Permissions.FestivalSeo.View);
        await base.OnInitializedAsync();
        await LoadSeoTags();
    }

    private async Task LoadSeoTags()
    {
        LoadSelectedFestivalId();
        var res = await FestivalSeoManager.GetPageSeoTag(new GetPageSeoTagsQuery()
        {
            PageId = SelectedFestivalId.ToString()
        });
        if (res.Succeeded && res.Data != null)
            _seoTags = Mapper.Map<AddEditSeoTagRequest>(res.Data);
        else
            foreach (var message in res.Messages)
                _snackBar.Add(message, Severity.Error);
    }
    
    private async Task SaveAsync()
    {
        _processing = true;
        _seoTags.PageId = SelectedFestivalId.ToString();
        _seoTags.PageTitle=string.Empty;
        _seoTags.Type = PageType.FestivalPage;
        var res = await FestivalSeoManager.AddEditSeoTags(_seoTags);
        if (res.Succeeded)
            _snackBar.Add("Seo setting Saved successfully",Severity.Success);
        else
            foreach (var message in res.Messages)
                _snackBar.Add(message, Severity.Error);
        _processing = false;
    }
}
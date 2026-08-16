using System;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;
using HiSubmit.Client.Infrastructure.Managers.AdminSeo;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Admin.Content.Seo;

public partial class SeoForm
{
    #region Inject

    [Inject] private IAdminSeoManager AdminSeoManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }

    #endregion

    private FluentValidationValidator _fluentValidationValidator;

    private AddEditSeoTagRequest _seoTags = new();

    private PageType _pageType = PageType.HomePage;
    private bool _processing;

    protected override async Task OnInitializedAsync()
    {
        await LoadSeoTags();
        await base.OnInitializedAsync();
    }

    private async Task ChangePageType(PageType type)
    {
        _pageType = type;
        await LoadSeoTags();
    }
    private async Task LoadSeoTags()
    {
        var res = await AdminSeoManager.GetPageSeoTag(new GetPageSeoTagsQuery
        {
            PageType = _pageType,
            PageId = "0"
        });
        if (res.Succeeded)
        {
            if (res.Data != null)
                _seoTags = Mapper.Map<AddEditSeoTagRequest>(res.Data);
        }
        else
            foreach (var message in res.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private async Task SaveAsync()
    {
        _processing = true;
        _seoTags.PageId = "0";
        _seoTags.PageTitle=string.Empty;
        _seoTags.Type = _pageType;
        var res = await AdminSeoManager.AddEditSeoTags(_seoTags);
        if (res.Succeeded)
            _snackBar.Add("Seo setting Saved successfully",Severity.Success);
        else
            foreach (var message in res.Messages)
                _snackBar.Add(message, Severity.Error);
        _processing = false;
    }
}


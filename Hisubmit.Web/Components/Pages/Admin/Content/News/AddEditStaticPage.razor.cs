using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using HiSubmit.Client.Infrastructure.Managers.StaticPages;
using HiSubmit.Web.Components.Shared.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Admin.Content.News;

public partial class AddEditStaticPage
{
    [Inject] private IStaticPageManager StaticPageManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }
    [Parameter] public int PageId { get; set; }

    private AddEditStaticPageRequest _staticPage = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;

    protected override async Task OnInitializedAsync()
    {
        if (PageId != 0)
        {
            await LoadNew();
        }

        await base.OnInitializedAsync();
    }

    private async Task LoadNew()
    {
        var response =
            await StaticPageManager.GetDetailAsync(new GetDetailStaticPageQuery()
        {
            Id = PageId
        });
        if (response.Succeeded)
        {
            _staticPage = Mapper.Map<AddEditStaticPageRequest>(response.Data);
            _staticPage.SeoTag ??= new AddEditSeoTagRequest();
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
        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            var response = await StaticPageManager.SaveAsync(_staticPage);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _navigationManager.NavigateTo("/admin/contents");
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
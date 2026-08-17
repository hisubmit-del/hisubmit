using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;
using HiSubmit.Client.Infrastructure.Managers.Seo;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Web.Components.Shared.SeoTags;

public partial class SeoMetaTags
{
    #region Parameters

    [Parameter] public PageType PageType { get; set; }

    [Parameter] public string PageId { get; set; }

    [Parameter] public string OgImageUrl { get; set; }

    #endregion

    #region Inject

    [Inject]
    private ISeoManager SeoManager { get; set; }

    [Inject]
    private ILogger<SeoMetaTags> Logger { get; set; }

    #endregion

    #region Prerendering
    
    private PersistingComponentStateSubscription _subscription;

    private Task PersistFestival()
    {
        ApplicationState.PersistAsJson("seoTags", _seoTags);
        return Task.CompletedTask;
    }

    #endregion

    private GetPageSeoTagResult _seoTags=new();

    private string _index;
    private string _follow;
    protected override async Task OnInitializedAsync()
    {
        if (ApplicationState.TryTakeFromJson
                <GetPageSeoTagResult>
                ("seoTags", out var stored))
        {
            _seoTags = stored;
        }
        else
        {
            try
            {
                var res = await SeoManager.GetPageSeoTag(new GetPageSeoTagsQuery()
                {
                    PageType = PageType,
                    PageId = PageId
                });
                if (res.Succeeded && res.Data != null)
                    _seoTags = res.Data;
            }
            catch (Exception exception)
            {
                // SEO is optional metadata. A temporary database/API failure
                // must not prevent the requested page from rendering.
                Logger.LogWarning(exception, "Could not load SEO metadata for page type {PageType} and page id {PageId}.", PageType, PageId);
            }
        }

        _seoTags ??= new GetPageSeoTagResult();
        _seoTags.Title ??= "HiSubmit";
        _seoTags.Description ??= "HiSubmit online submission platform";
        _seoTags.MetaKeywords ??= "festivals, artists, submissions";

        _index = _seoTags.Index ? "index" : "noindex";
        _follow = _seoTags.Follow ? "follow" : "nofollow";
        
        await base.OnInitializedAsync();
    }
}


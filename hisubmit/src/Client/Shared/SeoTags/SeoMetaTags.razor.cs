using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;
using HiSubmit.Client.Infrastructure.Managers.Seo;

using Microsoft.AspNetCore.Components;

namespace HiSubmit.Client.Shared.SeoTags;

public partial class SeoMetaTags
{
    #region Parameters

    [Parameter] public PageType PageType { get; set; }

    [Parameter] public string PageId { get; set; }

    #endregion

    #region Inject

    [Inject]
    private ISeoManager SeoManager { get; set; }

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
            var res = await SeoManager.GetPageSeoTag(new GetPageSeoTagsQuery()
            {
                PageType = PageType,
                PageId = PageId
            });
            if (res.Succeeded && res.Data!=null)
                _seoTags = res.Data; 
        }

      

        _index = _seoTags.Index ? "index" : "noindex";
        _follow = _seoTags.Follow ? "follow" : "nofollow";
        
        await base.OnInitializedAsync();
    }
}


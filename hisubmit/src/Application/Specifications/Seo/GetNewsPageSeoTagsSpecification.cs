using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.SeoTags;

namespace HiSubmit.Application.Specifications.Seo;

public class GetNewsPageSeoTagsSpecification:HeroSpecification<MetaTag>
{
    public GetNewsPageSeoTagsSpecification(string pageId)
    {
        Criteria = metaTag => metaTag.Type == PageType.News && metaTag.PageId == pageId;
    }
}
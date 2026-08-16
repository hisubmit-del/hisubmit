using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;

namespace HiSubmit.Client.Infrastructure.Mappings;

public class SeoProfile:Profile
{
    public SeoProfile()
    {
        CreateMap<GetPageSeoTagResult, AddEditSeoTagRequest>()
            .ReverseMap();
    }
}
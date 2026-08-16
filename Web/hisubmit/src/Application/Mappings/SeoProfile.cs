using AutoMapper;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Application.Features.Seo;
using HiSubmit.Application.Features.Seo.GetPAgeSeoTags;
using Hisubmit.Client.SharedModels.Features.Seo;

namespace HiSubmit.Application.Mappings;

public class SeoProfile:Profile
{
    public SeoProfile()
    {
        CreateMap<AddEditSeoTagRequest, MetaTag>()
            .ForMember(p=>p.PageTitle,map=>map.Ignore())
            .ForMember(p=>p.PageId,map=>map.Ignore())
            .ForMember(p=>p.Id,map=>map.Ignore())
            .ForMember(p=>p.Type,map=>map.Ignore())
            .ReverseMap();

        CreateMap<GetPageSeoTagResult, MetaTag>().ReverseMap();
    }
}

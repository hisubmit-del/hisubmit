using AutoMapper;
using HiSubmit.Application.Features.StaticPages.Commands;
using HiSubmit.Application.Features.StaticPages.Queries;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using HiSubmit.Domain.Entities.Content;
using ContentType = HiSubmit.Domain.Entities.Content.ContentType;

namespace HiSubmit.Application.Mappings;

public class StaticPageProfile:Profile
{
    public StaticPageProfile()
    {
        CreateMap<StaticPageAndFAQ, GetAllStaticPageResponse>()
            .ForMember(p=>p.Content,map=>map
                .MapFrom(src=>src.Type==ContentType.Faq?src.Content:string.Empty))
            .ReverseMap();
        CreateMap<StaticPageAndFAQ, GetDetailStaticPageResponse>().ReverseMap();
        CreateMap<StaticPageAndFAQ, AddEditStaticPageRequest>().ReverseMap();
    }
}

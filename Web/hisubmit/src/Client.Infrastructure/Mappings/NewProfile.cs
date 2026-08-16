using AutoMapper;
using Hisubmit.Client.SharedModels.Features.News.Commands;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;

namespace HiSubmit.Client.Infrastructure.Mappings;

public class NewProfile:Profile
{
    public NewProfile()
    {
        CreateMap<AddEditNewCommand, GetDetailNewResponse>().ReverseMap()
            ;

        CreateMap<AddEditStaticPageRequest, GetDetailStaticPageResponse>().ReverseMap();
    }
}
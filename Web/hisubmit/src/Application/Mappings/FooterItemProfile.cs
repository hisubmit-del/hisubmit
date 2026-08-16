using AutoMapper;
using HiSubmit.Application.Features.FooterItems;
using HiSubmit.Application.Features.FooterItems.Commands;
using HiSubmit.Domain.Entities.Content;

namespace HiSubmit.Application.Mappings;

public class FooterItemProfile:Profile
{
    public FooterItemProfile()
    {
        CreateMap<MenuItem, FooterItemDto>().ReverseMap();
        CreateMap<AddEditFooterItemCommand, MenuItem>().ReverseMap();
    }
}
using AutoMapper;
using HiSubmit.Application.Features.News.Commands;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Domain.Entities.Content;
using GetDetailNewResponse = HiSubmit.Application.Features.News.Queries.GetDetailNewResponse;

namespace HiSubmit.Application.Mappings;

public class NewProfile:Profile
{
    public NewProfile()
    {
        CreateMap<New, GetAllNewResponse>().ReverseMap();
        CreateMap<New, GetDetailNewResponse>().ReverseMap();
        CreateMap<New, AddEditNewCommand>().ReverseMap();
    }
}
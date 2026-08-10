using AutoMapper;
using HiSubmit.Application.Features.Brands.Commands.AddEdit;
using HiSubmit.Application.Features.Brands.Queries.GetAll;
using HiSubmit.Application.Features.Brands.Queries.GetById;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetById;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Application.Mappings
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<AddEditArtCategoryCommand, ArtCategory>().ReverseMap();
            CreateMap<GetBrandByIdResponse, ArtCategory>().ReverseMap();
            CreateMap<GetAllArtCategoryResponse, ArtCategory>().ReverseMap();
        }
    }
}
using AutoMapper;
using HiSubmit.Application.Features.Products.Commands.AddEdit;
using HiSubmit.Application.Features.Products.Queries.GetAllPaged;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Seo;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Domain.Entities.SeoTags;

namespace HiSubmit.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<GetAllPagedProductsResponse, Product>().ReverseMap();
        
        CreateMap<AddEditProductCommand, Product>().ReverseMap();

        CreateMap<ProductImage, ProductImageDto>()
            .ReverseMap();

        CreateMap<Product, AddEditProductRequest>()
            .ReverseMap();

        CreateMap<AddEditSeoTagRequest, MetaTag>()
            .ReverseMap();
    }
}
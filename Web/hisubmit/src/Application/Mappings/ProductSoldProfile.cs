using AutoMapper;
using HiSubmit.Application.Features.SoldProducts.Commands;
using HiSubmit.Application.Features.SoldProducts.Queries;
using HiSubmit.Domain.Entities.Payments;

namespace HiSubmit.Application.Mappings;

public class ProductSoldProfile:Profile
{
    public ProductSoldProfile()
    {
        CreateMap<GetAllSoldProductResponse, ProductSold>()
            .ReverseMap()
            .ForMember(des => des.ProductName, map => map.MapFrom(src => src.Product.Name))
            .ForMember(des => des.ProductType, map => map.MapFrom(src => src.Product.ProductType));

        CreateMap<GetSoldProductDetailResponse, ProductSold>()
            .ReverseMap();

        CreateMap<AddProductSoldCommand, ProductSold>().ReverseMap();
    }
}
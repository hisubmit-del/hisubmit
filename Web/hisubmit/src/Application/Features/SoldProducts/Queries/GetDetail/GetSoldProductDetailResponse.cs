using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Application.Features.Payments.Queries;
using HiSubmit.Application.Features.Products.Queries.GetAllPaged;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.SoldProducts.Queries;

public class GetSoldProductDetailResponse
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public int ProductId { get; set; }
    public int AddressId { get; set; }
    public decimal Income { get; set; }
    public decimal ShareFestivalIncome { get; set; }
    public string UserName { get; set; }
    public ProductSoldStatus Status { get; set; }
    public AddEditAddressCommand Address { get; set; }
    public GetAllPagedProductsResponse Product { get; set; }
    public object UserPhoneNumber { get; set; }
    public object UserEmail { get; set; }

    public GetCartItemResponse CartItem { get; set; }
}
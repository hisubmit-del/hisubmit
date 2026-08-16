using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;

namespace Hisubmit.Client.SharedModels.Features.SoldProducts.Commands;

public class AddProductSoldCommand 
{
    public string Email { get; set; }
    public int ProductId { get; set; }
    public  ProductType ProductType { get; set; }
    public ProductSoldStatus Status { get; set; }
    public AddEditAddressCommand Address { get; set; }
}

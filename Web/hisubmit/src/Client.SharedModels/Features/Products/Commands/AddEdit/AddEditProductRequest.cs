using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;

public class AddEditProductRequest
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public int FestivalId { get; set; }
    [Required] public string Description { get; set; }
    public string ImageDataURL { get; set; }
    public ProductType ProductType { get; set; }
    public UploadRequest UploadRequest { get; set; } = new UploadRequest() { UploadType = UploadType.Product };

    public AddEditSeoTagRequest SeoTag { get; set; } = new();
    public List<ProductImageDto> ProductImages { get; set; } = new();
}

public class ProductImageDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Url { get; set; }
    public UploadRequest UploadRequest { get; set; }
}


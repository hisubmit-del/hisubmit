namespace Hisubmit.Client.SharedModels.Features.Products.Queries.GetProductImage;

public class GetProductImageQuery 
{
    public int Id { get; set; }

    public GetProductImageQuery(int productId)
    {
        Id = productId;
    }
}
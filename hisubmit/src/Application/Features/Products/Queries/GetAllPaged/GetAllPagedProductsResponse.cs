using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Products.Queries.GetAllPaged
{
    public class GetAllPagedProductsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
      //  public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public int FestivalId { get; set; }
        public  bool IsEnable { get; set; }
        public  string FestivalName { get; set; }
        public string ImageDataURL { get; set; }
        public ProductType ProductType { get; set; }
    }
}
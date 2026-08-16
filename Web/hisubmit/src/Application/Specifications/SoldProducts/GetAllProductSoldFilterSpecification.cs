using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;

namespace HiSubmit.Application.Specifications.SoldProducts;

internal class GetAllSoldProductFilterSpecification : HeroSpecification<ProductSold>
{
    public GetAllSoldProductFilterSpecification(string searchString)
    {
        AddInclude(productSold => productSold.Product);

        Criteria = productSold => string.IsNullOrWhiteSpace(searchString) ||
                                  productSold.Email.Contains(searchString) ||
                                  productSold.Product.Name.Contains(searchString);
    }
}
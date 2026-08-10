using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Application.Specifications.Catalog;

public class ProductFilterSpecification : HeroSpecification<Product>
{
    public ProductFilterSpecification(string searchString,int? festivalId,bool? isEnable)
    {
        Includes.Add(a => a.Festival);
        if (!string.IsNullOrEmpty(searchString))
        {
            Criteria = p =>(festivalId==null || p.FestivalId==festivalId) && 
                           (isEnable==null || p.IsEnable==isEnable.Value) &&
                           (p.Name.Contains(searchString) || p.Description.Contains(searchString)
                                                          || p.Barcode.Contains(searchString) ||
                                                          p.Festival.Name.Contains(searchString));
        }
        else
        {
            Criteria = p=> festivalId==null || p.FestivalId==festivalId  ;
        }
    }
}
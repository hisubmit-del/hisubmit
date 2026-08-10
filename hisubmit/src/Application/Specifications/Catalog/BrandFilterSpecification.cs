using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Application.Specifications.Catalog
{
    public class BrandFilterSpecification : HeroSpecification<ArtCategory>
    {
        public BrandFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.Description.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}

using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class FestivalArtCategory : AuditableEntity<int>
    {
        public int FestivalId { get; set; }
        public int ArtCategoryId { get; set; }
        public Festival Festival { get; set; }
        public ArtCategory ArtCategory { get; set; }
    }
}


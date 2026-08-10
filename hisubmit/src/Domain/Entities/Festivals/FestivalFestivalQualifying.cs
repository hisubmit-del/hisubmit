using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class FestivalFestivalQualifying : AuditableEntity<int>
    {
        public int FestivalId { get; set; }
        public int FestivalQualifyingId { get; set; }
        public  Festival Festival { get; set; }
        public FestivalQualifying FestivalQualifying { get; set; }
    }
}


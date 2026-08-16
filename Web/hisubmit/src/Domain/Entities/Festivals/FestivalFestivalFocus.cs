using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class FestivalFestivalFocus : AuditableEntity<int>
    {
        public int FestivalId { get; set; }
        public int FestivalFocusId { get; set; }
        public Festival Festival { get; set; }
        public FestivalFocus FestivalFocus { get; set; }
    }
}


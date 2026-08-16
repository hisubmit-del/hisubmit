using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class FestivalFile : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string FileURL { get; set; }
        public FileFormat FileFormat { get; set; }
        public string Description { get; set; }
        public int FestivalId { get; set; }
        public Festival Festival { get; set; }
    }
}


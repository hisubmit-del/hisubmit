using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Locations
{
    public class Country:AuditableEntity<int>
    {
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string Code { get; set; }
        public int Int { get; set; }
    }
}

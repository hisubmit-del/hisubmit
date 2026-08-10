using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects
{
    public class SubProjectTypeFilmSpecification : AuditableEntity<int>
    {
        public int SubProjectTypeId { get; set; }
        public int FilmSpecificationId { get; set; }

        public FilmSpecification FilmSpecification { get; set; }
        public SubProjectType SubProjectType { get; set; }
    }
}

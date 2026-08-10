using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Submitter;

namespace HiSubmit.Application.Specifications.Submits
{
    public class GetAllProjectSubmitSpecification : HeroSpecification<Submit>
    {
        public GetAllProjectSubmitSpecification(int? projectId)
        {
            Criteria = submit => (projectId == null || submit.ProjectId == projectId);
        }
    }
}

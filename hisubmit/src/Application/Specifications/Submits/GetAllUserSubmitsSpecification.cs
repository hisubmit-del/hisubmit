using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Entities.Submitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Specifications.Submits
{
    public class GetAllUserSubmitsIdSpecification : HeroSpecification<Submit>
    {
        public GetAllUserSubmitsIdSpecification(string userId)
        {
            AddInclude(submit => submit.Project);
            Criteria = submit => (string.IsNullOrWhiteSpace(userId) || submit.Project.UserId == userId);
        }
    }
    
   
}

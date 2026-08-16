using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Festivals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Specifications.Festivals
{
    public class GetNextDeadLine : HeroSpecification<DeadLine>
    {
        public GetNextDeadLine()
        {
            Criteria = (deadLine) => deadLine.Date >= DateTime.Now;
        }
    }
}
